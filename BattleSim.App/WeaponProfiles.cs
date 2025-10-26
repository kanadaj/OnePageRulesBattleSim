using BattleSim.Core.Models;
using BattleSim.Core.Rules;
using BattleSim.Core.Rules.Weapons;
using BattleSim.Core.Simulation;

namespace BattleSim.App;

public static class WeaponProfiles
{
    
    public static readonly WeaponProfile HandWeapon = new WeaponProfile(
        name: "Hand Weapon",
        attacksPerModel: 1,
        armorPenetration: 0
    );
    
    public static readonly WeaponProfile DualHandWeapon = new WeaponProfile(
        name: "Dual Hand Weapon",
        attacksPerModel: 2,
        armorPenetration: 0
    );


    public static readonly WeaponProfile HeavyHandWeapon = new WeaponProfile(
        name: "Heavy Hand Weapon",
        attacksPerModel: 1,
        armorPenetration: 1
    );

    public static WeaponProfile HeavyGreatWeapon(int attacks = 1) => new WeaponProfile(
        name: "Heavy Great Weapon",
        attacksPerModel: attacks,
        armorPenetration: 3
    );


    public static WeaponProfile GreatWeapon(int attacks = 1) => new WeaponProfile(
        name: "Great Weapon",
        attacksPerModel: attacks,
        armorPenetration: 2
    );

    public static readonly WeaponProfile GreatAxe = new WeaponProfile(
        name: "Great Axe",
        attacksPerModel: 2,
        armorPenetration: 1
    );

    public static readonly WeaponProfile Halberd = new WeaponProfile(
        name: "Halberd",
        attacksPerModel: 1,
        armorPenetration: 0,
        afterHitRules: new IAfterHitRule[] { new RendingRule() }
    );

    public static readonly WeaponProfile HeavyHalberd = new WeaponProfile(
        name: "Heavy Halberd",
        attacksPerModel: 1,
        armorPenetration: 1,
        afterHitRules: new IAfterHitRule[] { new RendingRule() }
    );
    
    public static readonly WeaponProfile Spear = new WeaponProfile(
        name: "Spear",
        attacksPerModel: 1,
        armorPenetration: 0,
        beforeHitRules: new IBeforeHitOffensiveRule[] { new CounterRule() }
    );

    public static WeaponProfile Impact(int hits = 1) => new WeaponProfile(
        name: "Impact",
        attacksPerModel: hits,
        armorPenetration: 0,
        beforeHitRules: new IBeforeHitOffensiveRule[] { new ReliableRule() }
    );

    public static WeaponProfile HeavyImpact(int hits = 1, int armorPenetration = 1) => new WeaponProfile(
        name: "Impact",
        attacksPerModel: hits,
        armorPenetration: armorPenetration,
        beforeHitRules: new IBeforeHitOffensiveRule[] { new ReliableRule() }
    );
    
    public static readonly WeaponProfile Lance = new WeaponProfile(
        name: "Lance",
        attacksPerModel: 1,
        armorPenetration: 0,
        beforeHitRules: new IBeforeHitOffensiveRule[] { new ThrustRule() }
    );
    
    public static readonly WeaponProfile HeavyLance = new WeaponProfile(
        name: "Heavy Lance",
        attacksPerModel: 1,
        armorPenetration: 1,
        beforeHitRules: new IBeforeHitOffensiveRule[] { new ThrustRule() }
    );

    public static readonly WeaponProfile HighbornLance = new WeaponProfile(
        name: "Highborn Lance",
        attacksPerModel: 1,
        armorPenetration: 1,
        beforeHitRules: new IBeforeHitOffensiveRule[] { new ThrustRule(), new CavalryCapRule() }
    );
    
    public static readonly WeaponProfile Claws = new WeaponProfile(
        name: "Claws",
        attacksPerModel: 2,
        armorPenetration: 0
    );
    
    public static readonly WeaponProfile IceClaws = new WeaponProfile(
        name: "Ice Claws",
        attacksPerModel: 6,
        armorPenetration: 1
    );
    
    public static readonly WeaponProfile FlameClaws = new WeaponProfile(
        name: "Flame Claws",
        attacksPerModel: 4,
        armorPenetration: 0,
        beforeHitRules: new IBeforeHitOffensiveRule[] { new ReliableRule() },
        afterHitRules: new IAfterHitRule[] { new RendingRule() }
    );
    
    public static readonly WeaponProfile RendingClaws = new WeaponProfile(
        name: "Rending Claws",
        attacksPerModel: 3,
        afterHitRules: new IAfterHitRule[] { new RendingRule() }
    );
    
    public static WeaponProfile BreathAttack(int blast = 3) => new WeaponProfile(
        name: "Breath Attack",
        attacksPerModel: 1,
        armorPenetration: 1,
        beforeHitRules: new IBeforeHitOffensiveRule[] { new ReliableRule() },
        afterHitRules: new IAfterHitRule[] { new BlastRule(blast) }
    );

    public static readonly WeaponProfile Greathammer = new WeaponProfile(
        name: "Greathammer",
        attacksPerModel: 6,
        armorPenetration: 2
    );

    public static readonly WeaponProfile DualHammer = new WeaponProfile(
        name: "Dual Hammer",
        attacksPerModel: 8,
        armorPenetration: 1
    );

    public static WeaponProfile Longbow(int attacks = 1) => new WeaponProfile(
        name: "Longbow",
        attacksPerModel: attacks,
        armorPenetration: 0,
        isRanged: true
    );

    public static WeaponProfile MasterCraftedBow(int attacks = 1) => new WeaponProfile(
        name: "Master-Crafted Bow",
        attacksPerModel: attacks,
        isRanged: true,
        afterHitRules: new IAfterHitRule[] { new CrackRule() }
    );


    public static readonly WeaponProfile GatlingGun = new WeaponProfile(
        name: "Gatling Gun",
        attacksPerModel: 4,
        armorPenetration: 1,
        isRanged: true
    );


    public static readonly WeaponProfile Flamethrower = new WeaponProfile(
        name: "Flamethrower",
        attacksPerModel: 1,
        armorPenetration: 0,
        beforeHitRules: new IBeforeHitOffensiveRule[] { new ReliableRule() },
        afterHitRules: new IAfterHitRule[] { new BlastRule(3) },
        isRanged: true
    );

    public static readonly WeaponProfile ToxinMortar = new WeaponProfile(
        name: "Flamethrower",
        attacksPerModel: 1,
        armorPenetration: 0,
        afterHitRules: new IAfterHitRule[] { new BlastRule(3), new BaneRule() },
        isRanged: true
    );

    public static readonly WeaponProfile OvertunedGatlingGun = new WeaponProfile(
        name: "Overtuned Gatling Gun",
        attacksPerModel: 4,
        armorPenetration: 4,
        afterHitRules: new IAfterHitRule[] { new OvertunedRule() },
        isRanged: true
    );

    public static readonly WeaponProfile DrillFist = new WeaponProfile(
        name: "Drill Fist",
        attacksPerModel: 1,
        armorPenetration: 4,
        afterDefenseRules: new IAfterDefenseRule[] { new DeadlyRule(3) }
    );

    public static readonly WeaponProfile ShockFist = new WeaponProfile(
        name: "Shock Fist",
        attacksPerModel: 4,
        armorPenetration: 1,
        afterHitRules: new IAfterHitRule[] { new RendingRule() }
    );

    public static readonly WeaponProfile RollerFist = new WeaponProfile(
        name: "Roller Fist",
        attacksPerModel: 4,
        armorPenetration: 2,
        beforeHitRules: new IBeforeHitOffensiveRule[] { new ThrustRule() }
    );

    public static readonly WeaponProfile WarpstoneBlades = new WeaponProfile(
        name: "Warpstone Blades",
        attacksPerModel: 3,
        armorPenetration: 1,
        afterHitRules: new IAfterHitRule[] { new UnpredictableFighterRule() }
    );

    public static WeaponProfile Claw(int attacks = 10) => new WeaponProfile(
        name: "Claw",
        attacksPerModel: attacks
    );

    public static WeaponProfile GiantClaw(int attacks = 12) => new WeaponProfile(
        name: "Giant Claw",
        attacksPerModel: attacks,
        armorPenetration: 1
    );

    public static WeaponProfile RendingClaw(int attacks = 6) => new WeaponProfile(
        name: "Claw",
        attacksPerModel: attacks,
        afterHitRules: new IAfterHitRule[] { new RendingRule() }
    );

    public static WeaponProfile Stomp(int attacks = 4) => new WeaponProfile(
        name: "Stomp",
        attacksPerModel: attacks,
        armorPenetration: 1
    );

    public static WeaponProfile HeavyStomp(int attacks = 6) => new WeaponProfile(
        name: "Stomp",
        attacksPerModel: attacks,
        armorPenetration: 2
    );

    public static WeaponProfile CrossingBarrage(int attacks = 1) => new WeaponProfile(
        name: "Crossing Barrage",
        attacksPerModel: attacks,
        armorPenetration: 1,
        afterHitRules: new IAfterHitRule[] { new MultiplyHitsRule() }
    );

    public static WeaponProfile SwarmAttack(int attacks = 3) => new WeaponProfile(
        name: "Swarm Attack",
        attacksPerModel: attacks,
        armorPenetration: 0
    );
}