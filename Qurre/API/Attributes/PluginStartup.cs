using System;
namespace Qurre.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PluginEnable : Attribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PluginDisable : Attribute { }
}