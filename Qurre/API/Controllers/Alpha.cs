using Mirror;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public static class Alpha
    {
        public static AlphaWarheadController Controller => AlphaWarheadController.Singleton;
        public static AlphaWarheadNukesitePanel InnerPanel => Object.FindObjectOfType<AlphaWarheadNukesitePanel>();
        public static AlphaWarheadOutsitePanel OutsidePanel => Object.FindObjectOfType<AlphaWarheadOutsitePanel>();
        public static GameObject InnerPanelLever => InnerPanel.lever.gameObject;

        public static bool InnerPanelEnabled
        {
            get => InnerPanel.Networkenabled;
            set => InnerPanel.Networkenabled = value;
        }

        public static bool OutsidePanelEnabled
        {
            get => OutsidePanel.keycardEntered;
            set => OutsidePanel.NetworkkeycardEntered = value;
        }

        public static bool Detonated => Controller._alreadyDetonated;
        public static bool Active => Controller.Info.InProgress;

        public static float TimeToDetonation
        {
            get => AlphaWarheadController.TimeUntilDetonation;
            set
            {
                Controller.Info.StartTime = NetworkTime.time;
                Controller.ForceTime(value);
            }
        }

        public static bool Locked
        {
            get => Controller.IsLocked;
            set => Controller.IsLocked = value;
        }

        public static int Cooldown
        {
            get => (int)Controller.CooldownEndTime;
            set => Controller.NetworkCooldownEndTime = value;
        }

        public static void Start(bool isAutomatic = false, bool suppressSubtitles = false)
        {
            Controller.InstantPrepare();
            Controller.StartDetonation(isAutomatic, suppressSubtitles);
        }

        public static void InstantPrepare() => Controller.InstantPrepare();

        public static void CancelDetonation() => Controller.CancelDetonation();

        public static void Stop() => Controller.CancelDetonation();

        public static void Detonate() => Controller.Detonate();

        public static void Shake() => Controller.RpcShake(false);
    }
}