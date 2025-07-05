namespace CapyGoDMG.Data
{
    public class Attack
    {
        public string Name {  get; set; }
        public float Coefficient { get; set; }
        public List<Enums.DMGTypesEmum> DMGTypes { get; set; }

        public static Attack NormalAttack => new Attack { Name = "Normal", Coefficient = 1f, DMGTypes = new List<Enums.DMGTypesEmum> { Enums.DMGTypesEmum.Basic } };
        public static Attack ComboAttack => new Attack { Name = "Combo", Coefficient = 1f, DMGTypes = new List<Enums.DMGTypesEmum> { Enums.DMGTypesEmum.Basic, Enums.DMGTypesEmum.Combo } };
        public static Attack CounterAttack => new Attack { Name = "Counter", Coefficient = 1f, DMGTypes = new List<Enums.DMGTypesEmum> { Enums.DMGTypesEmum.Basic, Enums.DMGTypesEmum.Counter } };
        
        //todo: Rage Attack is based on the weapon
        //public static Attack RageAttack => new Attack { /*Coefficient = 1f,*/ DMGTypes = new List<Enums.DMGTypesEmum> { Enums.DMGTypesEmum.Skill, Enums.DMGTypesEmum.Physical, Enums.DMGTypesEmum.Rage } };
        public static Attack DaggerAttack => new Attack { Name = "Dagger", Coefficient = 0.45f, DMGTypes = new List<Enums.DMGTypesEmum> { Enums.DMGTypesEmum.Skill, Enums.DMGTypesEmum.Physical, Enums.DMGTypesEmum.Dagger } };
        public static Attack BoltAttack => new Attack { Name = "Bolt", Coefficient = 0.3f, DMGTypes = new List<Enums.DMGTypesEmum> { Enums.DMGTypesEmum.Skill, Enums.DMGTypesEmum.Lightning, Enums.DMGTypesEmum.Bolt } };
        public static Attack ChiAttack => new Attack { Name = "Chi", Coefficient = 0.7f, DMGTypes = new List<Enums.DMGTypesEmum> { Enums.DMGTypesEmum.Skill, Enums.DMGTypesEmum.Physical, Enums.DMGTypesEmum.Chi } };
        public static Attack IcySpikeAttack => new Attack { Name = "Icy Spike", Coefficient = 0.3f, DMGTypes = new List<Enums.DMGTypesEmum> { Enums.DMGTypesEmum.Skill, Enums.DMGTypesEmum.Ice } };
        public static Attack PoisonDotAttack => new Attack { Name = "Poison DoT", Coefficient = 0.2f, DMGTypes = new List<Enums.DMGTypesEmum> { Enums.DMGTypesEmum.Poison, Enums.DMGTypesEmum.DoT } };
        public static Attack BurnDotAttack => new Attack { Name = "Burn DoT", Coefficient = 0.3f, DMGTypes = new List<Enums.DMGTypesEmum> { Enums.DMGTypesEmum.Fire, Enums.DMGTypesEmum.DoT } };
        public static Attack LightSpearAttack => new Attack { Name = "Light Spear", Coefficient = 0.3f, DMGTypes = new List<Enums.DMGTypesEmum> { Enums.DMGTypesEmum.Skill, Enums.DMGTypesEmum.Physical, Enums.DMGTypesEmum.LightSpear } };

        public static Attack CustomAttack(float coeff, List<Enums.DMGTypesEmum> dmgTypes) => new Attack { Name = "Custom", Coefficient = coeff, DMGTypes = dmgTypes };
    }
}
