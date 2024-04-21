using HarmonyLib;
using MoonSharp.Interpreter.Interop.BasicDescriptors;
using MoonSharp.Interpreter.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighPriestess
{
    [HarmonyPatch(typeof(DescriptorHelpers), "GetVisibilityFromAttributes")]
    class PatchVisibility
    {
        static void Postfix(ref bool? __result)
        {
            // Everything is visible to Lua.
            __result = new bool?(true);
        }
    }

    [HarmonyPatch(typeof(DispatchingUserDataDescriptor), "AddMemberTo")]
    class PatchOverloads
    {
        static Exception Finalizer()
        {
            // Some types fail a Descriptor being made due to
            //   ArgumentException: Multiple members named ... are being added to type ... and one or more of these members do not support overloads.
            // Let's just suppress that and make an incomplete Descriptor instead of failing completely.
            return null;
        }
    }
}
