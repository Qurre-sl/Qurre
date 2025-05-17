using CommandSystem;

namespace Qurre.Internal.Misc;

internal class BanSender : ICommandSender
{
    internal BanSender(string logName)
    {
        LogName = logName;
    }

    public string LogName { get; }

    public void Respond(string message, bool success = true)
    {
    }
}