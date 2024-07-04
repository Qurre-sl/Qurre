namespace Qurre.API.Controllers.Structs
{
    static class BcComponent
    {
        static private global::Broadcast bc;
        static internal global::Broadcast Component
        {
            get
            {
                bc ??= Server.Host.GameObject.GetComponent<global::Broadcast>();

                return bc;
            }
        }

        static internal void Refresh()
        {
            bc = null;
        }
    }
}