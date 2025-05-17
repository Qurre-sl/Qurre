namespace Qurre.API.Addons;

public class Version
{
    private const string SMajor = "2";
    private const string SMinor = "0";
    private const string SBuild = "0";
    private const string SRevision = "185";

    private const string STesting = "zeta";

    internal const string AssemblyVersion = $"{SMajor}.{SMinor}.{SBuild}.{SRevision}";
    internal const string AssemblyCustom = $"v2-{STesting}";

    internal Version()
    {
    }

    public static int Major { get; } = int.Parse(SMajor);

    public static int Minor { get; } = int.Parse(SMinor);

    public static int Build { get; } = int.Parse(SBuild);

    public static int Revision { get; } = int.Parse(SRevision);

    public static string Testing => STesting;

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(Testing)) return $"{Major}-{Testing} r-{Revision}";

        string version = $"{Major}";

        if (Minor <= 0)
            return version;

        version += $".{Minor}";

        if (Build <= 0)
            return version;

        version += $".{Build}";

        if (Revision <= 0)
            return version;

        version += $".{Revision}";

        return version;
    }
}