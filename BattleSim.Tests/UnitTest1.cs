using BattleSim.App.UnitProfiles;
using BattleSim.Core.Models;
using BattleSim.Core.Rules;
using BattleSim.Core.Rules.Units;
using BattleSim.Core.Rules.Weapons;
using BattleSim.Core.Simulation;

namespace BattleSim.Tests;

public class BattleSimulatorTests
{
    [Fact]
    public void SingleAttack_NoRetaliation_ComputesExpectedValues()
    {
        var attacker = new UnitProfile(
            name: "Hero",
            quality: 2,
            defense: 7,
            toughness: 1,
            fear: 0,
            modelCount: 1,
            weapons: [new WeaponProfile("Sword", 1)]);

        var defender = new UnitProfile(
            name: "Goblin",
            quality: 5,
            defense: 7,
            toughness: 1,
            fear: 0,
            modelCount: 1,
            weapons: []);

        var simulator = new BattleSimulator();
        var result = simulator.Simulate(attacker, defender);

        var expectedWipeProbability = 25d / 36d;
        var expectedTieProbability = 11d / 36d;

        Assert.Equal(expectedWipeProbability, result.AverageWoundsToUnitB, precision: 6);
        Assert.Equal(0d, result.AverageWoundsToUnitA, precision: 6);
        Assert.Equal(expectedWipeProbability, result.ProbabilityUnitADestroysUnitB, precision: 6);
        Assert.Equal(expectedTieProbability, result.ProbabilityOfTie, precision: 6);
        Assert.Equal(expectedWipeProbability, result.ProbabilityUnitAWins, precision: 6);
        Assert.Equal(0d, result.ProbabilityUnitBWins, precision: 6);
        Assert.Equal(expectedWipeProbability, result.AverageModelsLostByUnitB, precision: 6);
    }

    [Fact]
    public void FearAffectsResolutionWithoutChangingCasualties()
    {
        var fearless = new UnitProfile(
            name: "Skeleton",
            quality: 4,
            defense: 4,
            toughness: 2,
            fear: 0,
            modelCount: 10,
            weapons: []);

        var terrifying = new UnitProfile(
            name: "Wraith",
            quality: 3,
            defense: 3,
            toughness: 3,
            fear: 2,
            modelCount: 5,
            weapons: []);

        var simulator = new BattleSimulator();
        var result = simulator.Simulate(terrifying, fearless);

        Assert.Equal(0d, result.AverageWoundsToUnitA, precision: 6);
        Assert.Equal(0d, result.AverageWoundsToUnitB, precision: 6);
        Assert.Equal(1d, result.ProbabilityUnitAWins, precision: 6);
        Assert.Equal(0d, result.ProbabilityUnitBWins, precision: 6);
        Assert.Equal(0d, result.ProbabilityUnitADestroysUnitB, precision: 6);
        Assert.Equal(0d, result.ProbabilityUnitBDestroysUnitA, precision: 6);
        Assert.Equal(0d, result.ProbabilityOfTie, precision: 6);
    }

    [Fact]
    public void ExplodingSixesAddsAutomaticWounds()
    {
        var explosiveWeapon = new WeaponProfile(
            name: "Arcane Bolt",
            attacksPerModel: 2,
            armorPenetration: 1,
            rules: [new ExplodingSixesOffensiveRule()]);

        var mage = new UnitProfile(
            name: "Mage",
            quality: 3,
            defense: 6,
            toughness: 2,
            fear: 1,
            modelCount: 1,
            weapons: [explosiveWeapon]);

        var knight = new UnitProfile(
            name: "Knight",
            quality: 3,
            defense: 3,
            toughness: 2,
            fear: 0,
            modelCount: 1,
            weapons: [new WeaponProfile("Lance", 2)]);

        var simulator = new BattleSimulator();
        var result = simulator.Simulate(mage, knight);

        Assert.True(result.AverageWoundsToUnitB > 0d);
        Assert.True(result.AverageModelsLostByUnitB > 0d);
        Assert.True(result.ProbabilityUnitADestroysUnitB > 0d);
        Assert.True(result.ProbabilityUnitBDestroysUnitA < 1d);
    }

    [Fact]
    public void EvasionReducesIncomingHits()
    {
    var axe = new WeaponProfile("Axe", 2);
        var attacker = new UnitProfile("Raider", 3, 5, 1, 0, 5, [axe]);

        var plainDefender = new UnitProfile("Guard", 4, 4, 1, 0, 5, []);
        var evasiveDefender = new UnitProfile(
            name: "Guard",
            quality: 4,
            defense: 4,
            toughness: 1,
            fear: 0,
            modelCount: 5,
            weapons: [],
            modelRules: [new EvasionRule()]);

        var simulator = new BattleSimulator();
        var baseline = simulator.Simulate(attacker, plainDefender);
        var evasive = simulator.Simulate(attacker, evasiveDefender);

        Assert.True(evasive.AverageWoundsToUnitB < baseline.AverageWoundsToUnitB);
    }

    [Fact]
    public void RegenerationCanBeSuppressedByBane()
    {
        var normalWeapon = new WeaponProfile("Blade", 2, 1);
        var baneWeapon = new WeaponProfile("Blade", 2, 1, rules: [new BaneOffensiveRule()]);

        var attackerNormal = new UnitProfile("Paladin", 3, 5, 2, 0, 1, [normalWeapon]);
        var attackerBane = new UnitProfile("Paladin", 3, 5, 2, 0, 1, [baneWeapon]);

        var defenderNoRegen = new UnitProfile("Troll", 5, 4, 3, 0, 1, []);
        var defenderRegen = new UnitProfile(
            name: "Troll",
            quality: 5,
            defense: 4,
            toughness: 3,
            fear: 0,
            modelCount: 1,
            weapons: [],
            modelRules: [new RegenerationRule()]);

        var simulator = new BattleSimulator();

        var baseline = simulator.Simulate(attackerNormal, defenderNoRegen);
        var regen = simulator.Simulate(attackerNormal, defenderRegen);
        var baselineBane = simulator.Simulate(attackerBane, defenderNoRegen);
        var suppressed = simulator.Simulate(attackerBane, defenderRegen);

        Assert.True(regen.AverageWoundsToUnitB < baseline.AverageWoundsToUnitB);
        Assert.Equal(baselineBane.AverageWoundsToUnitB, suppressed.AverageWoundsToUnitB, precision: 6);
    }

    [Fact]
    public void RendingIncreasesDamageThroughArmor()
    {
        var standard = new WeaponProfile("Halberd", 2);
        var rending = new WeaponProfile(
            name: "Halberd",
            attacksPerModel: 2,
            rules: [new RendingOffensiveRule()]);

        var attackerStandard = new UnitProfile("Guard", 3, 5, 1, 0, 3, [standard]);
        var attackerRending = new UnitProfile("Guard", 3, 5, 1, 0, 3, [rending]);

        var defender = new UnitProfile("Armored Orc", 4, 4, 2, 0, 3, []);

        var simulator = new BattleSimulator();
        var baseline = simulator.Simulate(attackerStandard, defender);
        var enhanced = simulator.Simulate(attackerRending, defender);

        Assert.True(enhanced.AverageWoundsToUnitB > baseline.AverageWoundsToUnitB);
    }

    [Fact]
    public void SlayerAndReapAdjustArmorPenetrationWhenEligible()
    {
        var weapon = new WeaponProfile(
            name: "Polearm",
            attacksPerModel: 2,
            rules: new IBeforeHitOffensiveRule[] { new SlayerRule(), new ReapRule() });

        var attacker = new UnitProfile("Halberdier", 3, 5, 1, 0, 3, [weapon]);

        var eliteTarget = new UnitProfile("Ogre", 4, 3, 3, 0, 2, []);
        var regularTarget = new UnitProfile("Soldier", 4, 4, 2, 0, 2, []);

        var simulator = new BattleSimulator();
        var versusElite = simulator.Simulate(attacker, eliteTarget);
        var versusRegular = simulator.Simulate(attacker, regularTarget);

        Assert.True(versusElite.AverageWoundsToUnitB > versusRegular.AverageWoundsToUnitB);
    }

    [Fact]
    public void FuriousAndThrustBenefitChargingAttacks()
    {
        var lancerWeapon = new WeaponProfile(
            name: "Lance",
            attacksPerModel: 2,
            rules: [new ThrustRule(), new FuriousOffensiveRule()]);

        var cavalry = new UnitProfile("Cavalry", 3, 4, 2, 0, 3, [lancerWeapon]);
        var infantry = new UnitProfile("Spearman", 4, 4, 1, 0, 6, []);

        var simulator = new BattleSimulator();

        var charging = simulator.Simulate(cavalry, infantry);
        var defending = simulator.Simulate(infantry, cavalry);

        Assert.True(charging.AverageWoundsToUnitB > defending.AverageWoundsToUnitA);
    }

    [Fact]
    public void CounterAllowsDefenderToStrikeFirst()
    {
    var axe = new WeaponProfile("Axe", 2);
        var attacker = new UnitProfile("Barbarian", 3, 5, 1, 0, 4, [axe]);

    var spear = new WeaponProfile("Spear", 1);
    var counterSpear = new WeaponProfile("Spear", 1, rules: new IBeforeHitOffensiveRule[] { new CounterRule() });

        var defenderNormal = new UnitProfile("Guard", 4, 4, 1, 0, 4, [spear]);
        var defenderCounter = new UnitProfile("Guard", 4, 4, 1, 0, 4, [counterSpear]);

        var simulator = new BattleSimulator();

        var normal = simulator.Simulate(attacker, defenderNormal);
        var counter = simulator.Simulate(attacker, defenderCounter);

        Assert.True(counter.AverageWoundsToUnitA > normal.AverageWoundsToUnitA);
        Assert.True(counter.AverageWoundsToUnitB < normal.AverageWoundsToUnitB);
    }

    [Fact]
    public void ReliableIgnoresEvasionPenalty()
    {
    var reliableWeapon = new WeaponProfile("Repeater", 3, rules: new IBeforeHitOffensiveRule[] { new ReliableRule() });
        var shooter = new UnitProfile("Marksman", 4, 5, 1, 0, 2, [reliableWeapon]);

        var plainTarget = new UnitProfile("Scout", 4, 4, 1, 0, 3, []);
        var evasiveTarget = new UnitProfile(
            name: "Scout",
            quality: 4,
            defense: 4,
            toughness: 1,
            fear: 0,
            modelCount: 3,
            weapons: [],
            modelRules: [new EvasionRule()]);

        var simulator = new BattleSimulator();
        var baseline = simulator.Simulate(shooter, plainTarget);
        var versusEvasion = simulator.Simulate(shooter, evasiveTarget);

        Assert.Equal(baseline.AverageWoundsToUnitB, versusEvasion.AverageWoundsToUnitB, precision: 6);
    }

    [Fact]
    public void PreciseImprovesHitChance()
    {
    var basicWeapon = new WeaponProfile("Bow", 2);
    var preciseWeapon = new WeaponProfile("Bow", 2, rules: new IBeforeHitOffensiveRule[] { new PreciseRule() });

        var archerBasic = new UnitProfile("Archer", 4, 5, 1, 0, 2, [basicWeapon]);
        var archerPrecise = new UnitProfile("Archer", 4, 5, 1, 0, 2, [preciseWeapon]);

        var defender = new UnitProfile("Target", 5, 5, 1, 0, 3, []);

        var simulator = new BattleSimulator();
        var baseline = simulator.Simulate(archerBasic, defender);
        var enhanced = simulator.Simulate(archerPrecise, defender);

        Assert.True(enhanced.AverageWoundsToUnitB > baseline.AverageWoundsToUnitB);
    }

    [Fact]
    public void OvertunedWeaponsCauseSelfDamage()
    {
        var standardWeapon = new WeaponProfile("Arc Rifle", 3, 1);
        var overtunedWeapon = new WeaponProfile(
            name: "Arc Rifle",
            attacksPerModel: 3,
            armorPenetration: 1,
            rules: [new OvertunedOffensiveRule()]);

        var standardShooter = new UnitProfile("Engineer", 3, 5, 2, 0, 2, [standardWeapon]);
        var overtunedShooter = new UnitProfile("Engineer", 3, 5, 2, 0, 2, [overtunedWeapon]);

        var target = new UnitProfile("Dummy", 5, 5, 1, 0, 3, []);

        var simulator = new BattleSimulator();
        var baseline = simulator.Simulate(standardShooter, target);
        var overtuned = simulator.Simulate(overtunedShooter, target);

        Assert.Equal(0d, baseline.AverageWoundsToUnitA, precision: 6);
        Assert.True(overtuned.AverageWoundsToUnitA > 0d);
        Assert.Equal(baseline.AverageWoundsToUnitB, overtuned.AverageWoundsToUnitB, precision: 6);
    }

    [Fact]
    public void CavalryCapLimitsChargingModels()
    {
        var uncappedWeapon = new WeaponProfile("Lance", 2, 1);
        var cappedWeapon = new WeaponProfile(
            name: "Lance",
            attacksPerModel: 2,
            armorPenetration: 1,
            rules: new IBeforeHitOffensiveRule[] { new CavalryCapRule() });

        var uncappedCavalry = new UnitProfile("Knights", 3, 4, 2, 0, 10, [uncappedWeapon]);
        var cappedCavalry = new UnitProfile("Knights", 3, 4, 2, 0, 10, [cappedWeapon]);

        var infantry = new UnitProfile("Infantry", 4, 4, 1, 0, 20, []);

        var simulator = new BattleSimulator();
        var uncapped = simulator.Simulate(uncappedCavalry, infantry);
        var capped = simulator.Simulate(cappedCavalry, infantry);

        Assert.True(capped.AverageWoundsToUnitB < uncapped.AverageWoundsToUnitB);
    }

    [Fact]
    public void CrossingBarrageAmplifiesDamage()
    {
        var baselineWeapon = new WeaponProfile(
            name: "Battery",
            attacksPerModel: 1,
            rules: new IBeforeHitOffensiveRule[] { new PreciseRule() });

        var crossingWeapon = new WeaponProfile(
            name: "Battery",
            attacksPerModel: 1,
            rules: [new PreciseRule(), new MultiplyHitsOffensiveRule()]);

        var baseline = new UnitProfile("Crew", 3, 6, 1, 0, 3, [baselineWeapon]);
        var enhanced = new UnitProfile("Crew", 3, 6, 1, 0, 3, [crossingWeapon]);

        var target = new UnitProfile("Siege Tower", 5, 4, 3, 0, 6, []);

        var simulator = new BattleSimulator();
        var baselineResult = simulator.Simulate(baseline, target);
        var enhancedResult = simulator.Simulate(enhanced, target);

        Assert.True(enhancedResult.AverageWoundsToUnitB > baselineResult.AverageWoundsToUnitB);
    }

    [Fact]
    public void RangedAttack_PreventsRetaliationAndAwardsWinOnDamage()
    {
        var rangedWeapon = new WeaponProfile(
            name: "Longbow",
            attacksPerModel: 1,
            rules: [new FixedDirectWoundsOffensiveRule(6)]);

        var attacker = new UnitProfile(
            name: "Archer Company",
            quality: 3,
            defense: 5,
            toughness: 1,
            fear: 0,
            modelCount: 10,
            weapons: [rangedWeapon]);

        var defender = new UnitProfile(
            name: "Shield Wall",
            quality: 4,
            defense: 4,
            toughness: 1,
            fear: 0,
            modelCount: 5,
            weapons: []);

        var simulator = new BattleSimulator();
        var result = simulator.Simulate(attacker, defender, SimulationMode.Ranged);

        Assert.Equal(0d, result.AverageWoundsToUnitA, precision: 6);
        Assert.True(result.AverageWoundsToUnitB > 0d);
        Assert.Equal(0d, result.ProbabilityUnitBWins, precision: 6);
        Assert.Equal(1d, result.ProbabilityUnitADestroysUnitB, precision: 6);
    }

    [Fact]
    public void RangedAttack_UsesHalfModelThresholdForElimination()
    {
        var softVolley = new WeaponProfile(
            name: "Volley",
            attacksPerModel: 1,
            rules: [new FixedDirectWoundsOffensiveRule(6)]);

        var decisiveVolley = new WeaponProfile(
            name: "Volley",
            attacksPerModel: 1,
            rules: [new FixedDirectWoundsOffensiveRule(8)]);

        var softAttacker = new UnitProfile(
            name: "Skirmishers",
            quality: 3,
            defense: 5,
            toughness: 1,
            fear: 0,
            modelCount: 5,
            weapons: [softVolley]);

        var decisiveAttacker = softAttacker with
        {
            Weapons = [decisiveVolley]
        };

        var defender = new UnitProfile(
            name: "Heavy Infantry",
            quality: 4,
            defense: 4,
            toughness: 1,
            fear: 0,
            modelCount: 8,
            weapons: []);

        var simulator = new BattleSimulator();
        var softResult = simulator.Simulate(softAttacker, defender, SimulationMode.Ranged);
        var decisiveResult = simulator.Simulate(decisiveAttacker, defender, SimulationMode.Ranged);

        Assert.Equal(0d, softResult.ProbabilityUnitADestroysUnitB, precision: 6);
        Assert.Equal(1d, decisiveResult.ProbabilityUnitADestroysUnitB, precision: 6);
    }

    [Fact]
    public void UnpredictableFighterAddsVarianceAndDamage()
    {
        var baselineWeapon = new WeaponProfile("Warp Blades", 3, 1);
        var unpredictableWeapon = new WeaponProfile(
            name: "Warp Blades",
            attacksPerModel: 3,
            armorPenetration: 1,
            rules: [new UnpredictableFighterOffensiveRule()]);

        var disciplined = new UnitProfile("Disciplined Guard", 3, 5, 1, 0, 5, [baselineWeapon]);
        var frenzied = new UnitProfile("Warp Frenzy", 3, 5, 1, 0, 5, [unpredictableWeapon]);

        var defender = new UnitProfile("Shield Wall", 4, 4, 2, 0, 8, []);

        var simulator = new BattleSimulator();
        var disciplinedResult = simulator.Simulate(disciplined, defender);
    var frenziedResult = simulator.Simulate(frenzied, defender);

    Assert.True(frenziedResult.AverageWoundsToUnitB > disciplinedResult.AverageWoundsToUnitB);
    Assert.True(frenziedResult.ProbabilityUnitADestroysUnitB >= disciplinedResult.ProbabilityUnitADestroysUnitB);
    }

    [Fact]
    public void HeroAddsOffensivePowerAndFear()
    {
        var baseline = HighElves.LionWarriors.GreatAxe;
        var chieftain = new HeroProfile(
            name: "Lion Chieftain",
            quality: 2,
            toughness: 2,
            fear: 1,
            defense: 3,
            weapons:
            [
                new WeaponProfile(
                    name: "Chieftain's Great Axe",
                    attacksPerModel: 3,
                    armorPenetration: 2,
                    rules: [new FuriousOffensiveRule()])
            ],
            auras: [new PiercingTagRule(1)]);

        var withHero = baseline.WithHero(chieftain);
        var target = Ratmen.GiantBeast;

        var simulator = new BattleSimulator();
        var baselineResult = simulator.Simulate(baseline, target);
        var heroResult = simulator.Simulate(withHero, target);

        Assert.True(heroResult.AverageWoundsToUnitB > baselineResult.AverageWoundsToUnitB);
        Assert.True(withHero.TotalFear > baseline.TotalFear);
        Assert.Equal(baseline.TotalWoundCapacity + withHero.Hero!.Toughness, withHero.TotalWoundCapacity);
    }

    [Fact]
    public void HeroDefensiveAuraShieldsTheUnit()
    {
        var attacker = Ratmen.StormVeterans.HeavyHalberd;
        var defenderBaseline = HighElves.PhoenixWarriors.HeavyHalberd;
        var highPriest = new HeroProfile(
            name: "Phoenix High Priest",
            quality: 3,
            toughness: 2,
            fear: 1,
            defense: 3,
            weapons:
            [
                new WeaponProfile("Sacred Flame", 2, armorPenetration: 1)
            ],
            auras: [new ShieldRule(), new RegenerationRule()]);

        var defenderBlessed = defenderBaseline.WithHero(highPriest);

        var simulator = new BattleSimulator();
        var baseline = simulator.Simulate(attacker, defenderBaseline);
        var blessed = simulator.Simulate(attacker, defenderBlessed);

        Assert.True(blessed.AverageWoundsToUnitB < baseline.AverageWoundsToUnitB);
        Assert.True(blessed.AverageWoundsToUnitA >= baseline.AverageWoundsToUnitA);
    }

    [Fact]
    public void HeroMatchupsProduceValidOutcomeDistributions()
    {
        var chieftain = new HeroProfile(
            name: "Lion Chieftain",
            quality: 2,
            toughness: 2,
            fear: 1,
            defense: 3,
            weapons:
            [
                new WeaponProfile("Chieftain's Great Axe", 3, armorPenetration: 2)
            ]);

        var packMaster = new HeroProfile(
            name: "Pack Master",
            quality: 3,
            toughness: 3,
            defense: 3,
            weapons:
            [
                new WeaponProfile("Shock Prod", 2, armorPenetration: 1)
            ],
            auras: [new ReliableRule()]);

        var simulator = new BattleSimulator();
        var result = simulator.Simulate(
            HighElves.LionWarriors.GreatAxe.AsCombinedUnit.WithHero(chieftain),
            Ratmen.RatOgres.RendingClaws.AsCombinedUnit.WithHero(packMaster));

        Assert.True(result.AverageWoundsToUnitA >= 0d);
        Assert.True(result.AverageWoundsToUnitB >= 0d);
        Assert.True(Math.Abs(result.ProbabilityUnitAWins + result.ProbabilityUnitBWins + result.ProbabilityOfTie - 1d) < 1e-6);
    }

    private sealed class ExplodingSixesOffensiveRule : IAfterHitOffensiveRule
    {
        public void Apply(AfterHitContext context)
        {
            context.Map(state => state.AddDirectWounds(state.NaturalSixes));
        }

        public string Name => "Exploding Sixes";
    }

    private sealed class FixedDirectWoundsOffensiveRule : IAfterHitOffensiveRule
    {
        private readonly int _directWounds;
        private readonly int _selfWounds;

        public FixedDirectWoundsOffensiveRule(int directWounds, int selfWounds = 0)
        {
            _directWounds = directWounds;
            _selfWounds = selfWounds;
        }

        public void Apply(AfterHitContext context)
        {
            context.Map(_ => new HitState(0, 0, 0, _directWounds, _selfWounds));
        }

        public string Name => "Fixed Direct Wounds";
    }
}
