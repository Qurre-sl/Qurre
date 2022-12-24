using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public static class Alpha
    {
        public static AlphaWarheadController Controller { get; } = AlphaWarheadController.Singleton;
        public static AlphaWarheadNukesitePanel InnerPanel => UnityEngine.Object.FindObjectOfType<AlphaWarheadNukesitePanel>();
        public static AlphaWarheadOutsitePanel OutsidePanel => UnityEngine.Object.FindObjectOfType<AlphaWarheadOutsitePanel>();
        public static GameObject InnerPanelever => InnerPanel.lever.gameObject;
        public static bool InnerPaneleverStatus
        {
            get => InnerPanel.Networkenabled;
            set
            {
                InnerPanel.Networkenabled = value;
            }
        }
        public static bool Enabled
        {
            get => OutsidePanel.keycardEntered;
            set => OutsidePanel.keycardEntered = value;
        }
        public static void Start(bool isAutomatic = true, bool suppressSubtitles = false)
        {
            Controller.InstantPrepare();
            Controller.StartDetonation(isAutomatic, suppressSubtitles);
        }
        public static void InstantPrepare() => Controller.InstantPrepare();
        public static void CancelDetonation() => Controller.CancelDetonation();
        public static void Stop() => Controller.CancelDetonation();
        public static void Detonate() => Controller.Detonate();
        public static void Shake() => Controller.RpcShake(false);
        public static bool Detonated { get; } = Controller._alreadyDetonated;
        public static bool Active { get; } = Controller.Info.InProgress;
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
    }
}
