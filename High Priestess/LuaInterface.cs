using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighPriestess
{
    public struct ModInfo
    {
        public string id;
        public string name;
        public string author;
        public string version;
    }

    public class LuaInterface
    {
        public static ModInfo mod;

        public static Type TypeOf(string assembly, string name)
            => LuaHarmonyBridge.GetAssemblyByName(assembly).GetType(name);

        public static void Patch(string assembly, string type, string method, object callback)
            => LuaHarmonyBridge.Patch(new BridgeKey
            {
                assembly = assembly,
                type = type,
                method = method,
            }, callback);
    }
}
