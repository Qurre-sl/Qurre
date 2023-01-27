namespace Qurre.API.Addons
{
    public class Version
    {
        static public int Major => 2;
        static public int Minor => 0;
        static public int Build => 0;
        static public int Revision => 5;

        static public string Testing => "-beta";

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