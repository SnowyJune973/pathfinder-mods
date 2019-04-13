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
    [Harmony12.HarmonyPatch(typeof(DescriptionTemplatesLevelup), "LevelUpClassPrerequisites", typeof(DescriptionBricksBox), typeof(TooltipData), typeof(bool))]
    static class DescriptionTemplatesLevelup_LevelUpClassPrerequisites_Patch {
        static void Postfix(DescriptionTemplatesLevelup __instance, DescriptionBricksBox box, TooltipData data, bool b) {
            try {
                if (data?.Archetype == null) return;
                Prerequisites(__instance, box, data.Archetype.GetComponents<Prerequisite>(), data.ParentFeatureSelection);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        static readonly FastInvoke Prerequisites = Helpers.CreateInvoker<DescriptionTemplatesLevelup>("Prerequisites", new Type[] { typeof(DescriptionBricksBox), typeof(IEnumerable<Prerequisite>), typeof(FeatureSelectionState) });
    }
    [Harmony12.HarmonyPatch(typeof(CharBSelectorLayer), "FillData", typeof(BlueprintCharacterClass), typeof(BlueprintArchetype[]), typeof(CharBFeatureSelector))]
    static class CharBSelectorLayer_FillData_Patch {
        static void Postfix(CharBSelectorLayer __instance, BlueprintCharacterClass charClass, BlueprintArchetype[] archetypesList) {
            try {
                var self = __instance;
                var items = self.SelectorItems;
                if (items == null || archetypesList == null || items.Count == 0) {
                    return;
                }

                // Note: conceptually this is the same as `CharBSelectorLayer.FillDataLightClass()`,
                // but for archetypes.

                // TODO: changing race won't refresh the prereq, although it does update if you change class.
                var state = Game.Instance.UI.CharacterBuildController.LevelUpController.State;
                foreach (var item in items) {
                    var archetype = item?.Archetype;
                    if (archetype == null || !archetypesList.Contains(archetype)) continue;

                    item.Show(state: true);
                    item.Toggle.interactable = item.enabled = MechanismChange.ArchetypeMeetsPrerequisites(archetype, state.Unit, state);
                    var classData = state.Unit.Progression.GetClassData(state.SelectedClass);
                    self.SilentSwitch(classData.Archetypes.Contains(archetype), item);
                }
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
    static class MysticSorceress {
        static LibraryScriptableObject library => Main.library;

        internal static BlueprintArchetype mysticSorceress;
        internal static BlueprintProgression mysticSorProgression;
        internal static BlueprintCharacterClass wizard;
        static UnityModManager.ModEntry.ModLogger logger => Main.logger;
        internal static void Load() {
            logger.Log("Rua!");
            if (mysticSorceress != null) return;

            wizard = Helpers.GetClass("ba34257984f4c41408ce1dc2004e342e");
            mysticSorceress = Helpers.Create<BlueprintArchetype>();
            mysticSorceress.name = "mysticSorceress";
            mysticSorceress.LocalizedName = Helpers.CreateString("MysticSorceress.Name", "谜纱魔女");
            mysticSorceress.LocalizedDescription = Helpers.CreateString("MysticSorceress.Desc", "喵喵喵");
            Helpers.SetField(mysticSorceress, "m_ParentClass", wizard);
            mysticSorceress.RemoveSpellbook = false;
            mysticSorceress.ReplaceStartingEquipment = false;
            mysticSorceress.ChangeCasterType = false;
            mysticSorceress.AddSkillPoints = 1;
            mysticSorceress.OverrideAttributeRecommendations = false;
            mysticSorceress.ReflexSave = wizard.WillSave; //High reflex save.
            mysticSorceress.OverrideAttributeRecommendations = true;
            mysticSorceress.RecommendedAttributes = new StatType[] { StatType.Intelligence, StatType.Charisma };
            mysticSorceress.ReplaceClassSkills = true;

            mysticSorceress.ClassSkills = new StatType[] {
                    StatType.SkillKnowledgeArcana,
                    StatType.SkillKnowledgeWorld,
                    StatType.SkillLoreNature,
                    StatType.SkillLoreReligion,
                    StatType.SkillPersuasion,
                    StatType.SkillUseMagicDevice,
                    StatType.SkillStealth
                };

            //Prerequisite: Int 16, Cha 14, Female, Elf/Halfelf/Human.
            mysticSorceress.AddComponent(
                Helpers.PrerequisiteStatValue(StatType.Intelligence, 16));
            mysticSorceress.AddComponent(
                Helpers.PrerequisiteStatValue(StatType.Charisma, 16));
            mysticSorceress.AddComponent(
                Helpers.PrerequisiteFeaturesFromList(Helpers.elf, Helpers.halfElf, Helpers.human));
            library.AddAsset(mysticSorceress, "3e01290a607e392ba11716c8a930d81f"); //MD5-32[MysticSorceress]
            mysticSorceress.AddComponent(Helpers.Create<PrerequisiteFemale>());

            //Spellbook
            var msSpellbook = Helpers.Create<BlueprintSpellbook>();
            msSpellbook.name = mysticSorceress.name;
            msSpellbook.Name = mysticSorceress.LocalizedName;
            msSpellbook.SpellsPerDay = wizard.Spellbook.SpellsPerDay;
            msSpellbook.SpellsKnown = null;
            msSpellbook.SpellsPerLevel = 2;
            msSpellbook.Spontaneous = false;
            msSpellbook.SpellList = wizard.Spellbook.SpellList;
            msSpellbook.CastingAttribute = StatType.Intelligence;
            msSpellbook.AllSpellsKnown = false;
            msSpellbook.CasterLevelModifier = 0;
            msSpellbook.CanCopyScrolls = true;
            msSpellbook.CantripsType = CantripsType.Cantrips;
            msSpellbook.CharacterClass = wizard;
            library.AddAsset(msSpellbook, "598bb74cead848edecd6b26d79bf9ebd");//MD5-32[MysticSorceress.Spellbook]
            mysticSorceress.ReplaceSpellbook = msSpellbook;

            var wizardArchtypes = wizard.Archetypes.ToList();
            wizardArchtypes.Add(mysticSorceress);
            wizard.Archetypes = wizardArchtypes.ToArray();

            //Progression-Add
            mysticSorceress.AddFeatures = new LevelEntry[] {
                    Helpers.LevelEntry(1, CreateCharismaBonusFeature())
                };
            var featureWeirdAttack = CreateSneakAttackFeature();
            var tmplist = mysticSorceress.AddFeatures.ToList<LevelEntry>();
            for (int i = 6; i <= 20; i += 3) {
                tmplist.Add(Helpers.LevelEntry(i, featureWeirdAttack));
            }
            tmplist.Add(Helpers.LevelEntry(12, CreateMysticCurtain()));
            tmplist.Add(Helpers.LevelEntry(8, CreateMysticGrace()));
            tmplist.Add(Helpers.LevelEntry(16, CreateResilientIllusion()));
            tmplist.Add(Helpers.LevelEntry(10, library.Get<BlueprintFeature>("f42f41dd022e73d4a95019bc03230014")));

            tmplist.Add(Helpers.LevelEntry(20, CreateCharmingSmile()));

            mysticSorceress.AddFeatures = tmplist.ToArray<LevelEntry>();

            //Progress-Remove
            mysticSorceress.RemoveFeatures = new LevelEntry[] {
                    Helpers.LevelEntry(1, library.Get<BlueprintFeatureSelection>("8c3102c2ff3b69444b139a98521a4899")),//Wizard feat selection
                    Helpers.LevelEntry(10, library.Get<BlueprintFeatureSelection>("8c3102c2ff3b69444b139a98521a4899")),
                    Helpers.LevelEntry(15, library.Get<BlueprintFeatureSelection>("8c3102c2ff3b69444b139a98521a4899"))
                };
            logger.Log(JsonConvert.SerializeObject(mysticSorceress));
        }

        internal static BlueprintFeature CreateCharismaBonusFeature() {
            var elvenMagic = library.Get<BlueprintFeature>("55edf82380a1c8540af6c6037d34f322");
            var component = Helpers.Create<AddStatBonusByClassLevel>();
            component.Descriptor = ModifierDescriptor.Circumstance;
            component.Values = new int[] { -1, 2, 2, 2, 2, 4, 4, 4, 4, 6, 6, 6, 6, 8, 8, 8, 8, 10, 10, 10, 10 };
            component.Stat = StatType.Charisma;
            component.ScaleByBasicAttackBonus = false;
            component.Wizard = wizard;
            var componentInt = Helpers.Create<AddStatBonusByClassLevel>();
            componentInt.Descriptor = ModifierDescriptor.Circumstance;
            componentInt.Values = new int[] { -1, 0, 0, 2, 2, 2, 2, 2, 2, 4, 4, 4, 4, 4, 4, 6, 6, 6, 8, 8, 10 };
            componentInt.Stat = StatType.Intelligence;
            componentInt.ScaleByBasicAttackBonus = false;
            componentInt.Wizard = wizard;
            return Helpers.CreateFeature("MysticSorceress.CharmingVeil",
                "幻惑轻纱",
                "谜纱魔女们是幻术的大师。虽近在咫尺，你却很难看清轻纱之下谜纱魔女们的真实面目：然而正是这种若隐若现的距离产生了超自然的美。\n1级时，谜纱魔女的魅力获得+2环境加值。之后每4级，这个环境加值增加2点，到17级时到达最大值+10。\n3级时，谜纱魔女的智力获得+2环境加值。之后每4级，这个环境加值增加2点，到19级时到达最大值+10。",
                "5ded197db3ffa16a8e1e5674f7b217e9",//MD5-32[MysticSorceress.FeatureCharmingVeil]
                elvenMagic.Icon,
                FeatureGroup.None,
                component, componentInt);
        }


        internal static BlueprintFeature CreateSneakAttackFeature() {
            var rougeSneakAttack = library.Get<BlueprintFeature>("9b9eac6709e1c084cb18c3a366e0ec87");
            var component = Helpers.Create<AddStatBonus>();
            component.Descriptor = ModifierDescriptor.Feat;
            component.Value = 1;
            component.Stat = StatType.SneakAttack;
            component.ScaleByBasicAttackBonus = false;
            var ans = Helpers.CreateFeature("MysticSorceress.WeirdAttack",
                "奇异攻势",
                "谜纱魔女们飘忽不定的身影为她们的攻击创造了优势。\n6级开始，谜纱魔女可以如游荡者一般进行偷袭，造成1d6点精准伤害，使用她的谜纱魔女等级作为游荡者等级。6级之后每升3级，偷袭伤害再增加1d6点。",
                "69b4fc41b944d1b196f1371e3f4b4224",//MD5-32[MysticSorceress.FeatureWeirdAttack]
                rougeSneakAttack.Icon,
                FeatureGroup.None,
                component);
            ans.Ranks = 20;
            return ans;
        }

        /*internal static BlueprintFeature CreateMysticCurtain() {
            var feat = Helpers.CreateFeature("MysticSorceress.MysticCurtain", "神秘之幕",
                "12级时，谜纱魔女们学会如何操纵幻术面纱来保护自己。\n谜纱魔女可以以一个迅捷动作激活此能力，在等同于她魅力调整值的轮数内，她将她的魅力调整值（如为负，则视为0）加到她的AC（这是一个偏斜加值）和豁免上，如同水妖精的“绝世优雅”能力。这个能力每天可使用2次，每升2级再增加1次使用次数。",
                "fd2512f0ca8aa91ed51557b0d3ff12d8",//MD5-32[MysticSorceress.FeatureMysticCurtain]
                Helpers.GetIcon("89940cde01689fb46946b2f8cd7b66b7"), // Invisibility
                FeatureGroup.None);
            //MD5-32[MysticSorceress.FeatureMysticCurtain.Resource]
            var resource = Helpers.CreateAbilityResource($"{feat.name}.Resource", "", "", "b9754ff848fd9782ae4bab15c288d02d", feat.Icon);
            resource.SetIncreasedByLevelStartPlusDivStep(1, 12, 0, 2, 1, 0, 0, new BlueprintCharacterClass[] { wizard });

            var spell = Helpers.CreateAbility($"{feat.name}.Ability", feat.Name, feat.Description,
                "ed35bf90401733ee4541c6f055770e2b", feat.Icon,//MD5-32[MysticSorceress.FeatureMysticCurtain.Ability]
                AbilityType.Supernatural, UnitCommand.CommandType.Swift, AbilityRange.Personal, "", "",
                Helpers.CreateRunActions(
                    Helpers.CreateActionDealDamage(DamageEnergyType.Fire,
                        DiceType.D6.CreateContextDiceValue(1, bonus: Helpers.CreateContextValueRank()))),
                Helpers.CreateContextRankConfig(ContextRankBaseValueType.ClassLevel, ContextRankProgression.Div2, classes: oracleArray),
                Helpers.CreateDeliverTouch());
        }*/

        internal static BlueprintFeature CreateMysticCurtain() {
            var component = Helpers.Create<MysticSorceressUnearthlyGrace>();

            var feat = Helpers.CreateFeature("MysticSorceress.MysticCurtain", "神秘之幕",
                "12级时，谜纱魔女们学会如何操纵幻术面纱来保护自己。\n谜纱魔女将她的魅力调整值（如为负，则视为0）加到她的AC（这是一个偏斜加值）和豁免上，如同水妖精的“绝世优雅”能力。这个能力取代法师的15级奖励专长。",
                "fd2512f0ca8aa91ed51557b0d3ff12d8",//MD5-32[MysticSorceress.FeatureMysticCurtain]
                Helpers.GetIcon("89940cde01689fb46946b2f8cd7b66b7"), // Invisibility
                FeatureGroup.None, component);
            return feat;
        }

        internal static BlueprintFeature CreateMysticGrace() {
            var component1_Illu = Helpers.Create<IncreaseSpellSchoolDC>();
            var component1_Ench = Helpers.Create<IncreaseSpellSchoolDC>();
            component1_Illu.BonusDC = component1_Ench.BonusDC = 1;
            component1_Ench.School = SpellSchool.Enchantment;
            component1_Illu.School = SpellSchool.Illusion;

            var component2 = Helpers.Create<MysticGraceDanceIncreaseDC>();
            var feat = Helpers.CreateFeature("MysticSorceress.MysticGrace", "婀娜倩影",
                "谜纱魔女飞扬的裙裾有着摄人心魄的美丽，她优雅的舞步和纤细的身姿帮助她迷惑他人。\n8级时，谜纱魔女释放幻术系和惑控系法术时DC+1。" +
                "此外，在她释放一个幻术或惑控系法术时，若将施法时间延长至整轮动作，她可以做一次DC=25+法术环级的沟通检定，根据检定的结果来为法术DC附加调整值：\n" +
                "★ 低于DC：无效果\n★ DC+(0~3)点：法术DC增加1\n" +
                "★ DC+(4~7)点：法术DC增加2\n" +
                "★ DC+(8~11)点：法术DC增加她魅力调整值的一半\n" +
                "★ DC+(12~)点：法术DC增加她的魅力调整值。\n这个能力取代10级的法师奖励专长。",
                "6f7d98dc704b6dbc1bf4f1fab3626bd2",//MD5-32[MysticSorceress.FeatureMysticGrace]
                Helpers.GetIcon("c7531715a3f046d4da129619be63f44c"), // Calistria
                FeatureGroup.None, component1_Illu, component1_Ench, component2);
            return feat;
        }
        internal static BlueprintFeature CreateCharmingSmile() {
            var component = Helpers.Create<CharmingSmileMetamagic>();
            var component2 = Helpers.Create<CharmingSmilePersistent>();
            var feat = Helpers.CreateFeature("MysticSorceress.CharmingSmile", "迷人微笑",
                "20级时，谜纱魔女已熟练于操控人心，一举一动皆有魔法力量流动。\n谜纱魔女释放的所有幻术系和惑控系法术视为没有[影响心灵]、[胁迫]、[魅惑]描述符，同时目标为抵抗这些法术进行的豁免检定双骰取低。",
                "de38ce508f26b8caa4c8972851331e73",//MD5-32[MysticSorceress.FeatureCharmingSmile]
                Helpers.GetIcon("d3f14f00f675a6341a41d2194186835c"), // AesimarHalo
                FeatureGroup.None, component, component2);
            return feat;
        }

        internal static BlueprintFeature CreateResilientIllusion() {
            var component = Helpers.Create<ResilientIllusionSelective>();
            var feat = Helpers.CreateFeature("MysticSorceress.ResilientIllusion", "弹性幻术",
                "控制幻术，而不被幻术所迷惑，是很多谜纱魔女的追求。在这样的追求之下，她们最终学会了操控幻术使其针对特定目标的方法。\n16级起，谜纱魔女释放的幻术系法术如同被附加了甄选法术专长一般。",
                "e3f928448bbbaf1816d14aa7e1cace95",//MD5-32[MysticSorceress.FeatureResilientIllusion]
                Helpers.GetIcon("cd1f4a784e0820647a34fe9bd5ffa770"), // TrickeryDomainBaseFeature
                FeatureGroup.None, component);
            return feat;
        }
    }
}
