using CPrompt;
using HarmonyLib;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using MoonSharp.Interpreter.Loaders;
using MoonSharp.VsCodeDebugger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityModManagerNet;

namespace HighPriestess
{
    static class Main
    {
        public static UnityModManager.ModEntry mod;
        public static Script script = new Script();

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            mod = modEntry;
            LuaInterface.mod = new ModInfo
            {
                id = mod.Info.Id,
                name = mod.Info.DisplayName,
                author = mod.Info.Author,
                version = mod.Info.Version,
            };

            var harmony = new Harmony(mod.Info.Id);
            harmony.PatchAll();

            var debugServer = new MoonSharpVsCodeDebugServer();
            debugServer.Start();
            debugServer.AttachToScript(script, "High Priestess Lua Context");

            UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;

            script.Options.DebugPrint = s => { mod.Logger.Log(s); };
            script.Options.ScriptLoader = new FileSystemScriptLoader();

            string _;

            script.Globals["_MILLENNIA"] = UserData.CreateStatic<LuaInterface>();
            script.Globals["_GAME"] = UserData.CreateStatic<AGame>();
            script.Globals["_DECKS"] = UserData.CreateStatic<ADeckManager>();
            script.Globals["_ENTITIES"] = UserData.CreateStatic<AEntityInfoManager>();
            script.Globals["_DEV_CONFIG"] = (Action<string, string>)((string key, string value) => ADevConfig.SetConfigValue(key, value, false, out _));

            var loader = (ScriptLoaderBase)script.Options.ScriptLoader;
            loader.IgnoreLuaPathGlobal = true;
            loader.ModulePaths = new string[] { "Scripts/?", "Scripts/?.lua" };

            script.DoFile(@"Mods\" + mod.Info.Id + @"\Prelude.lua");
            script.DoFile(@"Scripts\Startup.lua");

            return true;
        }
    }
}
