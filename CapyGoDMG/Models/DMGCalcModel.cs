using CapyGoDMG.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Html;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;

namespace CapyGoDMG.Models
{
    public class DMGCalcModel
    {
        public bool IsBoss { get; set; }

        public MarkupString CalculatedATK { get; set; }

        public CapyStats CapyStats { get; set; } = new CapyStats();

        public CapyDamageResults CapyDamageResults { get; set; } = new CapyDamageResults();

        public Dictionary<Enums.StatsEnum, float> InGameBonuses { get; set; } = new Dictionary<Enums.StatsEnum, float>();

        public void CalcInGameBonuses()
        {
            InGameBonuses = new Dictionary<Enums.StatsEnum, float>();

            if (CapyStats.Equip_Sphinx == 1)
            {
                InGameBonuses.Combine(Enums.StatsEnum.ATK, 30);
                InGameBonuses.Combine(Enums.StatsEnum.SKILL_CRIT_CHANCE, 10);
            }

            if (CapyStats.Equip_Sphinx == 2)
            {
                InGameBonuses.Combine(Enums.StatsEnum.ATK, 45);
                InGameBonuses.Combine(Enums.StatsEnum.SKILL_CRIT_CHANCE, 15);
            }

            if(CapyStats.Equip_Yawner == 1)
            {
                InGameBonuses.Combine(Enums.StatsEnum.ATK, 18);
            }
            if (CapyStats.Equip_Yawner == 2)
            {
                InGameBonuses.Combine(Enums.StatsEnum.ATK, 28);
            }
            if (CapyStats.Equip_Yawner == 3)
            {
                InGameBonuses.Combine(Enums.StatsEnum.ATK, 40);
            }

            if (CapyStats.Equip_Freya == 1) 
            {
                InGameBonuses.Combine(Enums.StatsEnum.SKILL_CRIT_CHANCE, 24);
                InGameBonuses.Combine(Enums.StatsEnum.SKILL_DMG, 45);
            }
            if (CapyStats.Equip_Freya == 2) 
            {
                InGameBonuses.Combine(Enums.StatsEnum.SKILL_CRIT_CHANCE, 25.5f);
                InGameBonuses.Combine(Enums.StatsEnum.SKILL_DMG, 52.5f);
            }
            if (CapyStats.Equip_Freya == 3) 
            {
                InGameBonuses.Combine(Enums.StatsEnum.SKILL_CRIT_CHANCE, 27);
                InGameBonuses.Combine(Enums.StatsEnum.SKILL_DMG, 60);
            }
            if (CapyStats.Equip_Freya == 4) 
            {
                InGameBonuses.Combine(Enums.StatsEnum.SKILL_CRIT_CHANCE, 27);
                InGameBonuses.Combine(Enums.StatsEnum.SKILL_DMG, 67.5f);
            }
            if (CapyStats.Equip_Freya == 5) 
            {
                InGameBonuses.Combine(Enums.StatsEnum.SKILL_CRIT_CHANCE, 30);
                InGameBonuses.Combine(Enums.StatsEnum.SKILL_DMG, 75);
            }

            if (CapyStats.Equip_Joker == 1)
            {
                InGameBonuses.Combine(Enums.StatsEnum.GLOBAL_SKILL_DMG, 40);
            }
        }


        public List<CalculatedDamage> CalcDMGRaw(Dictionary<Attack, int> attacks, bool withBonus = false)
        {
            var ATK = CapyStats.TOTAL_ATK;

            CapyStats cpStats = (CapyStats)CapyStats.Clone();

            if (withBonus)
            {
                CalcInGameBonuses();
                if (InGameBonuses.ContainsKey(Enums.StatsEnum.ATK))
                    ATK = (long)(ATK * (1 + (InGameBonuses[Enums.StatsEnum.ATK] / 100f)));

                foreach (var bonus in InGameBonuses)
                {
                    switch (bonus.Key)
                    {
                        case Enums.StatsEnum.ATK:
                            break;
                        case Enums.StatsEnum.DMG:
                            cpStats.Stat_DMGIncrease += bonus.Value;
                            break;
                        case Enums.StatsEnum.SKILL_DMG:
                            cpStats.Stat_SkillDMG += bonus.Value;
                            break;
                        case Enums.StatsEnum.GLOBAL_SKILL_DMG:
                            cpStats.Stat_Global_SkillDMG += bonus.Value;
                            break;
                        case Enums.StatsEnum.CRIT_CHANCE:
                            break;
                        case Enums.StatsEnum.WEAPON_CRIT_CHANCE:
                            break;
                        case Enums.StatsEnum.SKILL_CRIT_CHANCE:
                            break;
                        case Enums.StatsEnum.DMG_TAKEN:
                            break;
                    }

                }

            }

            var calculatedAttacks = new List<CalculatedDamage>();

            ATK -= CapyStats.Enemy_Defence;

            foreach (var attack in attacks)
            {
                var _coeff = attack.Key.Coefficient;
                var _critDMG = 1 + (GetCritMultiplier(attack.Key)/100f);
                var _DmgIncrease = 1 + (GetDMGMultiplier(attack.Key, cpStats)/100f);
                var _GlobalDMGIncrease = 1 + (GetRelevantGLOBALDMGIncreases(attack.Key, cpStats)/100f);
                var _FinalDMGIncrease = 1;

                var _damage = ATK * _coeff * _DmgIncrease * _GlobalDMGIncrease * _FinalDMGIncrease;

                calculatedAttacks.Add(new CalculatedDamage(attack.Key, ATK, _DmgIncrease, _GlobalDMGIncrease, _FinalDMGIncrease, _critDMG, 0f));
            }

            return calculatedAttacks;
        }

        public float GetCritMultiplier(Attack attack)
        {
            float total = CapyStats.Stat_CritDMG;

            foreach (var dmgType in attack.DMGTypes)
            {
                switch (dmgType)
                {
                    case Enums.DMGTypesEmum.Basic:
                        total += CapyStats.Stat_BasicCritDMG;
                        break;
                    case Enums.DMGTypesEmum.Skill:
                        total += CapyStats.Stat_SkillCritDMG;
                        break;
                }
            }

            return Math.Min(total, 500);
        }

        public float GetDMGMultiplier(Attack attack, CapyStats cpstats)
        {
            var increase = GetRelevantDMGIncreases(attack, cpstats);

            if (increase < 25) return increase;

            var decrease = GetRelevantEnemyDR(attack);

            return Math.Min(1000, Math.Max(25, increase - decrease));
        }

        public float GetRelevantDMGIncreases(Attack attack, CapyStats cpStats)
        {
            
            float total = cpStats.Stat_DMGIncrease;

            if (IsBoss) total += cpStats.Stat_DMGtoBoss;

            foreach (var dmgType in attack.DMGTypes)
            {
                switch (dmgType)
                {
                    case Enums.DMGTypesEmum.Basic:
                        total += cpStats.Stat_BasicDMG;
                        break;
                    case Enums.DMGTypesEmum.Skill:
                        total += cpStats.Stat_SkillDMG;
                        break;
                    case Enums.DMGTypesEmum.Rage:
                        total += cpStats.Stat_RageDMG;
                        break;
                    case Enums.DMGTypesEmum.Physical:
                        total += cpStats.Stat_PhysicalDMG;
                        break;
                    case Enums.DMGTypesEmum.Fire:
                        total += cpStats.Stat_FireDMG;
                        break;
                    case Enums.DMGTypesEmum.Lightning:
                        total += cpStats.Stat_LightningDMG;
                        break;
                    case Enums.DMGTypesEmum.Ice:
                        total += cpStats.Stat_IceDMG;
                        break;
                    case Enums.DMGTypesEmum.DoT:
                        total += cpStats.Stat_DoTDMG;
                        break;
                    case Enums.DMGTypesEmum.Counter:
                        total += cpStats.Stat_CounterDMG;
                        break;
                    case Enums.DMGTypesEmum.Combo:
                        total += cpStats.Stat_ComboDMG;
                        break;
                    case Enums.DMGTypesEmum.Dagger:
                        total += cpStats.Stat_DaggerDMG;
                        break;
                    case Enums.DMGTypesEmum.Bolt:
                        total += cpStats.Stat_BoltDMG;
                        break;
                    case Enums.DMGTypesEmum.Chi:
                        total += cpStats.Stat_ChiDMG;
                        break;
                }
            }

            return total;
        }
        public float GetRelevantEnemyDR(Attack attack)
        {
            float total = CapyStats.Enemy_DR;

            //if (IsBoss) total += CapyStats.Stat_DMGtoBoss;

            foreach (var dmgType in attack.DMGTypes)
            {
                switch (dmgType)
                {
                    case Enums.DMGTypesEmum.Basic:
                        total += CapyStats.Enemy_BasicDR;
                        break;
                    case Enums.DMGTypesEmum.Skill:
                        total += CapyStats.Enemy_SkillDR;
                        break;
                    case Enums.DMGTypesEmum.Rage:
                        break;
                    case Enums.DMGTypesEmum.Physical:
                        break;
                    case Enums.DMGTypesEmum.Fire:
                        break;
                    case Enums.DMGTypesEmum.Lightning:
                        break;
                    case Enums.DMGTypesEmum.Ice:
                        break;
                    case Enums.DMGTypesEmum.DoT:
                        break;
                    case Enums.DMGTypesEmum.Counter:
                        break;
                    case Enums.DMGTypesEmum.Combo:
                        break;
                    case Enums.DMGTypesEmum.Dagger:
                        break;
                    case Enums.DMGTypesEmum.Bolt:
                        break;
                    case Enums.DMGTypesEmum.Chi:
                        break;
                }
            }

            return total;
        }

        public float GetRelevantGLOBALDMGIncreases(Attack attack, CapyStats cpStats)
        {
            float total = cpStats.Stat_Global_DMGIncrease;

            if (IsBoss) total += cpStats.Stat_Global_DMGtoBoss;

            foreach (var dmgType in attack.DMGTypes)
            {
                switch (dmgType)
                {
                    case Enums.DMGTypesEmum.Basic:
                        total += cpStats.Stat_Global_BasicDMG;
                        break;
                    case Enums.DMGTypesEmum.Skill:
                        total += cpStats.Stat_Global_SkillDMG;
                        break;
                    case Enums.DMGTypesEmum.Rage:
                        total += cpStats.Stat_Global_RageDMG;
                        break;
                    case Enums.DMGTypesEmum.Physical:
                        total += cpStats.Stat_Global_PhysicalDMG;
                        break;
                    case Enums.DMGTypesEmum.Fire:
                        total += cpStats.Stat_Global_FireDMG;
                        break;
                    case Enums.DMGTypesEmum.Lightning:
                        total += cpStats.Stat_Global_LightningDMG;
                        break;
                    case Enums.DMGTypesEmum.Ice:
                        total += cpStats.Stat_Global_IceDMG;
                        break;
                    case Enums.DMGTypesEmum.DoT:
                        total += cpStats.Stat_Global_DoTDMG;
                        break;
                    case Enums.DMGTypesEmum.Counter:
                        total += cpStats.Stat_Global_CounterDMG;
                        break;
                    case Enums.DMGTypesEmum.Combo:
                        total += cpStats.Stat_Global_ComboDMG;
                        break;
                    case Enums.DMGTypesEmum.Dagger:
                        total += cpStats.Stat_Global_DaggerDMG;
                        break;
                    case Enums.DMGTypesEmum.Bolt:
                        total += cpStats.Stat_Global_BoltDMG;
                        break;
                    case Enums.DMGTypesEmum.Chi:
                        total += cpStats.Stat_Global_ChiDMG;
                        break;
                }
            }

            return total;
        }

    }

    public class CapyGear
    {
        public Equipment Weapon { get; set; } = new Equipment();
        public Equipment Chest { get; set; } = new Equipment();
        public Equipment Ring1 { get; set; } = new Equipment();
        public Equipment Ring2 { get; set; } = new Equipment();
        public Equipment Amulet1 { get; set; } = new Equipment();
        public Equipment Amulet2 { get; set; } = new Equipment();
    }

    public class CapyStats : ICloneable
    {
        /// <summary>
        /// The total ATK from all equipment
        /// </summary>
        public long Atk_Equipment { get; set; }

        /// <summary>
        /// The total ATK%
        /// </summary>
        public float Atk_PercentUp { get; set; }

        /// <summary>
        /// The total Global ATK%
        /// </summary>
        public float Atk_GlobalAtk { get; set; }

        /// <summary>
        /// The ATK multiplier from adventurer
        /// </summary>
        public float Atk_Adventurer { get; set; } = 1f;

        /// <summary>
        /// The Total ATK value from stats
        /// </summary>
        public long TOTAL_ATK { get { return (long)(Atk_Equipment * ( 1 + (Atk_PercentUp/100)) * (1 + (Atk_GlobalAtk / 100)) * Atk_Adventurer); } }

        #region DMG stats
        public float Stat_DMGIncrease { get; set; }
        public float Stat_CritDMG { get; set; }
        public float Stat_BasicCritDMG { get; set; }
        public float Stat_SkillCritDMG { get; set; }
        public float Stat_ComboDMG { get; set; }
        public float Stat_CounterDMG { get; set; }
        public float Stat_BasicDMG { get; set; }
        public float Stat_RageDMG { get; set; }
        public float Stat_SkillDMG { get; set; }
        public float Stat_IceDMG { get; set; }
        public float Stat_LightningDMG { get; set; }
        public float Stat_FireDMG { get; set; }
        public float Stat_PhysicalDMG { get; set; }
        public float Stat_DMGtoBoss { get; set; }
        public float Stat_DoTDMG { get; set; }
        public float Stat_DaggerDMG { get; set; }
        public float Stat_ChiDMG { get; set; }
        public float Stat_BoltDMG { get; set; }
        #endregion

        #region Global Stats
        public float Stat_Global_DMGIncrease { get; set; }
        public float Stat_Global_CritDMG { get; set; }
        public float Stat_Global_ComboDMG { get; set; }
        public float Stat_Global_CounterDMG { get; set; }
        public float Stat_Global_BasicDMG { get; set; }
        public float Stat_Global_RageDMG { get; set; }
        public float Stat_Global_SkillDMG { get; set; }
        public float Stat_Global_IceDMG { get; set; }
        public float Stat_Global_LightningDMG { get; set; }
        public float Stat_Global_FireDMG { get; set; }
        public float Stat_Global_PhysicalDMG { get; set; }
        public float Stat_Global_DMGtoBoss { get; set; }
        public float Stat_Global_DoTDMG { get; set; }
        public float Stat_Global_DaggerDMG { get; set; }
        public float Stat_Global_ChiDMG { get; set; }
        public float Stat_Global_BoltDMG { get; set; }
        #endregion

        public float Stat_Final_DMG { get; set; }

        #region Enemy
        public int Enemy_Defence { get; set; }
        public float Enemy_DR { get; set; }
        public float Enemy_SkillDR { get; set; }
        public float Enemy_BasicDR { get; set; }
        #endregion

        #region Equiptments
        public int Equip_Sphinx { get; set; }
        public int Equip_Yawner { get; set; }
        public int Equip_Freya { get; set; }
        public int Statue_Freya { get; set; }
        public int Equip_Elsa { get; set; }
        public int Statue_Elsa { get; set; }
        public int Equip_Joker { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }

    public class CalculatedDamage
    {
        public Attack _Attack { get; set; }
        public long ATK { get; set; }
        public float DMG { get; set; }
        public float GlDMG { get; set; }
        public float FnDMG { get; set; }
        public float CritDMG { get; set; }
        public float CritChance { get; set; }

        public CalculatedDamage(Attack attack, long aTK, float dMG, float glDMG, float fnDMG, float critDMG, float critChance)
        {
            _Attack = attack;
            ATK = aTK;
            DMG = dMG;
            GlDMG = glDMG;
            FnDMG = fnDMG;
            CritDMG = critDMG;
            CritChance = critChance;
        }

        public long CalcDamage()
        {
            return (long)(ATK * _Attack.Coefficient * DMG * GlDMG * FnDMG);
        }

        public long CalcCritDamage()
        {
            return (long)(CalcDamage() * CritDMG);
        }

        public MarkupString ReadAttack()
        {
            return new MarkupString($"<div class=\"panel panel-primary\" style=\"border:groove\">" +
                $"<b>ATK</b> - {ATK.ToKMB()}<br />" +
                $"<b>DMG</b> - x{DMG}<br />" +
                $"<b>Global</b> - x{GlDMG}<br />" +
                $"<b>Crit</b> - x{CritDMG}</div>");
        }
    }

    public class CapyDamageResults
    {
        public MarkupString DamageResult { get; set; }

        public MarkupString ParseAttacks(List<CalculatedDamage> attacks)
        {
            var sb = new StringBuilder();

            foreach (var attack in attacks)
            {
                sb.Append("<div class=\"panel panel-primary\" style=\"border:groove;margin:2px\">");
                sb.Append($"<b>{attack._Attack.Name}</b><br/>");
                sb.Append($"{attack.CalcDamage().ToKMB()} - {attack.CalcCritDamage().ToKMB()} CRIT");
                sb.Append(attack.ReadAttack());
                sb.Append("</div>");
            }

            return DamageResult = new MarkupString(sb.ToString());
        }
    }
}