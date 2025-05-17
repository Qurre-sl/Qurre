using Newtonsoft.Json.Linq;
using Qurre.API;
using Qurre.API.Addons;

namespace Qurre.Loader;

internal static class Configs
{
    private static JsonConfig Config { get; set; } = null!;

    internal static bool ShowCredits { get; set; }
    internal static bool PrintLogo { get; private set; }
    internal static bool RoundEndChaos { get; private set; }

    internal static string Banned { get; private set; } = "You have been banned. Reason: ";
    internal static string Kicked { get; private set; } = "You have been kicked. Reason: ";

    internal static void Setup()
    {
        JsonConfig.Init();
        Config = new JsonConfig("Qurre");

        Log.Debugging = Config.SafeGetValue("Debug", false, "Are Debug logs enabled?");
        Log.Logging = Config.SafeGetValue("Logging", true, "Are errors saved to the log file?");
        Log.AllLogging = Config.SafeGetValue("AllLogging", false, "Are all console output being saved to a log file?");

        ShowCredits = Config.SafeGetValue("ShowCredits", true, "Show credits of Qurre?");
        PrintLogo = Config.SafeGetValue("PrintLogo", true, "Print Qurre Logo?");
        RoundEndChaos = Config.SafeGetValue("RoundEndWithScpAndChaos", true,
            "Allow round to end if only SCP and Chaos are left");

        Paths.UpdatePluginsDirectory(
            Config.SafeGetValue("PluginsDirectory", "Plugins", "Custom Plugins name directory"));

        SetupTranslations();

        JsonConfig.UpdateFile();
    }

    private static void SetupTranslations()
    {
        JToken? par = Config.JsonArray["Translations"];
        if (par is null)
        {
            par = JObject.Parse("{ }");
            Config.JsonArray["Translations"] = par;
        }

        Banned = Config.SafeGetValue("Banned", Banned, source: par);
        Kicked = Config.SafeGetValue("Kicked", Kicked, source: par);
    }
}