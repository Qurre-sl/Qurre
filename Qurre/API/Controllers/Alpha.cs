using Mirror;
using UnityEngine;

namespace Qurre.API.Controllers
{
    static public class Alpha
    {
        static public AlphaWarheadController Controller => AlphaWarheadController.Singleton;
        static public AlphaWarheadNukesitePanel InnerPanel => Object.FindObjectOfType<AlphaWarheadNukesitePanel>();
        static public AlphaWarheadOutsitePanel OutsidePanel => Object.FindObjectOfType<AlphaWarheadOutsitePanel>();
        static public GameObject InnerPanelLever => InnerPanel.lever.gameObject;
        static public bool InnerPanelEnabled
        {
            get => InnerPanel.Networkenabled;
            set => InnerPanel.Networkenabled = value;
        }
        static public bool OutsidePanelEnabled
        {
            get => OutsidePanel.keycardEntered;
            set => OutsidePanel.NetworkkeycardEntered = value;
        }

        static public bool Detonated => Controller._alreadyDetonated;
        static public bool Active => Controller.Info.InProgress;
        static public float TimeToDetonation
        {
            get => AlphaWarheadController.TimeUntilDetonation;
            set
            {
                Controller.Info.StartTime = NetworkTime.time;
                Controller.ForceTime(value);
            }
        }
        static public bool Locked
        {
            get => Controller.IsLocked;
            set => Controller.IsLocked = value;
        }
        static public int Cooldown
        {
            get => (int)Controller.CooldownEndTime;
            set => Controller.NetworkCooldownEndTime = value;
        }

        static public void Start(bool isAutomatic = false, bool suppressSubtitles = false)
        {
            Controller.InstantPrepare();
            Controller.StartDetonation(isAutomatic, suppressSubtitles);
        }
        static public void InstantPrepare() => Controller.InstantPrepare();
        static public void CancelDetonation() => Controller.CancelDetonation();
        static public void Stop() => Controller.CancelDetonation();
        static public void Detonate() => Controller.Detonate();
        static public void Shake() => Controller.RpcShake(false);
    }
}