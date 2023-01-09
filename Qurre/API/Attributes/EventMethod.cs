using System;
namespace Qurre.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EventMethod : Attribute
    {
        public uint Type { get; }
        public EventMethod(uint type) => Type = type;
    }
}