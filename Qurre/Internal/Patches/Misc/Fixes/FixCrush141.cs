using System.Runtime.CompilerServices;
using HarmonyLib;
using InventorySystem.Items.Armor;
using JetBrains.Annotations;
using PlayerRoles.FirstPersonControl;
using RemoteAdmin;
using UnityEngine;

namespace Qurre.Internal.Patches.Misc.Fixes;

[HarmonyPatch]
[PublicAPI]
internal static class FixCrush141
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(FirstPersonMovementModule), nameof(FirstPersonMovementModule.UpdateMovement))]
    private static bool UpdateMovement(FirstPersonMovementModule __instance)
    {
        return __instance is not null &&
               __instance.Motor != null && __instance.CharControllerSet && __instance.CharController != null;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(FpcMotor), nameof(FpcMotor.UpdatePosition))]
    private static bool UpdatePosition(FpcMotor __instance)
    {
        return __instance is not null && __instance.MainModule != null && __instance.MainModule.CharControllerSet &&
               __instance.MainModule.CharController != null;
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(QueryProcessor), nameof(QueryProcessor.OnDestroy))]
    private static bool BodyArmorUpdate(QueryProcessor __instance)
    {
        return __instance._sender is { OutputId: not null };
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(BodyArmorPickup), nameof(BodyArmorPickup.Update))]
    private static bool BodyArmorUpdate(BodyArmorPickup __instance)
    {
        try
        {
            if (!__instance.IsAffected || Mathf.Abs(__instance._rb.linearVelocity.y) > 0.10000000149011612)
                return false;
            __instance._remainingReleaseTime -= Time.deltaTime;
            if (__instance._remainingReleaseTime > 0.0)
                return false;
            __instance._released = true;
            __instance._rb.constraints = RigidbodyConstraints.None;
        }
        catch
        {
        }

        return false;
    }


    /// <summary>
    ///     Полностью заменяем оригинальный метод. <br />
    ///     • Если <c>gameObject</c> ещё жив – возвращаем его HashCode (оригинальное поведение).<br />
    ///     • Если объект уже уничтожен – используем <c>GetInstanceID()</c> (Unity выдаёт
    ///     уникальный int даже для “псевдо-null” объектов).<br />
    ///     • На крайний случай (теоретически невозможно, но на всякий случай) берём
    ///     «идентичный» хэш через <c>RuntimeHelpers.GetHashCode</c>.
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.GetHashCode))]
    private static bool GetOrAddPatch(ReferenceHub __instance, ref int __result)
    {
        // 1) ReferenceHub сам по себе может оказаться псевдо-null
        if (!__instance)
        {
            __result = 0;
            return false; // пропускаем оригинал
        }

        try
        {
            // 2) Нормальный путь — живой GameObject
            GameObject go = __instance.gameObject;
            if (go) // оператор "bool" у UnityEngine.Object
            {
                __result = go.GetHashCode();
                return false;
            }

            // 3) GameObject уже уничтожен → fallback
            __result = __instance.GetInstanceID();
            return false;
        }
        catch
        {
            // 4) Абсолютный запасной вариант (не должен понадобиться)
            __result = RuntimeHelpers.GetHashCode(__instance);
            return false;
        }
    }
}