using System.Collections.Generic;
using JetBrains.Annotations;
using MEC;

namespace Qurre.API.Controllers;

[PublicAPI]
public class MapBroadcast
{
    private readonly List<Broadcast> _list = [];
    private string _msg;

    public MapBroadcast(string message, ushort time, bool instant, bool adminBC)
    {
        _msg = message;
        Time = time;

        Start();

        if (adminBC)
            foreach (Player pl in Player.List)
            {
                if (!PermissionsHandler.IsPermitted(pl.Sender.Permissions, PlayerPermissions.AdminChat))
                    continue;

                Broadcast bc = pl.Client.Broadcast(message, time, instant);
                _list.Add(bc);
            }
        else
            foreach (Player pl in Player.List)
            {
                Broadcast bc = pl.Client.Broadcast(message, time, instant);
                _list.Add(bc);
            }
    }

    public ushort Time { get; }
    public bool Active { get; private set; }

    public string Message
    {
        get => _msg;
        set
        {
            if (value == _msg)
                return;

            _msg = value;
            foreach (Broadcast? bc in _list) bc.Message = value;
        }
    }

    public void Start()
    {
        if (Active)
            return;

        Active = true;

        Timing.CallDelayed(Time, End);
    }

    public void End()
    {
        if (!Active)
            return;

        Active = false;

        foreach (Broadcast? bc in _list)
            bc.End();
    }
}