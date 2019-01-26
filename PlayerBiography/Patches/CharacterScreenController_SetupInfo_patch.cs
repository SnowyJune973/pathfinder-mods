using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic;
using Kingmaker.UI.ServiceWindow.CharacterScreen;
using Kingmaker.Localization;
using Kingmaker.Blueprints.Root;
using Harmony12;
using UnityModManagerNet;
using UnityEngine;
using UnityEngine.UI;
using Kingmaker.UI.Common;
using PlayerBiography;
namespace PlayerBiography.Patches {
    [HarmonyPatch(typeof(CharacterScreenController))]
    [HarmonyPatch("SetupInfo")]
    [HarmonyPatch(new Type[] { typeof(bool) })]
    class CharacterScreenController_SetupInfo_patch {
        static bool Prefix(CharacterScreenController __instance, bool lightUpdate, ref UnitDescriptor ___m_CurrentCharacter, ref int ___m_CurrentSection) {
            if (!Main.isShowingBio || ___m_CurrentCharacter.Unit != Game.Instance.Player.MainCharacter) {
                return true;
            }
            try {
                __instance.AbilityScores.FillData(___m_CurrentCharacter);
                __instance.CharName.text = UIUtility.GetSaberBookFormat(___m_CurrentCharacter.CharacterName, default(Color), 140, null);
                __instance.Alignment.UpdateData(___m_CurrentCharacter);
                __instance.AlignmentHistory.AlwaysHidden = false;
                __instance.AlignmentHistoryBiography.AlwaysHidden = true;
                __instance.AlignmentHistory.UpdateData(___m_CurrentCharacter);
                __instance.AlignmentHistoryBiography.UpdateData(___m_CurrentCharacter);
                __instance.Stories.AlwaysHidden = false;
                __instance.Level.FillData(___m_CurrentCharacter);
                __instance.ShowSection(___m_CurrentSection);
            }
            finally {
            }
            return false;
        }
    }
}
