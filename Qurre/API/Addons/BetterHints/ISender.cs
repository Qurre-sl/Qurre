using System;
using static Qurre.API.Addons.BetterHints.Manager;

namespace Qurre.API.Addons.BetterHints
{
    public interface ISender
    {
        void Hint(Player pl, HintStruct hs);

        Guid InjectAction(Player pl, InjectAct<string, string, bool, Player> act);
        bool UnjectAction(Player pl, Guid uid);
        bool ContainsAction(Player pl, Guid uid);

        Guid InjectGlobalAction(InjectAct<string, string, bool, Player> act);
        bool UnjectGlobalAction(Guid uid);
        bool ContainsGlobalAction(Guid uid);
    }
}