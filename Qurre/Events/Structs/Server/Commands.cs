using System.Reflection;
using JetBrains.Annotations;
using Qurre.API;
using Qurre.API.Controllers;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class RequestPlayerListCommandEvent : IBaseEvent
{
    private const uint EventID = ServerEvents.RequestPlayerListCommand;

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
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class RemoteAdminCommandEvent : IBaseEvent
{
    private const uint EventID = ServerEvents.RemoteAdminCommand;
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
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class GameConsoleCommandEvent : IBaseEvent
{
    private const uint EventID = ServerEvents.GameConsoleCommand;

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
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class ServerConsoleCommandEvent : IBaseEvent
{
    private const uint EventID = ServerEvents.ServerConsoleCommand;

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
    public uint EventId { get; } = EventID;
}