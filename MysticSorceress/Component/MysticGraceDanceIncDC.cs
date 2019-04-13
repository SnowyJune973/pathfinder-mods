using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Root.Strings.GameLog;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.RuleSystem;

namespace MysticSorceress.Component {
        [AllowMultipleComponents]
        [ComponentName("Increase spell school DC")]
        [AllowedOn(typeof(BlueprintUnitFact))]
        public class MysticGraceDanceIncreaseDC : RuleInitiatorLogicComponent<RuleCalculateAbilityParams> {
            // Token: 0x060019C4 RID: 6596 RVA: 0x0009C1E4 File Offset: 0x0009A3E4
            public override void OnEventAboutToTrigger(RuleCalculateAbilityParams evt) {
                var flag = false;
                int spellTier = 0;
                if (!flag && evt.Spell.IsSpell && (evt.Spell.School == SpellSchool.Enchantment || evt.Spell.School == SpellSchool.Illusion)) {
                    foreach (SpellComponent spellComponent in evt.Spell.GetComponents<SpellComponent>()) {
                        spellTier = (spellTier < evt.SpellLevel) ? evt.SpellLevel : spellTier;
                    }   
                }
                else return;
                if (1 == 1) {
                    int performDC = 25 + spellTier;
                    var D20 = RulebookEvent.Dice.D20;
                    var roll_ans = D20 + base.Owner.Stats.SkillPersuasion;
                    Helpers.GameLog.AddLogEntry(string.Format("表演：d20+{0} = {1}+{0} = {2}，DC {3}", (int)(base.Owner.Stats.SkillPersuasion), D20, roll_ans, performDC),
                        GameLogStrings.Instance.SkillCheckSuccess.Color, string.Format("表演：d20+{0} = {1}+{0} = {2}，DC {3}", (int)(base.Owner.Stats.SkillPersuasion), D20, roll_ans, performDC));
                    var chaModifier = (base.Owner.Stats.Charisma - 10) / 2;
                    if (roll_ans >= performDC+12) {
                        evt.AddBonusDC(chaModifier);
                    }
                    else if(roll_ans >= performDC + 8) {
                        evt.AddBonusDC(chaModifier / 2);
                    }
                    else if(roll_ans >= performDC + 4) {
                        evt.AddBonusDC(2);
                    }
                    else if (roll_ans >= performDC) {
                        evt.AddBonusDC(1);
                    }
                }
            }

            // Token: 0x060019C5 RID: 6597 RVA: 0x00003FBA File Offset: 0x000021BA
            public override void OnEventDidTrigger(RuleCalculateAbilityParams evt) {
            }

            // Token: 0x0400179C RID: 6044
            public SpellSchool School;

            // Token: 0x0400179D RID: 6045
            public int BonusDC;
        }
}
