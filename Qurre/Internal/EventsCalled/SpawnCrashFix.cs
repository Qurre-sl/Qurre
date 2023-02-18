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

        //[EventMethod(PlayerEvents.Spawn)]
        static internal void CrashNotify(SpawnEvent ev)
        {
            if (ev.Role != PlayerRoles.RoleTypeId.ClassD)
                return;

            Log.Debug($"Class D spawn position: x: {ev.Position.x}; y: {ev.Position.y}; z: {ev.Position.z}; Player: {ev.Player.UserInfomation.Nickname}");

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

            // idk why exactly global 76 & 91 & 31 [31 appeared after the public release of this fix]
            if (ev.Position.x.Difference(76) > 1 &&
                ev.Position.x.Difference(91) > 1 &&
                ev.Position.x.Difference(31) > 1)
                return;

            ev.Player.Client.Broadcast("<color=red>>> Crash Detected <<\n<color=black>..</color>Reconnecting..</color>", 15, true);
            ev.Player.Client.SendConsole("Crash Detected. Reconnecting...", "red");

            ev.Player.Connection.Send(new RoundRestartMessage(RoundRestartType.RedirectRestart, 1, Server.Port, true, false));
        }
    }
}