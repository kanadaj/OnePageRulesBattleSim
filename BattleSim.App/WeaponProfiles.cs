using BattleSim.Core.Models;
using BattleSim.Core.Rules.Weapons;

namespace BattleSim.App;

public static class WeaponProfiles
{
    public static readonly WeaponProfile HandWeapon = new(
        name: "Hand Weapon",
        attacksPerModel: 1,
        armorPenetration: 0
    );

    public static readonly WeaponProfile DualHandWeapon = new(
        name: "Dual Hand Weapon",
        attacksPerModel: 2,
        armorPenetration: 0
    );


    public static readonly WeaponProfile HeavyHandWeapon = new(
        name: "Heavy Hand Weapon",
        attacksPerModel: 1,
        armorPenetration: 1
    );

    public static WeaponProfile HeavyGreatWeapon(int attacks = 1) => new(
        name: "Heavy Great Weapon",
        attacksPerModel: attacks,
        armorPenetration: 3
    );


    public static WeaponProfile GreatWeapon(int attacks = 1) => new(
        name: "Great Weapon",
        attacksPerModel: attacks,
        armorPenetration: 2
    );

    public static readonly WeaponProfile GreatAxe = new(
        name: "Great Axe",
        attacksPerModel: 2,
        armorPenetration: 1
    );

    public static readonly WeaponProfile Halberd = new(
        name: "Halberd",
        attacksPerModel: 1,
        armorPenetration: 0,
        rules: [new RendingOffensiveRule()]
    );

    public static readonly WeaponProfile HeavyHalberd = new(
        name: "Heavy Halberd",
        attacksPerModel: 1,
        armorPenetration: 1,
        rules: [new RendingOffensiveRule()]
    );

    public static readonly WeaponProfile Spear = new(
        name: "Spear",
        attacksPerModel: 1,
        armorPenetration: 0,
        rules: [new CounterRule()]
    );

    public static WeaponProfile Impact(int hits = 1) => new(
        name: "Impact",
        attacksPerModel: hits,
        armorPenetration: 0,
        rules: [new ReliableRule()]
    );

    public static WeaponProfile HeavyImpact(int hits = 1, int armorPenetration = 1) => new(
        name: "Impact",
        attacksPerModel: hits,
        armorPenetration: armorPenetration,
        rules: [new ReliableRule()]
    );

    public static readonly WeaponProfile Lance = new(
        name: "Lance",
        attacksPerModel: 1,
        armorPenetration: 0,
        rules: [new ThrustRule()]
    );

    public static readonly WeaponProfile HeavyLance = new(
        name: "Heavy Lance",
        attacksPerModel: 1,
        armorPenetration: 1,
        rules: [new ThrustRule()]
    );

    public static readonly WeaponProfile HighbornLance = new(
        name: "Highborn Lance",
        attacksPerModel: 1,
        armorPenetration: 1,
        rules: [new ThrustRule(), new CavalryCapRule()]
    );

    public static readonly WeaponProfile Claws = new(
        name: "Claws",
        attacksPerModel: 2,
        armorPenetration: 0
    );

    public static readonly WeaponProfile IceClaws = new(
        name: "Ice Claws",
        attacksPerModel: 6,
        armorPenetration: 1
    );

    public static readonly WeaponProfile FlameClaws = new(
        name: "Flame Claws",
        attacksPerModel: 4,
        armorPenetration: 0,
        rules: [new ReliableRule(), new RendingOffensiveRule()]
    );

    public static readonly WeaponProfile RendingClaws = new(
        name: "Rending Claws",
        attacksPerModel: 3,
        rules: [new RendingOffensiveRule()]
    );

    public static WeaponProfile BreathAttack(int blast = 3) => new(
        name: "Breath Attack",
        attacksPerModel: 1,
        armorPenetration: 1,
        rules: [new ReliableRule(), new BlastOffensiveRule(blast)]
    );

    public static readonly WeaponProfile Greathammer = new(
        name: "Greathammer",
        attacksPerModel: 6,
        armorPenetration: 2
    );

    public static readonly WeaponProfile DualHammer = new(
        name: "Dual Hammer",
        attacksPerModel: 8,
        armorPenetration: 1
    );

    public static WeaponProfile Longbow(int attacks = 1) => new(
        name: "Longbow",
        attacksPerModel: attacks,
        armorPenetration: 0,
        isRanged: true
    );

    public static WeaponProfile MasterCraftedBow(int attacks = 1) => new(
        name: "Master-Crafted Bow",
        attacksPerModel: attacks,
        isRanged: true,
        rules: [new CrackOffensiveRule()]
    );


    public static readonly WeaponProfile GatlingGun = new(
        name: "Gatling Gun",
        attacksPerModel: 4,
        armorPenetration: 1,
        isRanged: true
    );


    public static readonly WeaponProfile Flamethrower = new(
        name: "Flamethrower",
        attacksPerModel: 1,
        armorPenetration: 0,
        rules: [new ReliableRule(), new BlastOffensiveRule(3)],
        isRanged: true
    );

    public static readonly WeaponProfile ToxinMortar = new(
        name: "Flamethrower",
        attacksPerModel: 1,
        armorPenetration: 0,
        rules: [new BlastOffensiveRule(3), new BaneOffensiveRule()],
        isRanged: true
    );

    public static readonly WeaponProfile OvertunedGatlingGun = new(
        name: "Overtuned Gatling Gun",
        attacksPerModel: 4,
        armorPenetration: 4,
        rules: [new OvertunedOffensiveRule()],
        isRanged: true
    );

    public static readonly WeaponProfile DrillFist = new(
        name: "Drill Fist",
        attacksPerModel: 1,
        armorPenetration: 4,
        rules: [new DeadlyRule(3)]
    );

    public static readonly WeaponProfile ShockFist = new(
        name: "Shock Fist",
        attacksPerModel: 4,
        armorPenetration: 1,
        rules: [new RendingOffensiveRule()]
    );

    public static readonly WeaponProfile RollerFist = new(
        name: "Roller Fist",
        attacksPerModel: 4,
        armorPenetration: 2,
        rules: [new ThrustRule()]
    );

    public static readonly WeaponProfile WarpstoneBlades = new(
        name: "Warpstone Blades",
        attacksPerModel: 3,
        armorPenetration: 1,
        rules: [new UnpredictableFighterOffensiveRule()]
    );

    public static readonly WeaponProfile MeteorHammers = new(
        name: "Meteor Hammer",
        attacksPerModel: 4,
        armorPenetration: 2
    );

    public static WeaponProfile Claw(int attacks = 10) => new(
        name: "Claw",
        attacksPerModel: attacks
    );

    public static WeaponProfile GiantClaw(int attacks = 12) => new(
        name: "Giant Claw",
        attacksPerModel: attacks,
        armorPenetration: 1
    );

    public static WeaponProfile RendingClaw(int attacks = 6) => new(
        name: "Claw",
        attacksPerModel: attacks,
        rules: [new RendingOffensiveRule()]
    );

    public static WeaponProfile Stomp(int attacks = 4) => new(
        name: "Stomp",
        attacksPerModel: attacks,
        armorPenetration: 1
    );

    public static WeaponProfile HeavyStomp(int attacks = 6) => new(
        name: "Stomp",
        attacksPerModel: attacks,
        armorPenetration: 2
    );

    public static WeaponProfile CrossingBarrage(int attacks = 1) => new(
        name: "Crossing Barrage",
        attacksPerModel: attacks,
        armorPenetration: 1,
        rules: [new MultiplyHitsOffensiveRule()]
    );

    public static WeaponProfile SwarmAttack(int attacks = 3) => new(
        name: "Swarm Attack",
        attacksPerModel: attacks,
        armorPenetration: 0
    );
}