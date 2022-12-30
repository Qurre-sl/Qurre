using LightContainmentZoneDecontamination;

namespace Qurre.API.Controllers
{
    static public class Decontamination
    {
        static public DecontaminationController Controller => DecontaminationController.Singleton;

        static public DecontaminationController.DecontaminationStatus Status
        {
            get => Controller.NetworkDecontaminationOverride;
            set => Controller.NetworkDecontaminationOverride = value;
        }
        static public bool Locked
        {
            get => Controller._stopUpdating;
            set => Controller._stopUpdating = value;
        }

        static public bool Begun => Controller.IsDecontaminating;
        static public bool InProgress => Controller._decontaminationBegun;
        static public void InstantStart() => Controller.FinishDecontamination();
    }
}