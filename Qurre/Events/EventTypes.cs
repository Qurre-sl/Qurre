namespace Qurre.Events
{
    internal enum PlayerEvents : int
    {
        //100+ <- Network events
        Joined = 101,
        Left = 102,

        //110+ <- Health events
        Death = 111,

        //120+ <- Admins with Player
        Banned = 121,

    }

    internal enum MapEvents : int
    {

    }
}