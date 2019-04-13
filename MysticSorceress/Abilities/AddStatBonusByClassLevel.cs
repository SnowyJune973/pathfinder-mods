using System;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic;

namespace MysticSorceress.Abilities {
    // Token: 0x020012DE RID: 4830
    [ComponentName("Add stat bonus")]
    [AllowedOn(typeof(BlueprintUnitFact))]
    [AllowedOn(typeof(BlueprintUnit))]
    [AllowMultipleComponents]
    public class AddStatBonusByClassLevel : OwnedGameLogicComponent<UnitDescriptor>, IHandleEntityComponent<UnitEntityData> {
        // Token: 0x06005E65 RID: 24165 RVA: 0x00194AD8 File Offset: 0x00192CD8
        public override void OnTurnOn() {
            ModifiableValue stat = base.Owner.Stats.GetStat(this.Stat);
            int level = base.Owner.Progression.GetClassLevel(this.Wizard);
            int num = this.Values[level] * base.Fact.GetRank();
            
            if (stat != null) {
                this.m_Modifier = stat.AddModifier(num, this, this.Descriptor);
            }
        }

        // Token: 0x06005E66 RID: 24166 RVA: 0x0003B6ED File Offset: 0x000398ED
        public override void OnTurnOff() {
            if (this.m_Modifier != null) {
                this.m_Modifier.Remove();
            }
            this.m_Modifier = null;
        }

        // Token: 0x06005E67 RID: 24167 RVA: 0x00194B54 File Offset: 0x00192D54
        public void OnEntityCreated(UnitEntityData entity) {
            int level = base.Owner.Progression.GetClassLevel(this.Wizard);
            if (base.Fact == null) {
                ModifiableValue stat = entity.Stats.GetStat(this.Stat);
                if (stat != null) {
                    stat.AddModifier(this.Values[level], this, this.Descriptor);
                }
            }
        }

        // Token: 0x06005E68 RID: 24168 RVA: 0x00003FBA File Offset: 0x000021BA
        public void OnEntityRemoved(UnitEntityData entity) {
        }
        public BlueprintCharacterClass Wizard;

        // Token: 0x04004C47 RID: 19527
        public ModifierDescriptor Descriptor;

        // Token: 0x04004C48 RID: 19528
        public StatType Stat;

        // Token: 0x04004C49 RID: 19529
        public int[] Values;

        // Token: 0x04004C4A RID: 19530
        public bool ScaleByBasicAttackBonus;

        // Token: 0x04004C4B RID: 19531
        private ModifiableValue.Modifier m_Modifier;
    }
}

