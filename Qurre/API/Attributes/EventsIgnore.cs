using System;

namespace Qurre.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class EventsIgnore : Attribute
    {
        public EventsIgnore() { }
    }
}