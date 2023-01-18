namespace Qurre.API.Addons.BetterHints
{
    public static class Manager
    {
        public delegate void InjectAct<T1, T2, T3, T4>(T1 curHint, out T2 addString, out T3 autoFormate, T4 player);

        public static ISender Sender { get; internal set; }
    }
}