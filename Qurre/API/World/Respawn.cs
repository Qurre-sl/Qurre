using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Respawning;
using Respawning.Waves;
using Respawning.Waves.Generic;

namespace Qurre.API.World;

[PublicAPI]
public static class Respawn
{
    static Respawn()
    {
        UpdateCachedWaves();
    }

    public static WaveQueueState QueueState
    {
        get => WaveManager.State;
        set => WaveManager.State = value;
    }

    public static List<SpawnableWaveBase> SpawnableWaveBases => WaveManager.Waves;

    public static TimeBasedWave[] TimeBasedWaves { get; private set; } = [];

    public static IEnumerable<TimeBasedWave> TimeLeftSortedWaves
        => TimeBasedWaves.OrderBy(wave => wave.Timer.TimeLeft);

    /// <summary>
    ///     Предсказывает ближайшую волну.
    ///     NULL, если TimeBasedWaves не содержит элементов, квота respawn-ов и поле RespawnTokens которых больше 0.
    /// </summary>
    public static TimeBasedWave? PredictedNextWave
        => TimeLeftSortedWaves.FirstOrDefault(wave =>
            !wave.Timer.IsPaused && wave is ILimitedWave { RespawnTokens: > 0 });

    public static NtfSpawnWave? NtfSpawnWave { get; private set; }
    public static ChaosSpawnWave? ChaosSpawnWave { get; private set; }
    public static NtfMiniWave? NtfMiniWave { get; private set; }
    public static ChaosMiniWave? ChaosMiniWave { get; private set; }

    public static int? NtfTokens
    {
        get => NtfSpawnWave?.RespawnTokens;
        set
        {
            if (NtfSpawnWave == null)
                throw new NullReferenceException("NtfSpawnWave is null");
            NtfSpawnWave.RespawnTokens = value ?? 0;
        }
    }

    public static int? ChaosTokens
    {
        get => ChaosSpawnWave?.RespawnTokens;
        set
        {
            if (ChaosSpawnWave == null)
                throw new NullReferenceException("ChaosSpawnWave is null");
            ChaosSpawnWave.RespawnTokens = value ?? 0;
        }
    }

    public static void UpdateCachedWaves()
    {
        TimeBasedWaves = SpawnableWaveBases.OfType<TimeBasedWave>().ToArray();
        NtfSpawnWave = SpawnableWaveBases.OfType<NtfSpawnWave>().FirstOrDefault();
        ChaosSpawnWave = SpawnableWaveBases.OfType<ChaosSpawnWave>().FirstOrDefault();
        NtfMiniWave = SpawnableWaveBases.OfType<NtfMiniWave>().FirstOrDefault();
        ChaosMiniWave = SpawnableWaveBases.OfType<ChaosMiniWave>().FirstOrDefault();
    }

    public static void Spawn(SpawnableWaveBase spawnableWaveBase, bool forceSpawn = false)
    {
        if (forceSpawn)
            WaveManager.Spawn(spawnableWaveBase);
        else
            WaveManager.InitiateRespawn(spawnableWaveBase);
    }

    public static void InvokeAnimation<TAnimatedWave>(TAnimatedWave spawnableWaveBase)
        where TAnimatedWave : SpawnableWaveBase, IAnimatedWave
    {
        WaveUpdateMessage.ServerSendUpdate(spawnableWaveBase, UpdateMessageFlags.Trigger);
    }

    public static void CallChaosCar()
    {
        if (ChaosSpawnWave != null)
            InvokeAnimation(ChaosSpawnWave);
    }

    public static void CallMtfHelicopter()
    {
        if (NtfSpawnWave != null)
            InvokeAnimation(NtfSpawnWave);
    }
}