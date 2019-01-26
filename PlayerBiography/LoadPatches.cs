using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.UI.LoadingScreen;
using Harmony12;
using PlayerBiography.Patches;
using System.Reflection;

namespace PlayerBiography {
    static public class LoadPatches {
        private static HarmonyInstance harmony;
        private static void LoadOnePrefixPatch(Type T1, string Mthd1, Type T2, string Mthd2) {
            harmony.Patch(T1.GetMethod(Mthd1), new HarmonyMethod(T2.GetMethod(Mthd2)));
        }
        public static void Load() {
            harmony = HarmonyInstance.Create("asia.snowyjune.me");
            //LoadOnePrefixPatch(typeof(LoadingScreen), "SetLoadingArea", typeof(LoadingScreen_SetLoadingArea_Patch), "Prefix");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
