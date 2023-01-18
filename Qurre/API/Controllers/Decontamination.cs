using LightContainmentZoneDecontamination;

namespace Qurre.API.Controllers
{
    public static class Decontamination
    {
        public static DecontaminationController Controller => DecontaminationController.Singleton;

        public static DecontaminationController.DecontaminationStatus Status
        {
            get => Controller.NetworkDecontaminationOverride;
            set => Controller.NetworkDecontaminationOverride = value;
        }

        public static bool Locked
        {
            get => Controller._stopUpdating;
            set => Controller._stopUpdating = value;
        }

        public static bool Begun => Controller.IsDecontaminating;
        public static bool InProgress => Controller._decontaminationBegun;

        public static void InstantStart() => Controller.FinishDecontamination();
    }
}