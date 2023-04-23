using Qurre.API;
using Qurre.API.Addons;
using System;
using System.Linq;

namespace Qurre.Loader
{
    internal class EntryPoint : ICharacterLoader
    {
        public void Init()
        {
            if (StartupArgs.Args.Any(arg => string.Equals(arg, "-disableAnsiColors", StringComparison.OrdinalIgnoreCase)))
                BetterColors.Enabled = false;

            Log.Info("Initializing Qurre...");

            try
            {
                Configs.Setup();

                CustomNetworkManager.Modded = true;

                Internal.EventsManager.Loader.PathQurreEvents();

                Plugins.Init();

                Prefabs.Init();

                Log.Custom(BetterColors.Bold($"Qurre {BetterColors.BrightRed($"v{Core.Version}")} enabled"), "Loader", ConsoleColor.Red);
            }
            catch(Exception e)
            {
                ServerConsole.AddLog(e.ToString(), ConsoleColor.Red);
            }


            if (!Configs.PrintLogo || Log.Errored) return;

            MEC.Timing.CallDelayed(0.5f, () => ServerConsole.AddLog(BetterColors.Hidden("⠀") + @"
                                                                                
                                                                                
           .:^:^:..:.                                         ..                
          .^^^^^^:^!~~^:.                              .::..:^^^^~^.            
         :~.:^^^~:.^:^^!7^.                          ^~~~^~!^^~^^^.~.           
         :::^^::::^:!~^7~:!~:                      ^^!!~~~:^^^^:^^:^^           
        .~..:^^~!~^^^^!!^!??7!~:                 .~!!~!7^!~^~~^:^^::::          
        ^^..::.^7^~:^:~~??7^.::~               .^!!~^~~?7^:~~?!^::..^^          
        ~^.^!~^....:!!!!~: :^.:~             .:~^^^~^..^~~~::^^:::~.:!.         
        ~^~7!:::^:: ~7: .^^~77~~7!77!~: .~~^^!^~~^~~~^^:.:~~^!?~.^^.:!.         
        .:~J!:    ::7?^ ..^!?7!~?JJJ?JJ?~???J7!~~~~~^~~^::!!^^^..:~~:^          
         :~!^:.:..  :~~.::::!??????J?!J?!J7!JJJ?7~~~^:^^^.^.  ..:^~~::          
         :!^^:^^:::      .:^~7??JJJJJ!!?7J77?JJ?7~^^::...  :..:..:~~~.          
         .~:^~!^^^!       ~???????JJJ7?7!J?J??JJ?~7!^      ^^::~~~^!^           
          :!^^^^.~!:     .???JJ?JJJJJ?7?!!JJJ7J?7?J??.     ~7^:~~!~!.           
   :^::::..^^::^:^?~      :..:7J?JJJ?7?J!777JJJJJ!^:^:    .?7^^^~7?^   ....     
 .^^^^^^^^^^^^^^^.!!:     ..  .!7?JJ7??7~?J7!JJ?7.        !?!:^:^!!:::^^^^^:.   
 .^^::::^^^^^~~~~.:~~^    ~^  .77777!??!.?JJ!7~~~. .~~   ^!~^^^~~~^^:^^^^^:^:   
 .^^:::::^^^^^^^~~:::~^   !7^:^7!?J?!?J!.!JJ7~:~!..~!:  ^^:::^^~^^~~~^^^::^~:   
  ^!!: .::.^~~~^^^~~^:~.  .~~^^!^.7?!?J~^7?7!:!!^:^^.  :^:^^~~^^^^^^:::::^^~:   
  .^~.    ..:~?7^~~~~~:~:      :~~^~!J?.!J7!~:^^      ..^!!~^^~7!~:...   ~~^.   
   .~:        :~~!!!^^!!!.      ^!~!~J?:7?!~!!::     :~~~^^~7~!7^.      .~:.    
     .          ^~!!?77~^^.     ^7!?^7?^^?7~!?!:    :~~!~~7!~^^.        ^:      
                 :!~~77~:::     ^777^~?~:??!7^~.   .^.^7??!!~:         ..       
                   :^^^^:..     .!^~~:7!^7:!^ :    .::^~~^^:.                   
                                 :::! :.!! !^:       ...:..                     
                                   :!   .  !^.                                  
                                   ^7     .!:                                   
                                   ^7  ....7^                                   
                                   :7:.^!.:7.                                   
                                    ^~:7?^^^                                    
                                     :^^:^:                                     
                                                                                
" + BetterColors.Hidden("⠀"), ConsoleColor.Red));

            ServerConsole.AddLog(BetterColors.Bold("Bold"));
            ServerConsole.AddLog(BetterColors.Dim("Dim"));
            ServerConsole.AddLog(BetterColors.Italic("Italic"));
            ServerConsole.AddLog(BetterColors.Underline("Underline"));
            ServerConsole.AddLog(BetterColors.Inverse("Inverse"));
            ServerConsole.AddLog(BetterColors.Hidden("Hidden"));
            ServerConsole.AddLog(BetterColors.Strikethrough("Strikethrough"));

            ServerConsole.AddLog(BetterColors.BgCyan("BgCyan"));
        }
    }
}