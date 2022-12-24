using System;
namespace Qurre.Events
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class EventMethod : Attribute
    {
        public uint Type { get; }
        public EventMethod(uint type) => Type = type;
    }
}