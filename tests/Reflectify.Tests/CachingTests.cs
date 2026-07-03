using Xunit;
using Reflectify;
using Reflectify.Models;

namespace Reflectify.Tests;

public class CachingTests
{
    [Fact]
    public void GetProperties_MultipleCalls_ReturnsSameData()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());

        var first = reflectify.GetProperties(typeof(CacheModel));
        var second = reflectify.GetProperties(typeof(CacheModel));

        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.Equal(first!.Count, second!.Count);

        for (int i = 0; i < first.Count; i++)
        {
            Assert.Equal(first[i].PropertyInfo.Name, second[i].PropertyInfo.Name);
        }
    }

    [Fact]
    public void GetMethods_MultipleCalls_ReturnsSameData()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());

        var first = reflectify.GetMethods(typeof(CacheModel));
        var second = reflectify.GetMethods(typeof(CacheModel));

        Assert.Equal(first.Count, second.Count);

        for (int i = 0; i < first.Count; i++)
        {
            Assert.Equal(first[i].Method.Name, second[i].Method.Name);
        }
    }

    [Fact]
    public void GetTypeInfo_MultipleCalls_ReturnsSameType()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());

        var first = reflectify.GetTypeInfo(typeof(CacheModel));
        var second = reflectify.GetTypeInfo(typeof(CacheModel));

        Assert.Equal(first.Type, second.Type);
    }

    [Fact]
    public void GetProperties_DifferentTypes_DontInterfere()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());

        var propsA = reflectify.GetProperties(typeof(CacheModel));
        var propsB = reflectify.GetProperties(typeof(AnotherModel));

        Assert.NotNull(propsA);
        Assert.NotNull(propsB);

        var namesA = propsA!.Select(p => p.PropertyInfo.Name).OrderBy(x => x).ToList();
        var namesB = propsB!.Select(p => p.PropertyInfo.Name).OrderBy(x => x).ToList();

        Assert.NotEqual(namesA, namesB);
    }

    [Fact]
    public void GenericOverload_CachesWithTypeOverload_SameCache()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());

        var instance = new CacheModel();
        var fromGeneric = reflectify.GetProperties(instance);
        var fromType = reflectify.GetProperties(typeof(CacheModel));

        Assert.Equal(fromGeneric!.Count, fromType!.Count);
    }

    [Fact]
    public void GetProperty_AfterGetProperties_ReturnsCachedData()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());

        var allProps = reflectify.GetProperties(typeof(CacheModel));
        var singleProp = reflectify.GetProperty(typeof(CacheModel), "Name");

        Assert.NotNull(singleProp);
        Assert.Equal("Name", singleProp!.PropertyInfo.Name);
    }

    [Fact]
    public async Task ConcurrentAccess_DoesNotThrow()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var type = typeof(CacheModel);
        var exceptions = new List<Exception>();
        var lockObj = new object();

        var tasks = Enumerable.Range(0, 20).Select(i => Task.Run(() =>
        {
            try
            {
                if (i % 3 == 0)
                    reflectify.GetProperties(type);
                else if (i % 3 == 1)
                    reflectify.GetMethods(type);
                else
                    reflectify.GetTypeInfo(type);
            }
            catch (Exception ex)
            {
                lock (lockObj) { exceptions.Add(ex); }
            }
        }));

        await Task.WhenAll(tasks);

        Assert.Empty(exceptions);
    }
}

public class CacheModel
{
    public string Name { get; set; } = "";
    public int Value { get; set; }

    public void Process(int x, string y) { }

    public void Execute() { }
}

public class AnotherModel
{
    public DateTime Timestamp { get; set; }
    public Guid Id { get; set; }
}
