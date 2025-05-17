using System;
using JetBrains.Annotations;

namespace Qurre.API.Attributes;

[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class EventsIgnore : Attribute
{
}