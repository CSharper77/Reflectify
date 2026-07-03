using System.Reflection;

namespace Reflectify.Models;

/// <summary>Wraps a <see cref="MethodInfo"/> together with its custom attributes and provides helpers for invocation.</summary>
public class ExtendedMethodInfo : AttributesInfo
{
    /// <summary>The underlying method information.</summary>
    public MethodInfo Method { get; }

    /// <summary>Initializes the wrapper with the method and its attributes.</summary>
    public ExtendedMethodInfo(MethodInfo method, List<Attribute> attributes) : base(attributes)
    {
        Method = method;
    }

    /// <summary>Invokes the method synchronously with the given arguments.</summary>
    public object? Invoke(object? instance, params object[] args)
    {
        return Method.Invoke(instance, args);
    }

    /// <summary>Invokes the method with optional generic type arguments and returns the typed result.</summary>
    public TReturn? Invoke<TReturn>(object? instance, Type[]? genericTypes = null, params object[] args)
    {
        var method = genericTypes?.Length > 0 ? Method.MakeGenericMethod(genericTypes) : Method;
        return (TReturn?)method.Invoke(instance, args);
    }

    private bool IsVoidTaskMethod()
    {
        var methodReturnType = Method.ReturnType;
        var result = methodReturnType.IsGenericType && methodReturnType.GetGenericTypeDefinition() == typeof(Task<>);
        return !result;
    }
    
    /// <summary>Invokes the method asynchronously and returns the result as <see cref="object"/>.</summary>
    public async Task<object?> InvokeAsync(object? instance, Type[]? genericTypes = null, params object[] args)
    {
        var method = genericTypes?.Length > 0 ? Method.MakeGenericMethod(genericTypes) : Method;
        var result = method.Invoke(instance, args);
        
        if (result is Task task)
        {
            await task.ConfigureAwait(false);
            
            var methodReturnType = Method.ReturnType;
            if (!IsVoidTaskMethod())
            {
                var resultProperty =  task.GetType().GetProperty("Result");
                return resultProperty?.GetValue(task);
            }
            else
            {
                throw new ArgumentException($"The method {Method.Name} returned an unexpected result type.");
            }
        }
        throw new ArgumentException($"The method {Method.Name} is not a task method.");
    }

    /// <summary>Invokes the method asynchronously and returns the typed result.</summary>
    public async Task<TResult?> InvokeAsync<TResult>(object? instance, Type[]? genericTypes = null, params object[] args) where TResult : class
    {
        return (TResult?)await InvokeAsync(instance, genericTypes, args).ConfigureAwait(false);
    }

    /// <summary>Invokes an async void / Task method without capturing the return value.</summary>
    public async Task InvokeVoidAsync(object? instance, Type[]? genericTypes = null, params object[] args)
    {
        var method = genericTypes?.Length > 0 ? Method.MakeGenericMethod(genericTypes) : Method;
        var result = method.Invoke(instance, args);
        if (result is Task task)
        {
            await task.ConfigureAwait(false);
        }
        else
        {
            throw new ArgumentException($"The method {Method.Name} is not a task method.");
        }
    }
    
}