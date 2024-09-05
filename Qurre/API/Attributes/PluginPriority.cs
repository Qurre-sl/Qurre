using System;
namespace Qurre.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class PluginPriority : Attribute
    {
        public int Priority { get; }
        public PluginPriority(int priority) => Priority = priority;
    }
}