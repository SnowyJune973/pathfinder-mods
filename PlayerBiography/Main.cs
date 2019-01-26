using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;
using Harmony12;
using UnityModManagerNet;
using Kingmaker;
using Kingmaker.Localization;
using Kingmaker.EntitySystem.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace PlayerBiography {
    public static class Main {
        static bool isOn = true;
        public static bool isShowingBio = true;
        public static string bio, title = "Title";
        private static string text1_area;
        private static int present_actor = -1;
        private static Dictionary<UnitReference, string> text1_datas;
        public static LoadingScreenDescModify lsdm;
        static bool Load(UnityModManager.ModEntry modEntry) {
            isOn = true;
            lsdm = new LoadingScreenDescModify();
            text1_area = "";
            text1_datas = new Dictionary<UnitReference, string>();
            LoadPatches.Load();
            modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
            modEntry.OnGUI = new Action<UnityModManager.ModEntry>(Main.OnGUI);
            return true;
        }
        
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
            isOn = value;
            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry) {
            Scene activeScene = SceneManager.GetActiveScene();
            bool flag = activeScene.name == "Start" || activeScene.name == "MainMenu";
            if (flag) {
                GUILayout.Label("Game is not loaded.", Array.Empty<GUILayoutOption>());
                return;
            }
            /*
            GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
            GUILayout.Label("Input biography", new GUILayoutOption[]{
                GUILayout.ExpandWidth(false)
            });
            text_area1 = GUILayout.TextArea(text_area1, Array.Empty<GUILayoutOption>());
            var button1Click = GUILayout.Button(new GUIContent("Refresh bio"), new GUILayoutOption[] {
                GUILayout.ExpandWidth(false)
            });
            if (button1Click) {
                bio = text_area1;
            }
            GUILayout.EndVertical();*/
            lsdm.Init();
            if (lsdm.Inited) {
                GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
                GUILayout.Label(new GUIContent("Loading Description:"), Array.Empty<GUILayoutOption>());
                GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
                var actor_cnt = lsdm.pcAndCustomFollowers.Count;
                bool[] buttons_1 = new bool[actor_cnt];
                for (int i = 0; i < actor_cnt; i++) {
                    var unitRef = lsdm.pcAndCustomFollowers[i];
                    buttons_1[i] = GUILayout.Button(new GUIContent("  "+unitRef.Value.CharacterName+"  "), new GUILayoutOption[]{
                        GUILayout.ExpandWidth(false)
                    });
                    text1_datas[unitRef] = lsdm.GetLoadingDescription(unitRef);
                }
                GUILayout.EndHorizontal();
                for (int i = 0; i < actor_cnt; i++) {
                    if (buttons_1[i]) {
                        if (present_actor >= 0) {
                            text1_datas[lsdm.pcAndCustomFollowers[present_actor]] = text1_area;
                        }
                        present_actor = i;
                        text1_area = text1_datas[lsdm.pcAndCustomFollowers[present_actor]];
                        break;
                    }
                }
                if (present_actor >= 0) {
                    text1_area = GUILayout.TextArea(text1_area, new GUILayoutOption[] {
                        GUILayout.ExpandWidth(true)
                    });
                    text1_datas[lsdm.pcAndCustomFollowers[present_actor]] = text1_area;
                    var button2 = GUILayout.Button(new GUIContent("Submit"), new GUILayoutOption[] {
                        GUILayout.ExpandWidth(false)
                    });
                    if (button2) {
                        lsdm.UpdateBio(lsdm.pcAndCustomFollowers[present_actor], text1_area);
                    }
                }
                GUILayout.EndVertical();
            }
        }


    }
}
