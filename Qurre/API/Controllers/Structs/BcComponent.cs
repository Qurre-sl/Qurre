namespace Qurre.API.Controllers.Structs
{
    internal static class BcComponent
    {
        private static global::Broadcast bc;

        internal static global::Broadcast Component
        {
            get
            {
                if (bc is null)
                {
                    bc = Server.Host.GameObject.GetComponent<global::Broadcast>();
                }

                return bc;
            }
        }
    }
}