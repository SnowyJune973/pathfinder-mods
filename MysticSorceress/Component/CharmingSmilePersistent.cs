using System;
using System.Collections.Generic;
using System.Linq;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Loot;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Designers.Mechanics.Recommendations;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums.Damage;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Utility;

namespace MysticSorceress.Component {
    public class CharmingSmilePersistent : OwnedGameLogicComponent<UnitDescriptor>, IGlobalRulebookHandler<RuleRollD20> {
        public void OnEventAboutToTrigger(RuleRollD20 evt) {
            try {
                var rule = Rulebook.CurrentContext.PreviousEvent as RuleSavingThrow;
                if (rule == null) return;

                var context = rule.Reason.Context?.SourceAbilityContext;
                if (context?.Caster != Owner.Unit  ||
                    context.AbilityBlueprint.Type != AbilityType.Spell) {
                    return;
                }
                if (context.AbilityBlueprint.School != SpellSchool.Enchantment && context.AbilityBlueprint.School != SpellSchool.Illusion) {
                    return;
                }
                // Note: RuleSavingThrow rolls the D20 before setting the stat bonus, so we need to pass it in.
                // (The game's ModifyD20 component has a bug because of this.)
                evt.SetReroll(2, false);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void OnEventDidTrigger(RuleRollD20 evt) {
            if (evt.RerollsAmount > 0) {
                Log.Write($"Persistent spell: reroll {evt.Result}.");
            }
        }
    }
}
