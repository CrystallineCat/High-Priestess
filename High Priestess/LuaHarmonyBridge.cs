using HarmonyLib;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HighPriestess
{
    public struct BridgeKey
    {
        public string assembly;
        public string type;
        public string method;
    }
    public class LuaHarmonyBridge
    {
        public static IDictionary<BridgeKey, IList<object>> bridges = new Dictionary<BridgeKey, IList<object>>();

        public static Assembly GetAssemblyByName(string name)
            => AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SingleOrDefault(assembly => assembly.GetName().Name == name);

        public static object CallIntoLua(MethodInfo originalMethod, object self, object[] args, object initial, bool isVoid)
        {
            var argTable = new Table(Main.script);
            var n = 0;

            foreach (var parameter in originalMethod.GetParameters())
            {
                argTable[parameter.Name] = args[n++];
            }

            var lookup = new BridgeKey
            {
                assembly = originalMethod.DeclaringType.Assembly.GetName().Name,
                type = originalMethod.DeclaringType.FullName,
                method = originalMethod.Name,
            };

            var callbacks = bridges[lookup];
            var current = initial;

            foreach (var callback in callbacks)
            {
                if (isVoid)
                {
                    Main.script.Call(callback, self, argTable, current);
                } else
                {
                    current = Main.script.Call(callback, self, argTable, current).ToObject();
                }
            }

            return current;
        }

        public static void DoDelegationByte(MethodInfo __originalMethod, object __instance, ref Byte __result, object[] __args)
            => __result = (Byte)CallIntoLua(__originalMethod, __instance, __args, __result, false);

        public static void DoDelegationSByte(MethodInfo __originalMethod, object __instance, ref SByte __result, object[] __args)
            => __result = (SByte)CallIntoLua(__originalMethod, __instance, __args, __result, false);

        public static void DoDelegationInt16(MethodInfo __originalMethod, object __instance, ref Int16 __result, object[] __args)
            => __result = (Int16)CallIntoLua(__originalMethod, __instance, __args, __result, false);

        public static void DoDelegationInt32(MethodInfo __originalMethod, object __instance, ref Int32 __result, object[] __args)
            => __result = (Int32)CallIntoLua(__originalMethod, __instance, __args, __result, false);

        public static void DoDelegationInt64(MethodInfo __originalMethod, object __instance, ref Int64 __result, object[] __args)
            => __result = (Int64)CallIntoLua(__originalMethod, __instance, __args, __result, false);

        public static void DoDelegatioUInt16(MethodInfo __originalMethod, object __instance, ref UInt16 __result, object[] __args)
            => __result = (UInt16)CallIntoLua(__originalMethod, __instance, __args, __result, false);

        public static void DoDelegationUInt32(MethodInfo __originalMethod, object __instance, ref UInt32 __result, object[] __args)
            => __result = (UInt32)CallIntoLua(__originalMethod, __instance, __args, __result, false);

        public static void DoDelegationUInt64(MethodInfo __originalMethod, object __instance, ref UInt64 __result, object[] __args)
            => __result = (UInt64)CallIntoLua(__originalMethod, __instance, __args, __result, false);

        public static void DoDelegationSingle(MethodInfo __originalMethod, object __instance, ref Single __result, object[] __args)
            => __result = (Single)CallIntoLua(__originalMethod, __instance, __args, __result, false);

        public static void DoDelegationDouble(MethodInfo __originalMethod, object __instance, ref Double __result, object[] __args)
            => __result = (Double)CallIntoLua(__originalMethod, __instance, __args, __result, false);

        public static void DoDelegationDecimal(MethodInfo __originalMethod, object __instance, ref Decimal __result, object[] __args)
            => __result = (Decimal)CallIntoLua(__originalMethod, __instance, __args, __result, false);

        public static void DoDelegationObject(MethodInfo __originalMethod, object __instance, ref object __result, object[] __args)
            => __result = CallIntoLua(__originalMethod, __instance, __args, __result, false);

        public static void DoDelegationVoid(MethodInfo __originalMethod, object __instance, object[] __args)
            => CallIntoLua(__originalMethod, __instance, __args, null, true);

        public static string DelegationMethodName(MethodInfo info)
        {
            var name = "DoDelegation" + info.ReturnType.Name;

            if (typeof(LuaHarmonyBridge).GetMethod(name) != null)
            {
                return name;
            }
            else
            {
                return "DoDelegationObject";
            }
        }

        public static void Patch(BridgeKey key, object callback)
        {
            var harmony = new Harmony(Main.mod.Info.Id);

            if (!bridges.ContainsKey(key))
            {
                var info =
                    GetAssemblyByName(key.assembly)
                        .GetType(key.type)
                        .GetMethod(key.method, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                var postfix = typeof(LuaHarmonyBridge).GetMethod(DelegationMethodName(info));

                harmony.Patch(info, postfix: new HarmonyMethod(postfix));

                bridges[key] = new List<object>();
            }

            bridges[key].Add(callback);
        }
    }
}
