using System;
using JetBrains.Annotations;

namespace Qurre.API.Attributes;

[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
internal class PluginPriority(int priority) : Attribute
{
    public int Priority { get; } = priority;
}