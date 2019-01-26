using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker;
using Kingmaker.UnitLogic;
using Kingmaker.UI.ServiceWindow.CharacterScreen;
using Harmony12;
using UnityModManagerNet;

namespace PlayerBiography {
    [HarmonyPatch(typeof(CharSBiography))]
    [HarmonyPatch("FillData")]
    [HarmonyPatch(new Type[] { typeof(UnitDescriptor) })]
    class CharSBiography_FillData_patch {
        private static void Postfix(CharSBiography __instance, UnitDescriptor unit) {
            UnityModManager.Logger.Log("喵喵！", "Patch: ");
            if (unit.Unit != Game.Instance.Player.MainCharacter) {
                return;
            }
            UnityModManager.Logger.Log("喵喵喵！", "Patch: ");
            __instance.BioText.text = Main.bio + "\n\n" + __instance.BioText.text;
            UnityModManager.Logger.Log("喵4！", "Patch: ");
        }

        CharSBiography_FillData_patch() {
            UnityModManager.Logger.Log("喵！", "Patch: ");
        }
    }
}
