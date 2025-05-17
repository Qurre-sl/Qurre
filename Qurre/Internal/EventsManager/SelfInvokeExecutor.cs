using System;
using System.Reflection;
using Qurre.Internal.Attributes;

namespace Qurre.Internal.EventsManager;

/// <summary>
///     Выполняет все статические методы, помеченные атрибутом <see cref="SelfInvoke" />.
/// </summary>
internal static class SelfInvokeExecutor
{
    private static bool _initialized;

    /// <summary>
    ///     Вызывает все методы с атрибутом <see cref="SelfInvoke" /> внутри текущей сборки.
    ///     Повторный вызов метода ничего не делает.
    /// </summary>
    internal static void InvokeAll()
    {
        if (_initialized)
            return;

        _initialized = true;

        Assembly assembly = Assembly.GetExecutingAssembly();
        const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        foreach (Type type in assembly.GetTypes())
            foreach (MethodInfo method in type.GetMethods(flags))
            {
                // Проверяем наличие нужного атрибута
                if (!method.IsDefined(typeof(SelfInvoke), false))
                    continue;

                // Поддерживаются только методы без параметров
                if (method.GetParameters().Length != 0)
                    continue;

                try
                {
                    method.Invoke(null, null);
                }
                catch (Exception ex)
                {
                    // Логируем, но не прерываем выполнение остальных методов
#if DEBUG
                    API.Log.Custom($"Ошибка при вызове {type.FullName}.{method.Name}: {ex}", "SelfInvoke");
#endif
                }
            }
    }
}