using System;
using System.Linq;

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
            try
            {
                Version = new(version);
            }
            catch
            {
                int[] versions = version.Split('.').Select(x => TryParse(x)).ToArray();

                if (versions.Length >= 4)
                    Version = new(versions[0], versions[1], versions[2], versions[3]);
                else if (versions.Length == 3)
                    Version = new(versions[0], versions[1], versions[2]);
                else if (versions.Length == 2)
                    Version = new(versions[0], versions[1]);
                else if (versions.Length == 1)
                    Version = new(versions[0], 0);
                else
                    Version = new(0, 0);

            }

            static int TryParse(string str)
            {
                try { return int.Parse(str); }
                catch { return 0; }
            }
        }
    }
}