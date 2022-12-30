using System;
namespace Qurre.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PluginInit : Attribute
    {
        public string Name { get; }
        public string Developer { get; }
        public Version Version { get; }

        public PluginInit(string name, string developer = "", string version = "")
        {
            Name = name;
            Developer = developer;
            Version = new(version);
        }
    }
}