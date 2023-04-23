namespace Qurre.API.Addons
{
    public class Version
    {
        const string s_major = "2";
        const string s_minor = "0";
        const string s_build = "0";
        const string s_revision = "20";

        internal const string AssemblyVersion = $"{s_major}.{s_minor}.{s_build}.{s_revision}";
        internal const string AssemblyCustom = "v2-gamma";

        static readonly int _major = int.Parse(s_major);
        static readonly int _minor = int.Parse(s_minor);
        static readonly int _build = int.Parse(s_build);
        static readonly int _revision = int.Parse(s_revision);

        static public int Major => _major;
        static public int Minor => _minor;
        static public int Build => _build;
        static public int Revision => _revision;

        static public string Testing => "-gamma";

        internal Version() { }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Testing))
            {
                return $"{Major}{Testing} r-{Revision}";
            }

            string vers = $"{Major}";

            if (Minor > 0)
            {
                vers += $".{Minor}";

                if (Build > 0)
                {
                    vers += $".{Build}";

                    if (Revision > 0)
                        vers += $".{Revision}";
                }
            }

            return vers;
        }
    }
}