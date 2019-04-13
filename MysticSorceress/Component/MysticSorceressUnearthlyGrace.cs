using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic;

namespace MysticSorceress.Component {
    class MysticSorceressUnearthlyGrace : OwnedGameLogicComponent<UnitDescriptor>{
        public override void OnTurnOn() {
            ModifiableValueAttributeStat charisma = base.Owner.Stats.Charisma;
            ModifiableValueArmorClass ac = base.Owner.Stats.AC;
            ModifiableValueSavingThrow saveFort = base.Owner.Stats.SaveFortitude;
            ModifiableValueSavingThrow saveRef = base.Owner.Stats.SaveReflex;
            ModifiableValueSavingThrow saveWill = base.Owner.Stats.SaveWill;
            this.m_ModifierAC = ac.AddModifier(charisma.Bonus, this, ModifierDescriptor.Deflection);
            this.m_ModifierFort = saveFort.AddModifier(charisma.Bonus, this, ModifierDescriptor.Deflection);
            this.m_ModifierRef = saveRef.AddModifier(charisma.Bonus, this, ModifierDescriptor.Deflection);
            this.m_ModifierWill = saveWill.AddModifier(charisma.Bonus, this, ModifierDescriptor.Deflection);
        }

        // Token: 0x06005F24 RID: 24356 RVA: 0x0003BCF6 File Offset: 0x00039EF6
        public override void OnTurnOff() {
            if (this.m_ModifierAC != null) {
                this.m_ModifierAC.Remove();
            }
            if (this.m_ModifierFort != null) {
                this.m_ModifierFort.Remove();
            }
            if(this.m_ModifierRef != null) {
                this.m_ModifierRef.Remove();
            }
            if (this.m_ModifierWill != null) {
                this.m_ModifierWill.Remove();
            }
            this.m_ModifierAC = null;
            this.m_ModifierFort = null;
            this.m_ModifierRef = null;
            this.m_ModifierWill = null;
        }

        // Token: 0x04004CDB RID: 19675
        private ModifiableValue.Modifier m_ModifierAC, m_ModifierFort, m_ModifierRef, m_ModifierWill;
    }
}
