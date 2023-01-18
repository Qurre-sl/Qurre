using System;

namespace Qurre.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class PluginPriority : Attribute
    {
        public PluginPriority(int priority) => Priority = priority;

        public int Priority { get; }
    }
}