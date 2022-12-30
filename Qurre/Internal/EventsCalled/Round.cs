using Qurre.API;
using Qurre.API.Attributes;
using Qurre.API.Controllers;
using Qurre.Events;

namespace Qurre.Internal.EventsCalled
{
    static class Round
    {
        [EventMethod(RoundEvents.Waiting)]
        static internal void Waiting()
        {
            Server.host = null;
            Server.hinv = null;

            API.Extensions.DamagesCached.Clear();
            Patches.Player.Admins.Banned.Cached.Clear();

            if (API.Round.CurrentRound == 0)
                API.Addons.Prefabs.InitLate();

            API.Round.CurrentRound++;

            RoundSummary.RoundLock = false;

            MapRoundInit();
        }

        static void MapRoundInit()
        {
            MapClearLists();

            Map.AmbientSoundPlayer = Server.Host.GameObject.GetComponent<AmbientSoundPlayer>();

            foreach (var tesla in Server.GetObjectsOf<TeslaGate>()) Map.Teslas.Add(new Tesla(tesla));
        }

        static void MapClearLists()
        {
            Map.Cassies.Clear();

            Map.Lights.Clear();
            Map.Primitives.Clear();
            Map.ShootingTargets.Clear();
            Map.WorkStations.Clear();

            Map.Doors.Clear();
            Map.Generators.Clear();
            Map.Lockers.Clear();
            Map.Ragdolls.Clear();
            Map.Sinkholes.Clear();
            Map.Teslas.Clear();
            Map.Windows.Clear();
        }
    }
}