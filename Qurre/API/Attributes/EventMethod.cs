using System;

namespace Qurre.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EventMethod : Attribute
    {
        public uint Type { get; }
        public int Priority { get; }

        public EventMethod(uint type, int priority = 0)
        {
            Type = type;
            Priority = priority;
        }
    }
}