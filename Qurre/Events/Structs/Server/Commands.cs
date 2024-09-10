using System.Reflection;
using JetBrains.Annotations;
using Qurre.API;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class RequestPlayerListCommandEvent : IBaseEvent
{
    internal RequestPlayerListCommandEvent(CommandSender sender, Player? player, string command)
    {
        Sender = sender;
        Player = player ?? Server.Host;
        Command = command;
        Reply = string.Empty;
        Allowed = true;
    }

    public CommandSender Sender { get; }
    public Player Player { get; }
    public string Command { get; }
    public string Reply { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = ServerEvents.RequestPlayerListCommand;
}

[PublicAPI]
public class RemoteAdminCommandEvent : IBaseEvent
{
    private string _reply = string.Empty;

    internal RemoteAdminCommandEvent(CommandSender sender, Player? player, string command, string name, string[] args)
    {
        Sender = sender;
        Player = player ?? Server.Host;

        Command = command;
        Name = name;
        Args = args;

        Prefix = string.Empty;
        Success = true;
        Allowed = true;
    }

    public CommandSender Sender { get; }
    public Player Player { get; }

    public string Command { get; }
    public string Name { get; }
    public string[] Args { get; }

    public string Reply
    {
        get => _reply;
        set
        {
            if (string.IsNullOrEmpty(Prefix))
                Prefix = Assembly.GetCallingAssembly().GetName().Name;

            _reply = value;
        }
    }

    public string Prefix { get; set; }
    public bool Success { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = ServerEvents.RemoteAdminCommand;
}

[PublicAPI]
public class GameConsoleCommandEvent : IBaseEvent
{
    internal GameConsoleCommandEvent(Player? player, string command, string name, string[] args)
    {
        Player = player ?? Server.Host;

        Command = command;
        Name = name;
        Args = args;

        Reply = string.Empty;
        Color = "white";
        Allowed = true;
    }

    public Player Player { get; }

    public string Command { get; }
    public string Name { get; }
    public string[] Args { get; }

    public string Reply { get; set; }
    public string Color { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = ServerEvents.GameConsoleCommand;
}

[PublicAPI]
public class ServerConsoleCommandEvent : IBaseEvent
{
    internal ServerConsoleCommandEvent(string command, string name, string[] args)
    {
        Command = command;
        Name = name;
        Args = args;

        Reply = string.Empty;
        Allowed = true;
    }

    public string Command { get; }
    public string Name { get; }
    public string[] Args { get; }

    public string Reply { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = ServerEvents.ServerConsoleCommand;
}