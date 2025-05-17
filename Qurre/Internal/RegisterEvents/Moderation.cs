// using LabApi.Events.Handlers;
// using LabApi.Features.Enums;
// using Qurre.Events.Structs;
// using Qurre.Internal.Attributes;
// using Qurre.Internal.EventsManager;
// using LabEvents = LabApi.Events.Arguments.ServerEvents;
//
// namespace Qurre.Internal.RegisterEvents;
//
// internal static class Moderation
// {
//     [SelfInvoke]
//     internal static void Init()
//     {
//         ServerEvents.CommandExecuting += OnGameConsoleCommand;
//     }
//
//     private static void OnGameConsoleCommand(LabEvents.CommandExecutingEventArgs ev)
//     {
//         if (ev.CommandType is not CommandType.Console)
//             return;
//
//         new GameConsoleCommandEvent(ev).InvokeEvent();
//     }
// }

/* TODO: IMPORTANT

  public static event LabEventHandler<BanIssuingEventArgs>? BanIssuing;
  public static event LabEventHandler<BanIssuedEventArgs>? BanIssued;
  public static event LabEventHandler<BanRevokingEventArgs>? BanRevoking;
  public static event LabEventHandler<BanRevokedEventArgs>? BanRevoked;
  public static event LabEventHandler<BanUpdatingEventArgs>? BanUpdating;
  public static event LabEventHandler<BanUpdatedEventArgs>? BanUpdated;

  сделать эти ивенты, в папке "Administrations"
*/