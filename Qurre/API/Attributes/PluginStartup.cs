using System;

namespace Qurre.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PluginEnable : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class PluginDisable : Attribute { }
}