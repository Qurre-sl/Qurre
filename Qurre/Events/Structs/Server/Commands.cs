using System;
using System.Reflection;
using CommandSystem;
using JetBrains.Annotations;
using LabApi.Events.Arguments.ServerEvents;
using Qurre.API;
using Qurre.API.Entities.Characters;

namespace Qurre.Events.Structs;

/// <summary>
///     Event fired when the <c>REQUEST_PLAYER_LIST</c> remote-admin command is executed.
/// </summary>
[PublicAPI]
public sealed class RequestPlayerListCommandEvent : IBaseEvent
{
    #region Constants

    // Unique identifier of the event (do not rename or alter).
    private const uint EventID = ServerEvents.RequestPlayerListCommand;

    #endregion

    #region Constructors

    /// <summary>
    ///     Creates an instance of <see cref="RequestPlayerListCommandEvent" />.
    /// </summary>
    /// <param name="sender">RA sender of the command.</param>
    /// <param name="player">Player that executed the command (falls back to host).</param>
    /// <param name="command">Full raw command string.</param>
    internal RequestPlayerListCommandEvent(
        CommandSender sender,
        Player? player,
        string command)
    {
        Sender = sender;
        Player = player ?? Server.Host;
        Command = command;

        // Defaults
        Reply = string.Empty;
        Allowed = true;
    }

    #endregion

    #region Public API

    /// <summary>Remote-admin sender of the command.</summary>
    public CommandSender Sender { get; }

    /// <summary>Player associated with the command (host if <see langword="null" />).</summary>
    public Player Player { get; }

    /// <summary>Raw command string.</summary>
    public string Command { get; }

    /// <summary>
    ///     Gets or sets reply that will be sent back to the console.
    /// </summary>
    public string Reply { get; set; }

    /// <summary>
    ///     Gets or sets whether the command will be processed by the game.
    /// </summary>
    public bool Allowed { get; set; }

    /// <inheritdoc />
    public uint EventId { get; } = EventID;

    #endregion
}

/// <summary>
///     Event fired for any remote-admin command that reaches the server.
/// </summary>
[PublicAPI]
public sealed class RemoteAdminCommandEvent : IBaseEvent
{
    #region Constants

    private const uint EventID = ServerEvents.RemoteAdminCommand;

    #endregion

    #region Private fields

    // Backing field for the mutable <see cref="Reply"/> property
    private string _reply = string.Empty;

    #endregion

    #region Constructors

    /// <summary>
    ///     Creates an instance of <see cref="RemoteAdminCommandEvent" />.
    /// </summary>
    internal RemoteAdminCommandEvent(
        CommandSender sender,
        Player? player,
        string command,
        string name,
        string[] args)
    {
        Sender = sender;
        Player = player ?? Server.Host;

        Command = command;
        Name = name;
        Args = args;

        // Defaults
        Prefix = string.Empty;
        Success = true;
        Allowed = true;
    }

    #endregion

    #region Public API

    /// <summary>Remote-admin sender.</summary>
    public CommandSender Sender { get; }

    /// <summary>Associated player (host if <see langword="null" />).</summary>
    public Player Player { get; }

    /// <summary>Raw command string.</summary>
    public string Command { get; }

    /// <summary>Primary command identifier without arguments.</summary>
    public string Name { get; }

    /// <summary>Array of command arguments.</summary>
    public string[] Args { get; }

    /// <summary>
    ///     Text that will be returned to remote-admin.
    ///     When set, the <see cref="Prefix" /> is automatically filled if empty.
    /// </summary>
    public string Reply
    {
        get => _reply;
        set
        {
            // Automatically prefix the message with the calling assembly
            if (string.IsNullOrEmpty(Prefix))
                Prefix = Assembly.GetCallingAssembly().GetName().Name;

            _reply = value;
        }
    }

    /// <summary>Prefix prepended to <see cref="Reply" />.</summary>
    public string Prefix { get; set; }

    /// <summary>Whether the command executed successfully from the plugin's perspective.</summary>
    public bool Success { get; set; }

    /// <summary>Whether the game should continue processing the command.</summary>
    public bool Allowed { get; set; }

    /// <inheritdoc />
    public uint EventId { get; } = EventID;

    #endregion
}

/// <summary>
///     Wrapper event for commands executed through the in-game game-console (<c>~</c>).
/// </summary>
[PublicAPI]
public sealed class GameConsoleCommandEvent : IBaseEvent
{
    #region Constants

    private const uint EventID = ServerEvents.GameConsoleCommand;

    #endregion

    #region Private fields

    private readonly CommandExecutingEventArgs _ev;

    #endregion

    #region Constructors

    /// <summary>
    ///     Creates an instance of <see cref="GameConsoleCommandEvent" />.
    /// </summary>
    internal GameConsoleCommandEvent(
        CommandExecutingEventArgs ev)
    {
        _ev = ev;
    }

    #endregion

    #region Proxies / Public API

    /// <summary>Console sender.</summary>
    public CommandSender Sender => _ev.Sender;

    /// <summary>Whether the command was found by the game.</summary>
    public bool CommandFound => _ev.CommandFound;

    /// <summary>Resolved command instance.</summary>
    public ICommand Command
    {
        get => _ev.Command;
        set => _ev.Command = value;
    }

    /// <summary>Command arguments.</summary>
    public ArraySegment<string> Arguments
    {
        get => _ev.Arguments;
        set => _ev.Arguments = value;
    }

    /// <summary>Command name (first token before the arguments).</summary>
    public string CommandName => _ev.CommandName;

    /// <summary>Whether the command is allowed to execute.</summary>
    public bool Allowed
    {
        get => _ev.IsAllowed;
        set => _ev.IsAllowed = value;
    }

    /// <inheritdoc />
    public uint EventId { get; } = EventID;

    #endregion
}

/// <summary>
///     Event fired for commands typed directly into the physical server console.
/// </summary>
[PublicAPI]
public sealed class ServerConsoleCommandEvent : IBaseEvent
{
    #region Constants

    private const uint EventID = ServerEvents.ServerConsoleCommand;

    #endregion

    #region Constructors

    /// <summary>
    ///     Creates an instance of <see cref="ServerConsoleCommandEvent" />.
    /// </summary>
    internal ServerConsoleCommandEvent(
        string command,
        string name,
        string[] args)
    {
        Command = command;
        Name = name;
        Args = args;

        // Defaults
        Reply = string.Empty;
        Allowed = true;
    }

    #endregion

    #region Public API

    /// <summary>Raw command string including arguments.</summary>
    public string Command { get; }

    /// <summary>Command identifier (first token).</summary>
    public string Name { get; }

    /// <summary>Arguments passed to the command.</summary>
    public string[] Args { get; }

    /// <summary>Text returned to the server console.</summary>
    public string Reply { get; set; }

    /// <summary>Whether the command should be processed by the game.</summary>
    public bool Allowed { get; set; }

    /// <inheritdoc />
    public uint EventId { get; } = EventID;

    #endregion
}