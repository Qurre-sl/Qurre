﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.Internal.EventsManager
{
    static internal class List
    {
        static internal Dictionary<uint, List<MethodInfo>> CallMethods = new();
        static internal Dictionary<MethodInfo, object> ClassesOfNonStaticMethods = new();
    }
}