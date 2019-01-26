using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic;
using Kingmaker.UI.LoadingScreen;
using Kingmaker.UI.Common;
using Kingmaker.Localization;
using Kingmaker.Blueprints.Root;
using Kingmaker.Blueprints.Area;
using Kingmaker.Utility;

using JetBrains.Annotations;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Persistence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Harmony12;
using UnityModManagerNet;
using PlayerBiography;

namespace PlayerBiography.Patches {
    [HarmonyPatch(typeof(LoadingScreen))]
    [HarmonyPatch("SetLoadingArea")]
    [HarmonyPatch(new Type[] { typeof(BlueprintArea) })]
    class LoadingScreen_SetLoadingArea_Patch {
        static bool Prefix(LoadingScreen __instance, BlueprintArea area, ref bool ___m_ShowDissolve, ref float ___m_CurrentThreshold, ref Material ___m_PictureMaterial) {
            UnityModManager.Logger.Log("Start", "[LoadingPatch]");
            __instance.MapContainer.SetActive(area.HasCustomUI);
            __instance.Picture.gameObject.SetActive(!area.HasCustomUI);
            if (area.HasCustomUI) {
                List<UnitReference> list = Game.Instance.Player.RemoteCompanions.ToList<UnitReference>();
                list = list.Union<UnitReference>(Game.Instance.Player.PartyCharacters).ToList<UnitReference>();
                if (list.Empty<UnitReference>()) {
                    __instance.MapContainer.SetActive(false);
                }
                
                else {
                    UnityModManager.Logger.Log("Miao", "[LoadingPatch]");
                    UnitReference unitReference = list.Random<UnitReference>();
                    if (UnitHelper.IsCustomCompanion(unitReference.Value)) {
                        UnityModManager.Logger.Log("Miao1", "[LoadingPatch]");
                        var descriptionTextData = Main.lsdm.GetLoadingDescription(unitReference); 
                        TMP_Text characterDesctiptionText = __instance.CharacterDesctiptionText;
                        characterDesctiptionText.text = UIUtility.GetSaberBookFormat(descriptionTextData, default(Color), 140, null);
                        UnityModManager.Logger.Log("miao1end", "[LoadingPatch]");

                    }
                    else if (unitReference == Game.Instance.Player.MainCharacter) {
                        UnityModManager.Logger.Log("miao2", "[LoadingPatch]");
                        var descriptionTextData = Main.lsdm.GetLoadingDescription(unitReference);
                        TMP_Text characterDesctiptionText = __instance.CharacterDesctiptionText;
                        characterDesctiptionText.text = UIUtility.GetSaberBookFormat(descriptionTextData, default(Color), 140, null);
                        UnityModManager.Logger.Log("miao2end", "[LoadingPatch]");
                    }
                    else {
                        BlueprintCompanionStory blueprintCompanionStory = Game.Instance.Player.CompanionStories.Get(unitReference.Value.Blueprint).LastOrDefault<BlueprintCompanionStory>();
                        TMP_Text characterDesctiptionText = __instance.CharacterDesctiptionText;
                        LocalizedString localizedString = (blueprintCompanionStory != null) ? blueprintCompanionStory.Description : null;
                        characterDesctiptionText.text = UIUtility.GetSaberBookFormat((localizedString == null) ? string.Empty : localizedString, default(Color), 140, null);
                    }
                    __instance.CharacterNameText.text = UIUtility.GetSaberBookFormat(unitReference.Value.CharacterName, default(Color), 140, null);
                    __instance.CharacterPortrait.sprite = unitReference.Value.Portrait.FullLengthPortrait;
                }
            }
            else {
                Sprite sprite = __instance.TempLoadingPicture;
                if (area.LoadingScreenSprites.Any<Sprite>()) {
                    sprite = area.LoadingScreenSprites.Random<Sprite>();
                }
                else if (area.ArtSetting != BlueprintArea.SettingType.Unspecified) {
                    LoadingScreen.SettingTypeScreens settingTypeScreens = __instance.SettingTypeScreensList.FirstItem((LoadingScreen.SettingTypeScreens s) => s.Type == area.ArtSetting);
                    sprite = ((settingTypeScreens != null) ? settingTypeScreens.Sprites.Random<Sprite>() : null);
                }
                __instance.Picture.sprite = sprite;
            }
            ___m_ShowDissolve = true;
            ___m_CurrentThreshold = 1f;
            ___m_PictureMaterial.SetFloat("_Threshold", ___m_CurrentThreshold);
            return false;
        }
    }
}
