using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.PubSubSystem;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic.Abilities;
namespace MysticSorceress.Abilities {
    public class CharmingSmileMetamagic : RuleInitiatorLogicComponent<RuleCalculateAbilityParams>, IInitiatorRulebookHandler<RuleCastSpell> {
        public static BlueprintFeature PersistentMetaMagicFeat;

        public void OnEventAboutToTrigger(RuleCastSpell evt) {
            try {
                var context = evt.Context;
                if (context.SpellSchool == SpellSchool.Illusion || context.SpellSchool == SpellSchool.Enchantment) {
                    context.RemoveSpellDescriptor(SpellDescriptor.MindAffecting);
                    context.RemoveSpellDescriptor(SpellDescriptor.Force);
                    context.RemoveSpellDescriptor(SpellDescriptor.Charm);
                }
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void OnEventDidTrigger(RuleCastSpell evt) {
            
        }
        public override void OnEventAboutToTrigger(RuleCalculateAbilityParams evt) {
            try {
                evt.AddMetamagic((Metamagic)0x10000000);
            }
            catch(Exception e) {
                Log.Error(e);
            }
        }

        public override void OnEventDidTrigger(RuleCalculateAbilityParams evt) { }
    };
}
