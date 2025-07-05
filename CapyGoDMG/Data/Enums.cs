namespace CapyGoDMG.Data
{
    public static class Enums
    {
        public enum DMGTypesEmum
        {
            Basic,
            Skill,
            Rage,
            Physical,
            Fire,
            Lightning,
            Ice,
            DoT,
            Counter,
            Combo,
            Dagger,
            Bolt,
            Chi,
            Poison,
            LightSpear
        }

        public enum StatsEnum
        {
            ATK,
            DMG,
            SKILL_DMG,
            GLOBAL_SKILL_DMG,
            CRIT_CHANCE,
            WEAPON_CRIT_CHANCE,
            SKILL_CRIT_CHANCE,
            DMG_TAKEN
        }

        public enum GearSlot
        {
            WEAPON,
            CHEST,
            RING,
            AMULET
        }

        public enum Rarity
        {
            COMMON,
            UNCOMMON,
            RARE,
            EPIC,
            LEGENDARY
        }
    }
}
