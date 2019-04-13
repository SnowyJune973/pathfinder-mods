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
    public class ResilientIllusionSelective : RuleInitiatorLogicComponent<RuleSpellTargetCheck> {
        public override void OnEventAboutToTrigger(RuleSpellTargetCheck evt) {
            try {
                //Log.Write($"Selective Spell checking target: {evt.Target.CharacterName}, has metamagic? {evt.Context.HasMetamagic((Metamagic)ModMetamagic.Selective)}");
                if (evt.Context.SpellSchool == SpellSchool.Illusion && evt.Initiator.IsAlly(evt.Target)) {
                    Log.Write($"Selective Spell setting target to be immune: {evt.Target.CharacterName}");
                    evt.IsImmune = true;
                }
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public override void OnEventDidTrigger(RuleSpellTargetCheck evt) { }
    }

    public class RuleSpellTargetCheck : RulebookTargetEvent {
        public readonly MechanicsContext Context;

        /// The area effect, if this check is from one, otherwise null.
        public readonly AreaEffectEntityData AreaEffect;

        public bool CanTargetUnit { get; private set; }

        public bool IsImmune { get; set; }

        public RuleSpellTargetCheck(MechanicsContext context, UnitEntityData target, AreaEffectEntityData areaEffect = null)
            : base(context.MaybeCaster, target) {
            Context = context;
            AreaEffect = areaEffect;
        }

        public override void OnTrigger(RulebookEventContext context) {
            CanTargetUnit = !IsImmune;
            //Log.Write($"RuleSpellTargetCheck: caster {Initiator} can target {Target}? {CanTargetUnit}");
        }
    }

    [Harmony12.HarmonyPatch(typeof(AbilityEffectRunAction), "Apply", typeof(AbilityExecutionContext), typeof(TargetWrapper))]
    static class AbilityEffectRunAction_Apply_Patch {
        internal static bool Prefix(AbilityExecutionContext context, TargetWrapper target) {
            try {
                return !target.IsUnit || context.TriggerRule(new RuleSpellTargetCheck(context, target.Unit)).CanTargetUnit;
            }
            catch (Exception e) {
                Log.Error(e);
            }
            return true;
        }
    }

    [Harmony12.HarmonyPatch(typeof(AbilityEffectRunActionOnClickedTarget), "Apply", typeof(AbilityExecutionContext))]
    static class AbilityEffectRunActionOnClickedTarget_Apply_Patch {
        static bool Prefix(AbilityExecutionContext context) => AbilityEffectRunAction_Apply_Patch.Prefix(context, context.MainTarget);
    }

    [Harmony12.HarmonyPatch(typeof(AreaEffectEntityData), "ShouldUnitBeInside", typeof(UnitEntityData))]
    static class AreaEffectEntityData_ShouldUnitBeInside_Patch {
        static void Postfix(AreaEffectEntityData __instance, UnitEntityData unit, ref bool __result) {
            try {
                if (!__result) return;
                var self = __instance;
                var context = self.Context;
                __result = context.TriggerRule(new RuleSpellTargetCheck(context, unit, self)).CanTargetUnit;
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}

