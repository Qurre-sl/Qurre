using System;
using JetBrains.Annotations;

namespace Qurre.API.Attributes;

[PublicAPI]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[MeansImplicitUse(ImplicitUseTargetFlags.Itself)]
public class EventMethod(uint type, int priority = 0) : Attribute
{
    public uint Type { get; } = type;
    public int Priority { get; } = priority;
}