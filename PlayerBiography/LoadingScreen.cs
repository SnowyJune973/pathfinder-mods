using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using UnityModManagerNet;
namespace PlayerBiography {
    public class LoadingScreenDescModify {
        public List<UnitReference> pcAndCustomFollowers;
        private Dictionary<string, string> loadScreenDesc;
        public bool Inited {
            get {
                return inited;
            }
        }
        bool inited;
        private bool isPCorCustomFollower(UnitReference unitRef) {
            return UnitHelper.IsCustomCompanion(unitRef) || unitRef == Game.Instance.Player.MainCharacter;
        }
        public LoadingScreenDescModify() {
            inited = false;
            pcAndCustomFollowers = new List<UnitReference>();
            loadScreenDesc = new Dictionary<string, string>();
            Read();
        }
        public void Init() {
            UnityModManager.Logger.Log("Init", "[LoadingScreenDescModify]");
            var allChars = Game.Instance.Player.RemoteCompanions.ToList<UnitReference>();
            allChars = allChars.Union<UnitReference>(Game.Instance.Player.PartyCharacters).ToList<UnitReference>();
            pcAndCustomFollowers = new List<UnitReference>();
            pcAndCustomFollowers.AddRange(from actor in allChars where isPCorCustomFollower(actor) select actor);
            UnityModManager.Logger.Log("Rua", "[LoadingScreenDescModify]");
            foreach (UnitReference unitRef in pcAndCustomFollowers){
                if (!loadScreenDesc.ContainsKey(unitRef.UniqueId)) {
                    loadScreenDesc[unitRef.UniqueId] = unitRef.ToString();
                }
            }
            inited = true;
        }

        public string GetLoadingDescription(UnitReference unitRef) {
            if (!inited) {
                Init();
            }
            if (!pcAndCustomFollowers.Contains(unitRef)){
                return "";
            }
            return loadScreenDesc[unitRef.UniqueId];
        }

        public void UpdateBio(UnitReference unitRef, string bio) {
            string id = unitRef.UniqueId;
            loadScreenDesc[id] = bio;
            Save();
        }

        public void Read() {
            UnityModManager.ModEntry umm = Main.modEntry;
            var dll_dir = umm.Path;
            var filepath = Path.Combine(dll_dir, "bio.txt");
            if (!File.Exists(filepath)) {
                File.Create(filepath).Close();
            }
            FileStream fs = new FileStream(Path.Combine(dll_dir, "bio.txt"), FileMode.Open);
            StreamReader fin = new StreamReader(fs);
            string line;
            while ((line = fin.ReadLine())!= null){
                var value = fin.ReadLine();
                value = value.Replace("\\n", "\n");
                loadScreenDesc[line] = value;
            }
            fin.Close();
            fs.Close();
        }
        public void Save() {
            UnityModManager.ModEntry umm = Main.modEntry;
            var dll_dir = umm.Path;
            FileStream fs = new FileStream(Path.Combine(dll_dir, "bio.txt"), FileMode.Create);
            StreamWriter fout = new StreamWriter(fs);
            foreach (KeyValuePair<string, string> kvp in loadScreenDesc) {
                fout.WriteLine(kvp.Key);
                var write_value = kvp.Value;
                write_value=write_value.Replace("\n", "\\n");
                write_value=write_value.Replace("\t", "\\t");
                fout.WriteLine(write_value);
            }
            fout.Close();
            fs.Close();
        }
    }
}
