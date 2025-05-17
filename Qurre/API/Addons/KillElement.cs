using System;
using JetBrains.Annotations;
using Qurre.API.Controllers;
using Qurre.API.Objects;

namespace Qurre.API.Addons;

[PublicAPI]
public readonly struct KillElement : IEquatable<KillElement>
{
    internal KillElement(Player killer, Player target, DamageTypes type, DateTime offset)
    {
        Killer = new CachedPlayer(killer);
        Target = new CachedPlayer(target);
        Type = type;
        Time = offset;
    }

    public CachedPlayer Killer { get; }
    public CachedPlayer Target { get; }
    public DamageTypes Type { get; }
    public DateTime Time { get; }

    public override bool Equals(object? obj)
    {
        return obj is KillElement other && Equals(other);
    }

    public bool Equals(KillElement obj)
    {
        return this == obj;
    }

    public override int GetHashCode()
    {
        return Tuple.Create(Killer, Target, Type, Time).GetHashCode();
    }

    public override string ToString()
    {
        return $"({Target} killed by {Killer} with {Type} at {Time})";
    }

    public static bool operator ==(KillElement a, KillElement b)
    {
        return a.Killer == b.Killer && a.Target == b.Target && a.Time == b.Time && a.Type == b.Type;
    }

    public static bool operator !=(KillElement a, KillElement b)
    {
        return !(a == b);
    }

    public static bool operator >(KillElement a, KillElement b)
    {
        return a.Time > b.Time;
    }

    public static bool operator <(KillElement a, KillElement b)
    {
        return a.Time < b.Time;
    }
}