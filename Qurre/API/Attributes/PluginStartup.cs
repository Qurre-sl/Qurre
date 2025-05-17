using System;
using JetBrains.Annotations;

namespace Qurre.API.Attributes;

[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class PluginEnable : Attribute
{
}

[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class PluginDisable : Attribute
{
}