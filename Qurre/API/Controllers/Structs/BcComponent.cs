namespace Qurre.API.Controllers.Structs;

internal static class BcComponent
{
    private static global::Broadcast? _bc;

    internal static global::Broadcast Component
    {
        get
        {
            _bc ??= Server.Host.GameObject.GetComponent<global::Broadcast>();

            return _bc;
        }
    }

    internal static void Refresh()
    {
        _bc = null;
    }
}