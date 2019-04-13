using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using System;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints;

namespace MysticSorceress.Abilities {
    // Token: 0x02000133 RID: 307
    [AllowMultipleComponents]
    public class PrerequisiteFemale : Prerequisite {
        // Token: 0x06000521 RID: 1313 RVA: 0x00006B59 File Offset: 0x00004D59
        public override bool Check(FeatureSelectionState selectionState, UnitDescriptor unit, LevelUpState state) {
            return this.CheckUnit(unit);
        }

        // Token: 0x06000522 RID: 1314 RVA: 0x0005B418 File Offset: 0x00059618
        public bool CheckUnit(UnitDescriptor unit) {
            return unit.Gender == Gender.Female;
        }

        public override string GetUIText() {
            return "性别：女性";
        }
    }
}