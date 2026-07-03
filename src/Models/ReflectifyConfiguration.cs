using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Reflectify.Models;

public class ReflectifyConfiguration
{
    /// <summary>
    /// Controls the lifetime of the <see cref="IReflectify"/> service. Default is Singleton.
    /// </summary>
    public LifeTime LifeTime { get; set; } = LifeTime.Singleton;

    /// <summary>
    /// Controls the search memebers used in reflection. Default is OnlyPublic.
    /// </summary>
    public DetectionVisibility DetectionVisibility { get; set; } = DetectionVisibility.OnlyPublic;
}

public enum LifeTime
{
    Singleton,
    Scoped,
    Transient
}

public enum DetectionVisibility
{
    OnlyPublic,
    All
}
