using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
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
    [HarmonyPatch(typeof(CharSComponentStory))]
    [HarmonyPatch("SetStory")]
    [HarmonyPatch(new Type[] { typeof(BlueprintCompanionStory), typeof(bool) })]
    class CharSComponentStory_SetStory_Patch {
        static bool Prefix(CharSComponentStory __instance, BlueprintCompanionStory story, bool refreshSize, ref CharSStories ___Controller) {
            if(story.Companion != Game.Instance.BlueprintRoot.DefaultPlayerCharacter) {
                return true;
            }
            __instance.gameObject.SetActive(true);
            var setter_HasStory = AccessTools.Property(typeof(CharSComponentStory),"HasStory").GetSetMethod(true);
            setter_HasStory.Invoke(__instance, new object[1]{ true});
            __instance.Story.SetActive(true);
            __instance.Placeholder.SetActive(false);
            __instance.MainPicture.sprite = __instance.DefaultImage;
            __instance.TitlePicture.text = Main.title;
            __instance.TitleStory.text = UIUtility.GetSaberBookFormat(Main.title, default(Color), 140, null);
            __instance.StoryText.text = Main.bio;
            var ControllerFuck = ___Controller;
            if (refreshSize) {
                __instance.FadeAnimator.DisappearAnimation(delegate {
                    __instance.gameObject.SetActive(false);
                    float num = __instance.BoxMaxSize.y / (float)(ControllerFuck.Stories.Count);
                    if (num > __instance.PictureMaxSize.y) {
                        num = __instance.PictureMaxSize.y;
                    }
                    __instance.BoxMinSize = new Vector2(__instance.BoxMinSize.x, num);
                    __instance.PictureMinSize = new Vector2(__instance.PictureMinSize.x, num - 10f);
                    __instance.FadeAnimator.AppearAnimation(null);
                });
            }
            return false;
        }
    }
}
