<p>
   <a href="https://discord.gg/zGUqfJQebn" alt="Discord">
      <img src="https://discord.com/api/guilds/779412392651653130/embed.png" alt="Discord"/>
   </a>
   <a href="https://github.com/Qurre-sl/Qurre/releases" alt="Downloads">
      <img src="https://img.shields.io/github/downloads/Qurre-sl/Qurre/total?color=%2300b813&style=plastic" alt="Downloads"/>
   </a>
   <a href="https://github.com/Qurre-sl/Qurre/releases" alt="Release">
      <img src="https://img.shields.io/github/v/release/Qurre-sl/Qurre.svg?style=plastic" alt="Release"/>
   </a>
   <a href="https://www.nuget.org/packages/Qurre" alt="nuget downloads">
      <img src="https://img.shields.io/nuget/dt/Qurre?label=nuget%20downloads&style=plastic" alt="nuget downloads"/>
   </a>
   <a href="https://www.nuget.org/packages/Qurre" alt="nuget release">
      <img src="https://img.shields.io/nuget/vpre/Qurre?style=plastic" alt="nuget release"/>
   </a>
</p>
<a href="https://discord.gg/zGUqfJQebn" alt="Discord">
<img src="https://cdn.scpsl.store/qurre/img/qurreLogo200x200-cyrcle.png" align="right" style="border-radius: 50%;" />
</a>

# Qurre
<p align="center">
<img src="https://readme-typing-svg.herokuapp.com/?font=Fira+Code&pause=1000&color=3FF781&center=true&vCenter=true&width=435&lines=Simplicity.;Convenience.;Functionality.">
</p>
<p align="center">
Simple, convenient and functional plugin loader for SCP: Secret Laboratory
</p>

# Select language
### EN [RU](https://github.com/Qurre-sl/Qurre/blob/main/README-RU.md)

# Installation
### Manual installation
1. Download `Qurre.zip` file from [releases](https://github.com/Qurre-sl/Qurre/releases/latest)
2. Move `Qurre` folder into `%appdata%` (on Linux: `~/.config`)
3. Move `Assembly-CSharp.dll` into game folder: `SCPSL_Data/Managed` (with file replacement)
4. That's all, you have successfully installed **Qurre**
### Automatic installation
You can find the auto-installer on [the discord server](https://discord.gg/qXBZUCaDBR), or you can download the archive from [here](https://cdn.scpsl.store/qurre/modules/Qurre-Installer.zip)

# Documentation
> Coming soon

# Configs
> Configs are in **JSON** format and are located in `%appdata%/Qurre/Configs`

> You can see the contents of the config after installing **Qurre**

# Plugin Examples
> *Coming soon

#### Small plugin example (to be removed after normal examples appear)
> What is important to know:
> 1. Method [PluginEnable] & [PluginDisable] must be static
> 2. The [EventMethod] event method can be non-static, but for better performance, it is better to use a static method
```cs
using Qurre.API;
using Qurre.API.Attributes;
using Qurre.Events;
using Qurre.Events.Structs;

[PluginInit("MyPlugin", "Qurre Team", "1.0.0")]
static class Plugin
{
    [PluginEnable] // Similarly, you can use [PluginDisable]
    static internal void Enabled()
    {
        Log.Info("Plugin Enabled");
    }
    
    [EventMethod(PlayerEvents.Join)]
    static internal void Join(JoinEvent ev)
    {
        Log.Info($"Player {ev.Player?.UserInfomation.Nickname} joined");
    }
    
    [EventMethod(PlayerEvents.PickupArmor)]
    [EventMethod(PlayerEvents.PickupItem)]
    static internal void TestMultiple(IBaseEvent ev)
    {
        if (ev is PickupArmorEvent ev1)
        {
            Log.Info($"Armor; Pl: {ev1.Player?.UserInfomation.Nickname}; Item: {ev1.Pickup?.Serial}");
            //ev1.Allowed = false;
        }
        else if (ev is PickupItemEvent ev2)
        {
            Log.Info($"Item; Pl: {ev2.Player?.UserInfomation.Nickname}; Item: {ev2.Pickup?.Serial}");
            //ev2.Allowed = false;
        }
    }
}
```

# Credits
**0Harmony - Owned by [Andreas Pardeike](https://github.com/pardeike)**

**Newtonsoft.Json - Owned by [James Newton-King](https://github.com/JamesNK)**

**Assembly-CSharp - Owned by [Northwood Studios](https://github.com/northwood-studios)**

**Because we are writing code for one game, it can be similar to the code of other plugin loaders {[Exiled](https://github.com/Exiled-Team/EXILED) or [Synapse](https://github.com/SynapseSL/Synapse)} (Always can carp to something somehow)**