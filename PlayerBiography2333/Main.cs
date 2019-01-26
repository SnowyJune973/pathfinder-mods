using System;
using System.Linq;
using System.IO;
using System.Text;
using Harmony12;
using UnityModManagerNet;
using Kingmaker;
using UnityEngine;

namespace PlayerBiography {
    public static class Main {
        static bool isOn = true;
        static string bio;
        static bool Load(UnityModManager.ModEntry modEntry) {
            isOn = true;
            bio = "";
            modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
            modEntry.OnGUI = new Action<UnityModManager.ModEntry>(Main.OnGUI);
            return true;
        }
        
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
            isOn = value;
            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry) {
            GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
            GUILayout.Label("Input biography", new GUILayoutOption[]{
                GUILayout.ExpandWidth(false)
            });
            GUILayout.TextArea(bio, Array.Empty<GUILayoutOption>());
            GUILayout.EndVertical();
        }
    }
}
