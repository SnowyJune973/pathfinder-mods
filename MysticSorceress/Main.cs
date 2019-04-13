using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Root;
using Kingmaker.UI.Common.Animations;
using Kingmaker.UI.LevelUp;
using Kingmaker.Utility;
using UnityEngine;
using UnityModManagerNet;

namespace MysticSorceress {
    public class Main {
        internal static LibraryScriptableObject library;
        public static bool enabled;
        public static UnityModManager.ModEntry.ModLogger logger;
        static Harmony12.HarmonyInstance harmonyInstance;

        [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), "LoadDictionary", new Type[0])]
        static class LibraryScriptableObject_LoadDictionary_Patch {
            static void Postfix(LibraryScriptableObject __instance) {
                var self = __instance;
                if (Main.library != null) return;
                Main.library = self;
                SafeLoad(Helpers.Load, "Helpers");
                SafeLoad(MysticSorceress.Load, "Mystic");
                
            }
        }
        static bool Load(UnityModManager.ModEntry modEntry) {
            logger = modEntry.Logger;
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            harmonyInstance = Harmony12.HarmonyInstance.Create(modEntry.Info.Id);
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
            enabled = value;
            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry) {
            if (!enabled) return;
        }
        static void OnSaveGUI(UnityModManager.ModEntry modEntry) {
        }

        internal static Exception Error(String message) {
            logger?.Log(message);
            return new InvalidOperationException(message);
        }

        internal static void SafeLoad(Action load, String name) {
            try {
                load();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}