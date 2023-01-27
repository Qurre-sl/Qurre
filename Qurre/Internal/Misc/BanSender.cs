using CommandSystem;

namespace Qurre.Internal.Misc
{
    internal class BanSender : ICommandSender
    {
        public string LogName { get; }
        public void Respond(string message, bool success = true) { }

        internal BanSender(string logName)
        {
            LogName = logName;
        }
    }
}