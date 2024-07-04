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
</p>
<a href="https://discord.gg/zGUqfJQebn" alt="Discord">
<img src="https://cdn.fydne.dev/qurre/img/qurreLogo200x200-cyrcle.png" align="right" style="border-radius: 50%;" />
</a>

# Qurre
<p align="center">
<img src="https://readme-typing-svg.herokuapp.com?font=Fira+Code&pause=1000&color=3FF781&center=true&vCenter=true&width=435&lines=Простой.;Удобный.;Функциональный.">
</p>
<p align="center">
Простой, удобный и функциональный загрузчик плагинов для SCP: Secret Laboratory
</p>

# Выберите язык README
### [EN](https://github.com/Qurre-sl/Qurre) RU

# Установка
1. Скачайте файл `Qurre.tar.gz` из [релизов](https://github.com/Qurre-sl/Qurre/releases/latest)
2. Поместите папку `Qurre` в `%appdata%` (На линукс: `~/.config`)
3. Поместите `Assembly-CSharp.dll` в папку с игрой: `SCPSL_Data/Managed` (с заменой файлов)
4. На этом все, вы успешно установили **Qurre**

# Документация
> Появится в скором времени

# Конфиги
> Конфиги имеют формат **JSON**, и находятся в `%appdata%/Qurre/Configs`

> Содержание конфига вы можете посмотреть после установки **Qurre**

# Примеры плагинов
> *Появятся в скоро времени

#### Пример небольшого плагина (будет удален после появления нормальных примеров)
> Что важно знать:
> 1. Метод [PluginEnable] & [PluginDisable] должны быть статическими
> 2. Метод ивента [EventMethod] может быть нестатическим, но для большей производительности, лучше использовать статический метод
```cs
using Qurre.API;
using Qurre.API.Attributes;
using Qurre.Events;
using Qurre.Events.Structs;

[PluginInit("MyPlugin", "Qurre Team", "1.0.0")]
static class Plugin
{
    [PluginEnable] // Аналогично можно подключить [PluginDisable]
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