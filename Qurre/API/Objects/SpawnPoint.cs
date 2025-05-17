using JetBrains.Annotations;
using UnityEngine;

namespace Qurre.API.Objects;

[PublicAPI]
public readonly struct SpawnPoint
{
    public Vector3 Position { get; }
    public float Horizontal { get; }

    internal SpawnPoint(Vector3 position, float horizontal)
    {
        Position = position;
        Horizontal = horizontal;
    }
}