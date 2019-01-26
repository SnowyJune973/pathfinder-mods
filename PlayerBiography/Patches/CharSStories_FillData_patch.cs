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
using PlayerBiography;
namespace PlayerBiography.Patches {
    [HarmonyPatch(typeof(CharSStories))]
    [HarmonyPatch("FillData")]
    [HarmonyPatch(new Type[] { typeof(UnitDescriptor) })]
    class CharSStories_FillData_patch {
        private static bool Prefix(CharSStories __instance, UnitDescriptor unit, ref List<CharSComponentStory> ___m_StoryBodies, ref int ___m_StoriesCount) {
            if(unit.Unit != Game.Instance.Player.MainCharacter) {
                return true;
            }
            __instance.Clear();
            var playerStory = new BlueprintCompanionStory();
            playerStory.Companion = Game.Instance.BlueprintRoot.DefaultPlayerCharacter;

            var setter_Stories = AccessTools.Property(typeof(CharSStories), "Stories").GetSetMethod(true);
            setter_Stories.Invoke(__instance, new object[] { new List<BlueprintCompanionStory>() { playerStory } });
            if (__instance.Stories == null || __instance.Stories.Count <= 0) {
                return false;
            }
            for (int i = 0; i < ___m_StoryBodies.Count; i++) {
                if (i < __instance.Stories.Count) {
                    ___m_StoryBodies[i].SetStory(__instance.Stories[i], ___m_StoriesCount != __instance.Stories.Count);
                }
            }
            for (int j = __instance.Stories.Count; j < ___m_StoryBodies.Count; j++) {
                ___m_StoryBodies[j].Clear();
            }
            if (__instance.Stories.Count<BlueprintCompanionStory>() == 1) {
                __instance.OpenOne(0);
            }
            else {
                __instance.ShowAll();
            }
            ___m_StoriesCount = __instance.Stories.Count;
            return false;
        }
    }
}
