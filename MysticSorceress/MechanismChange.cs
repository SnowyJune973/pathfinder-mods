using System;
using System.Collections.Generic;
using System.Linq;
using Kingmaker;
using Kingmaker.Assets.UI.LevelUp;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Root;
using Kingmaker.Blueprints.Validation;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.UI.Tooltip;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.Class.LevelUp.Actions;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Utility;
using Newtonsoft.Json;
using UnityModManagerNet;
using MysticSorceress.Abilities;
using MysticSorceress.Component;

namespace MysticSorceress {
    static class MechanismChange {
        internal static bool ArchetypeMeetsPrerequisites(BlueprintArchetype archetype, UnitDescriptor unit, LevelUpState state) {
            bool? all = null;
            bool? any = null;
            foreach (var prerequisite in archetype.GetComponents<Prerequisite>()) {
                var passed = prerequisite.Check(null, unit, state);
                if (prerequisite.Group == Prerequisite.GroupType.All) {
                    all = (!all.HasValue) ? passed : (all.Value && passed);
                }
                else {
                    any = (!any.HasValue) ? passed : (any.Value || passed);
                }
            }
            var result = (!all.HasValue || all.Value) && (!any.HasValue || any.Value);
            return result;
        }
    }
}
