using Qurre.API;
using Qurre.API.Attributes;
using Qurre.Events;
using Qurre.Events.Structs;
using RoundRestarting;

namespace Qurre.Internal.EventsCalled
{
    static class SpawnCrashFix
    {
        //static GameObject PositionObject;
        //static Room ClassDRoom;

        [EventMethod(PlayerEvents.Spawn)]
        static internal void CrashNotify(SpawnEvent ev)
        {
            if (ev.Role != PlayerRoles.RoleTypeId.ClassD)
                return;

            /*
            if (PositionObject is null)
                PositionObject = new();

            if (ClassDRoom is null)
                ClassDRoom = Map.Rooms.FirstOrDefault(x => x.Type == API.Objects.RoomType.LczClassDSpawn);

            Log.Info(ClassDRoom);

            Log.Info(ev.Position);
            PositionObject.transform.position = ev.Position;
            PositionObject.transform.parent = ClassDRoom.Transform;
            Log.Info(PositionObject.transform.position);
            Log.Info(PositionObject.transform.localPosition);

            if (Vector3.Distance(PositionObject.transform.localPosition, new(-13.7f, 0.9f, 4.2f)) > 1
                && Vector3.Distance(PositionObject.transform.localPosition, new(1.3f, 0.9f, -4.2f)) > 1)
                return;
            */

            // idk why exactly global 76 & 91
            if (ev.Position.x.Difference(76) > 1 &&
                ev.Position.x.Difference(91) > 1)
                return;

            ev.Player.Client.Broadcast("<color=red>Crash Detected\nReconnecting..</color>", 15, true);
            ev.Player.Client.SendConsole("Crash Detected. Reconnecting...", "red");

            ev.Player.Connection.Send(new RoundRestartMessage(RoundRestartType.RedirectRestart, 1, Server.Port, true, false));
        }
    }
}