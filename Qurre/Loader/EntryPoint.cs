using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PluginAPI;
using PluginAPI.Enums;
using PluginAPI.Events;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Loader;
using PluginAPI.Loader.Features;

namespace Qurre.Loader
{
    internal sealed class EntryPoint
    {
        public static EntryPoint Singleton { get; private set; }

        [PluginConfig]
        public MainConfig Config;

        [PluginEntryPoint("Qurre-Lite", "2.0.0", "Framework for SCP:SL servers with unique functions & api.", "Qurre Team")]
        private void Init()
        {
            Singleton = this;

        }
    }
}
