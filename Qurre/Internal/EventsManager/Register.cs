using System;
namespace Qurre.Internal.EventsManager
{
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
    public class Register : Attribute
    {
        public uint Type { get; }
        public Register(uint type) => Type = type;
    }
}