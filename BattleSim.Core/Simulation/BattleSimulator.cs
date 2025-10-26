using System;
using System.Collections.Generic;
using System.Linq;
using BattleSim.Core.Models;
using BattleSim.Core.Rules;
using BattleSim.Core.Rules.Weapons;

namespace BattleSim.Core.Simulation;

/// <summary>
/// Provides deterministic probability calculations for battles between two units.
/// </summary>
public sealed class BattleSimulator
{
    /// <summary>
    /// Simulates a single round of combat, allowing weapon special rules to adjust initiative order.
    /// </summary>
    public BattleResult Simulate(UnitProfile unitA, UnitProfile unitB, SimulationMode mode = SimulationMode.Melee)
    {
        if (unitA is null)
        {
            throw new ArgumentNullException(nameof(unitA));
        }

        if (unitB is null)
        {
            throw new ArgumentNullException(nameof(unitB));
        }

        return mode switch
        {
            SimulationMode.Melee => SimulateMelee(unitA, unitB),
            SimulationMode.Ranged => SimulateRanged(unitA, unitB),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }

    private static BattleResult SimulateMelee(UnitProfile unitA, UnitProfile unitB)
    {
        var unitAAttacksFirst = !UnitHasCounter(unitB);

        var firstAttacker = unitAAttacksFirst ? unitA : unitB;
        var secondAttacker = unitAAttacksFirst ? unitB : unitA;
        var firstIsCharging = unitAAttacksFirst;
        var secondIsCharging = !unitAAttacksFirst;

        var firstStrikeDistribution = ResolveUnitAttacks(
            firstAttacker,
            secondAttacker,
            firstAttacker.ModelCount,
            attackerHeroActive: firstAttacker.Hero is not null,
            defenderHeroActive: secondAttacker.Hero is not null,
            isCharging: firstIsCharging);
        var combinedOutcomes = new Dictionary<BattleOutcome, double>();

        foreach (var (firstDamage, firstProbability) in firstStrikeDistribution.Probabilities)
        {
            if (firstProbability <= 0d)
            {
                continue;
            }

            var defendersRemainingModels = RemainingRegularModels(secondAttacker, firstDamage.DefenderWounds);
            var defenderHeroSurvives = HeroSurvives(secondAttacker, firstDamage.DefenderWounds);

            var retaliationDistribution = ResolveUnitAttacks(
                secondAttacker,
                firstAttacker,
                defendersRemainingModels,
                attackerHeroActive: secondAttacker.Hero is not null && defenderHeroSurvives,
                defenderHeroActive: firstAttacker.Hero is not null,
                isCharging: secondIsCharging);

            foreach (var (secondDamage, secondProbability) in retaliationDistribution.Probabilities)
            {
                var probability = firstProbability * secondProbability;
                if (probability <= 0d)
                {
                    continue;
                }

                var woundsByUnitA = unitAAttacksFirst ? firstDamage.DefenderWounds : secondDamage.DefenderWounds;
                var woundsByUnitB = unitAAttacksFirst ? secondDamage.DefenderWounds : firstDamage.DefenderWounds;

                var selfWoundsForUnitA = unitAAttacksFirst ? firstDamage.SelfInflictedWounds : secondDamage.SelfInflictedWounds;
                var selfWoundsForUnitB = unitAAttacksFirst ? secondDamage.SelfInflictedWounds : firstDamage.SelfInflictedWounds;

                var scoreA = woundsByUnitA + unitA.TotalFear;
                var scoreB = woundsByUnitB + unitB.TotalFear;
                var outcome = new BattleOutcome(
                    WoundsToUnitA: woundsByUnitB + selfWoundsForUnitA,
                    WoundsToUnitB: woundsByUnitA + selfWoundsForUnitB,
                    ScoreForUnitA: scoreA,
                    ScoreForUnitB: scoreB,
                    HitsForUnitA: unitAAttacksFirst ? secondDamage.Hits : firstDamage.Hits,
                    HitsForUnitB: unitAAttacksFirst ? firstDamage.Hits : secondDamage.Hits);

                if (combinedOutcomes.TryGetValue(outcome, out var current))
                {
                    combinedOutcomes[outcome] = current + probability;
                }
                else
                {
                    combinedOutcomes[outcome] = probability;
                }
            }
        }

        var outcomeDistribution = new ProbabilityDistribution<BattleOutcome>(combinedOutcomes);

        return BuildBattleResult(
            unitA,
            unitB,
            outcomeDistribution,
            unitBEliminationCondition: wounds => wounds >= unitB.TotalWoundCapacity,
            unitAEliminationCondition: wounds => wounds >= unitA.TotalWoundCapacity);
    }

    private static BattleResult SimulateRanged(UnitProfile attacker, UnitProfile defender)
    {
        var attackDistribution = ResolveUnitAttacks(
            attacker,
            defender,
            attacker.ModelCount,
            attackerHeroActive: attacker.Hero is not null,
            defenderHeroActive: defender.Hero is not null,
            isCharging: false);

        var outcomes = new Dictionary<BattleOutcome, double>();

        foreach (var (damage, probability) in attackDistribution.Probabilities)
        {
            if (probability <= 0d)
            {
                continue;
            }

            var outcome = new BattleOutcome(
                WoundsToUnitA: damage.SelfInflictedWounds,
                WoundsToUnitB: damage.DefenderWounds,
                ScoreForUnitA: damage.DefenderWounds + attacker.TotalFear,
                ScoreForUnitB: defender.TotalFear,
                HitsForUnitA: 0,
                HitsForUnitB: damage.Hits);

            if (outcomes.TryGetValue(outcome, out var current))
            {
                outcomes[outcome] = current + probability;
            }
            else
            {
                outcomes[outcome] = probability;
            }
        }

        var distribution = new ProbabilityDistribution<BattleOutcome>(outcomes);

        return BuildBattleResult(
            attacker,
            defender,
            distribution,
            unitBEliminationCondition: wounds => wounds >= defender.TotalWoundCapacity,
            unitAEliminationCondition: wounds => wounds >= attacker.TotalWoundCapacity,
            unitAWinCondiiton: (scoreA, scoreB, woundsB) => MeetsRangedElimination(defender, woundsB),
            unitBWinCondition: (scoreA, scoreB, woundsA) => MeetsRangedElimination(attacker, woundsA),
            true);
    }

    private static BattleResult BuildBattleResult(
        UnitProfile unitA,
        UnitProfile unitB,
        ProbabilityDistribution<BattleOutcome> outcomeDistribution,
        Func<int, bool> unitBEliminationCondition,
        Func<int, bool> unitAEliminationCondition,
        Func<int, int, int, bool>? unitAWinCondiiton = null,
        Func<int, int, int, bool>? unitBWinCondition = null,
        bool ranged = false)
    {
        var averageWoundsToA = outcomeDistribution.Expectation(o => o.WoundsToUnitA);
        var averageWoundsToB = outcomeDistribution.Expectation(o => o.WoundsToUnitB);

        var averageModelsLostByA = outcomeDistribution.Expectation(o => ModelsLost(unitA, o.WoundsToUnitA));
        var averageModelsLostByB = outcomeDistribution.Expectation(o => ModelsLost(unitB, o.WoundsToUnitB));

        double probabilityUnitAWins = 0d;
        double probabilityUnitBWins = 0d;
        double probabilityOfTie = 0d;
        double probabilityAEliminatesB = 0d;
        double probabilityBEliminatesA = 0d;

        if (unitAWinCondiiton is null)
        {
            unitAWinCondiiton = (scoreA, scoreB,_) => scoreA > scoreB;
        }

        if (unitBWinCondition is null)
        {
            unitBWinCondition = (scoreA, scoreB, _) => scoreA < scoreB;
        }

        foreach (var (outcome, probability) in outcomeDistribution.Probabilities)
        {
            bool tie = !ranged;
            if (unitAWinCondiiton(outcome.ScoreForUnitA, outcome.ScoreForUnitB, outcome.WoundsToUnitB))
            {
                probabilityUnitAWins += probability;
                tie = false;
            }
            if (unitBWinCondition(outcome.ScoreForUnitA, outcome.ScoreForUnitB, outcome.WoundsToUnitA))
            {
                probabilityUnitBWins += probability;
                tie = ranged && !tie;
            }
            if (tie)
            {
                probabilityOfTie += probability;
            }

            if (unitBEliminationCondition(outcome.WoundsToUnitB))
            {
                probabilityAEliminatesB += probability;
            }

            if (unitAEliminationCondition(outcome.WoundsToUnitA))
            {
                probabilityBEliminatesA += probability;
            }
        }

        var alternativeOutcomeForA = probabilityUnitBWins;
        var alternativeOutcomeForB = probabilityUnitAWins;

        return new BattleResult(
            AverageWoundsToUnitA: averageWoundsToA,
            AverageWoundsToUnitB: averageWoundsToB,
            AverageModelsLostByUnitA: averageModelsLostByA,
            AverageModelsLostByUnitB: averageModelsLostByB,
            ProbabilityUnitAWins: probabilityUnitAWins,
            ProbabilityUnitBWins: probabilityUnitBWins,
            ProbabilityOfTie: probabilityOfTie,
            ProbabilityUnitADestroysUnitB: probabilityAEliminatesB,
            ProbabilityUnitBDestroysUnitA: probabilityBEliminatesA,
            AlternativeOutcomeProbabilityForUnitA: alternativeOutcomeForA,
            AlternativeOutcomeProbabilityForUnitB: alternativeOutcomeForB,
            OutcomeDistribution: outcomeDistribution.Probabilities,
            AverageHitsByUnitA: outcomeDistribution.Expectation(o => o.HitsForUnitA),
            AverageHitsByUnitB: outcomeDistribution.Expectation(o => o.HitsForUnitB));
    }

    private static bool MeetsRangedElimination(UnitProfile unit, int wounds)
    {
        if (wounds <= 0)
        {
            return false;
        }

        var totalModels = unit.ModelCount + (unit.Hero is not null ? 1 : 0);
        if (totalModels <= 0)
        {
            return false;
        }

        if (totalModels <= 1)
        {
            var baseToughness = unit.ModelCount > 0
                ? unit.Toughness
                : Math.Max(1, unit.Hero?.Toughness ?? unit.Toughness);
            var threshold = (int)Math.Ceiling(baseToughness / 2d);
            return wounds >= threshold;
        }

        var requiredModels = (int)Math.Ceiling(totalModels / 2d);
        var removedModels = CountModelsRemoved(unit, wounds);
        return removedModels >= requiredModels;
    }

    private static int CountModelsRemoved(UnitProfile unit, int wounds)
    {
        if (wounds <= 0)
        {
            return 0;
        }

        var toughnessPerModel = Math.Max(1, unit.Toughness);
        var regularCapacity = unit.RegularWoundCapacity;
        var regularRemoved = 0;

        if (regularCapacity > 0)
        {
            var regularWounds = Math.Min(wounds, regularCapacity);
            regularRemoved = Math.Min(unit.ModelCount, regularWounds / toughnessPerModel);
        }

        var heroRemoved = 0;
        if (unit.Hero is not null)
        {
            var remaining = wounds - regularCapacity;
            if (remaining >= unit.Hero.Toughness)
            {
                heroRemoved = 1;
            }
        }

        return regularRemoved + heroRemoved;
    }

    private static ProbabilityDistribution<AttackDamage> ResolveUnitAttacks(
        UnitProfile attacker,
        UnitProfile defender,
        int attackingModels,
        bool attackerHeroActive,
        bool defenderHeroActive,
        bool isCharging)
    {
        var aggregate = ProbabilityDistribution<AttackDamage>.Certain(new AttackDamage(0, 0, 0));

        var offensiveBeforeHit = CombineRules(attacker.AttackerBeforeHitRules, attackerHeroActive ? attacker.Hero?.OffensiveAuras : null);
        var defensiveBeforeHit = CombineRules(defender.DefensiveBeforeHitRules, defenderHeroActive ? defender.Hero?.DefensiveBeforeHitAuras : null);
        var defensiveAfterDefense = CombineRules(defender.DefensiveAfterDefenseRules, defenderHeroActive ? defender.Hero?.DefensiveAfterDefenseAuras : null);

        if (attackingModels > 0 && attacker.Weapons.Count > 0)
        {
            foreach (var weapon in attacker.Weapons)
            {
                var weaponDistribution = ResolveWeapon(attacker, defender, weapon, attackingModels, isCharging, attacker.Quality, offensiveBeforeHit, defensiveBeforeHit, defensiveAfterDefense);
                aggregate = aggregate.Combine(weaponDistribution, static (left, right) => left.Add(right));
            }
        }

        if (attackerHeroActive && attacker.Hero is { } hero && hero.Weapons.Count > 0)
        {
            var heroBeforeHit = CombineRules(hero.OffensiveAuras, hero.AttackerBeforeHitRules);
            var heroAfterHit = hero.OffensiveAfterHitAuras;
            foreach (var weapon in hero.Weapons)
            {
                var weaponDistribution = ResolveWeapon(attacker, defender, weapon, attackingModels: 1, isCharging, hero.Quality, heroBeforeHit, defensiveBeforeHit, defensiveAfterDefense);
                aggregate = aggregate.Combine(weaponDistribution, static (left, right) => left.Add(right));
            }
        }

        return aggregate;
    }

    private static ProbabilityDistribution<AttackDamage> ResolveWeapon(
        UnitProfile attacker,
        UnitProfile defender,
        WeaponProfile weapon,
        int attackingModels,
        bool isCharging,
        int quality,
        IReadOnlyList<IBeforeHitOffensiveRule> offensiveBeforeHit,
        IReadOnlyList<IBeforeHitDefensiveRule> defensiveBeforeHit,
        IReadOnlyList<IAfterDefenseRule> defensiveAfterDefense)
    {
        if (weapon.AttacksPerModel <= 0 || attackingModels <= 0)
        {
            return ProbabilityDistribution<AttackDamage>.Certain(new AttackDamage(0, 0, 0));
        }

        var initialAttackDice = weapon.AttacksPerModel * attackingModels;
        var beforeContext = new BeforeHitContext(attacker, defender, weapon, attackingModels, weapon.AttacksPerModel, initialAttackDice, quality, weapon.ArmorPenetration, isCharging);
        ApplyRules(defensiveBeforeHit, beforeContext);
        ApplyRules(offensiveBeforeHit, beforeContext);
        ApplyRules(weapon.BeforeHitRules, beforeContext);

        var totalAttacks = Math.Max(0, beforeContext.TotalAttacks);
        var qualityTarget = Math.Max(2, beforeContext.Quality);
        var armorPenetration = beforeContext.ArmorPenetration;

        var hitDistribution = BuildRollDistribution(totalAttacks, qualityTarget);

    var afterHitContext = new AfterHitContext(attacker, defender, weapon, hitDistribution, isCharging, totalAttacks, qualityTarget);
        ApplyRules(weapon.AfterHitRules, afterHitContext);
        if (attacker.Hero is not null)
        {
            ApplyRules(attacker.Hero.OffensiveAfterHitAuras, afterHitContext);
        }

        armorPenetration += afterHitContext.DefenseModifiers.AdditionalArmorPenetration;

        var defenseDistribution = ResolveDefense(afterHitContext.Distribution, defender, armorPenetration, afterHitContext.DefenseModifiers);

        var afterDefenseContext = new AfterDefenseContext(attacker, defender, weapon, defenseDistribution)
        {
            RegenerationEnabled = !afterHitContext.DefenseModifiers.SuppressRegeneration
        };
        ApplyRules(weapon.AfterDefenseRules, afterDefenseContext);
        ApplyRules(defensiveAfterDefense, afterDefenseContext);

        return afterDefenseContext.Distribution.Select(state => new AttackDamage(
            DefenderWounds: Math.Max(0, state.UnsavedWounds),
            SelfInflictedWounds: Math.Max(0, state.SelfWounds),
            Hits: Math.Max(0, state.SelfWounds)));
    }

    private static ProbabilityDistribution<HitState> BuildRollDistribution(int totalRolls, int target, bool rerollSixes = false)
    {
        if (totalRolls <= 0)
        {
            return ProbabilityDistribution<HitState>.Certain(new HitState(0, 0, 0));
        }

        var singleAttack = BuildSingleRollDistribution(target, rerollSixes);
        var distribution = ProbabilityDistribution<HitState>.Certain(new HitState(0, 0, 0));

        for (var i = 0; i < totalRolls; i++)
        {
            distribution = distribution.Combine(singleAttack, static (left, right) => left.Add(right));
        }

        return distribution;
    }

    private static ProbabilityDistribution<HitState> BuildSingleRollDistribution(int target, bool rerollSixes = false)
    {
        var quality = Math.Max(2, target);

        const double faceProbability = 1d / 6d;
        var probabilityNatOne = faceProbability;
        var probabilityNatSix = faceProbability;

        var successesBetweenTwoAndFive = 0;
        if (quality <= 5)
        {
            var minimumNeeded = Math.Max(quality, 2);
            successesBetweenTwoAndFive = Math.Max(0, 5 - minimumNeeded + 1);
        }

        var probabilityRegularSuccess = successesBetweenTwoAndFive * faceProbability;
        var probabilityRegularFailure = Math.Max(0d, 1d - (probabilityNatOne + probabilityNatSix + probabilityRegularSuccess));

        // If we reroll sixes, the chance of each roll increases by 1/6 (except 6 itself which is then rerolled)
        if (rerollSixes)
        {
            probabilityNatSix = faceProbability / 6d;
            probabilityNatOne += faceProbability / 6d;
            probabilityRegularSuccess += probabilityRegularSuccess / 6d;
            probabilityRegularFailure += probabilityRegularFailure / 6d;
        }
        

        var outcomes = new Dictionary<HitState, double>();

        void AddOutcome(HitState state, double probability)
        {
            if (probability <= 0)
            {
                return;
            }

            outcomes[state] = probability;
        }

        AddOutcome(new HitState(0, 0, 1), probabilityNatOne);
        AddOutcome(new HitState(1, 1, 0), probabilityNatSix);
        AddOutcome(new HitState(1, 0, 0), probabilityRegularSuccess);
        AddOutcome(new HitState(0, 0, 0), probabilityRegularFailure);

        return new ProbabilityDistribution<HitState>(outcomes);
    }

    private static ProbabilityDistribution<DefenseState> ResolveDefense(
        ProbabilityDistribution<HitState> hitDistribution,
        UnitProfile defender,
        int armorPenetration,
        DefenseModifiers defenseModifiers)
    {
    return hitDistribution.SelectMany(hitState =>
        {
            var totalHits = Math.Max(0, hitState.TotalHits);
            if (totalHits <= 0)
            {
                var wounds = Math.Max(0, hitState.DirectWounds);
                return ProbabilityDistribution<DefenseState>.Certain(new DefenseState(wounds, hitState.SelfWounds, 0));
            }

            var naturalSixHits = Math.Clamp(hitState.NaturalSixes, 0, totalHits);
            var regularHits = totalHits - naturalSixHits;

            var baseArmorPenetration = armorPenetration + defenseModifiers.AdditionalArmorPenetration + hitState.ArmorPenetrationBonus;
            var rerollSixes = defenseModifiers.ForceDefenseSixReroll;

            var regularDistribution = BuildRollDistribution(regularHits, defender.Defense - baseArmorPenetration, rerollSixes);
            var sixDistribution = BuildRollDistribution(naturalSixHits, defender.Defense - baseArmorPenetration - defenseModifiers.AdditionalArmorPenetrationOnNaturalSix);

            return regularDistribution.Combine(sixDistribution, static (regular, six) => regular.Add(six))
                                       .Select(total => new DefenseState(total.TotalHits + hitState.DirectWounds, hitState.SelfWounds, hitState.NaturalOnes));
        });
    }

    private static double ComputeUnsavedProbability(int defenseTarget, int armorPenetration, bool forceRerollSix)
    {
        var defense = Math.Max(2, defenseTarget);
        const double faceProbability = 1d / 6d;
        double unsaved = 0d;

        for (var roll = 1; roll <= 6; roll++)
        {
            if (roll == 1)
            {
                unsaved += faceProbability;
                continue;
            }

            if (roll == 6)
            {
                if (forceRerollSix)
                {
                    unsaved += ComputeUnsavedProbability(defenseTarget, armorPenetration, false) / 6d;
                }
                else
                {
                    continue;
                }
            }

            var adjusted = roll - armorPenetration;
            var saves = adjusted >= defense;
            if (!saves)
            {
                unsaved += faceProbability;
            }
        }

        return Math.Clamp(unsaved, 0d, 1d);
    }

    // private static ProbabilityDistribution<int> BuildBinomialDistribution(int trials, double successProbability)
    // {
    //     if (trials <= 0)
    //     {
    //         return ProbabilityDistribution<int>.Certain(0);
    //     }
    //
    //     successProbability = Math.Clamp(successProbability, 0d, 1d);
    //     var outcomes = new Dictionary<int, double>();
    //
    //     var combination = 1d;
    //     for (var k = 0; k <= trials; k++)
    //     {
    //         if (k == 0)
    //         {
    //             combination = 1d;
    //         }
    //         else
    //         {
    //             combination *= (double)(trials - (k - 1)) / k;
    //         }
    //
    //         var probability = combination * Math.Pow(successProbability, k) * Math.Pow(1d - successProbability, trials - k);
    //         if (probability <= 0)
    //         {
    //             continue;
    //         }
    //
    //         outcomes[k] = probability;
    //     }
    //
    //     return new ProbabilityDistribution<int>(outcomes);
    // }

    private static int RemainingRegularModels(UnitProfile defender, int woundsSustained)
    {
        if (defender.ModelCount <= 0)
        {
            return 0;
        }

        var regularCapacity = defender.RegularWoundCapacity;
        var effectiveWounds = Math.Min(woundsSustained, regularCapacity);
        var modelsDestroyed = Math.Min(defender.ModelCount, effectiveWounds / defender.Toughness);
        return Math.Max(0, defender.ModelCount - modelsDestroyed);
    }

    private static int ModelsLost(UnitProfile unit, int woundsSustained)
    {
        if (unit.ModelCount <= 0)
        {
            return 0;
        }

        var regularCapacity = unit.RegularWoundCapacity;
        var effectiveWounds = Math.Min(woundsSustained, regularCapacity);
        return Math.Min(unit.ModelCount, effectiveWounds / unit.Toughness);
    }

    private static bool HeroSurvives(UnitProfile unit, int woundsSustained)
    {
        if (unit.Hero is null)
        {
            return false;
        }

        var threshold = unit.RegularWoundCapacity + unit.Hero.Toughness;
        return woundsSustained < threshold;
    }

    private static bool UnitHasCounter(UnitProfile unit)
    {
        return unit.Weapons.Any(weapon => weapon.BeforeHitRules.OfType<CounterRule>().Any());
    }

    private static IReadOnlyList<T> CombineRules<T>(IReadOnlyList<T> primary, IReadOnlyList<T>? secondary) where T: IBeforeHitRule
    {
        if (secondary is null || secondary.Count == 0)
        {
            return primary;
        }

        if (primary.Count == 0)
        {
            return secondary;
        }

        return primary.Concat(secondary).ToArray();
    }

    private static IReadOnlyList<IAfterDefenseRule> CombineRules(IReadOnlyList<IAfterDefenseRule> primary, IReadOnlyList<IAfterDefenseRule>? secondary)
    {
        if (secondary is null || secondary.Count == 0)
        {
            return primary;
        }

        if (primary.Count == 0)
        {
            return secondary;
        }

        return primary.Concat(secondary).ToArray();
    }

    private static void ApplyRules(IEnumerable<IBeforeHitRule>? rules, BeforeHitContext context)
    {
        if (rules is null)
        {
            return;
        }

        foreach (var rule in rules)
        {
            rule.Apply(context);
        }
    }

    private static void ApplyRules(IEnumerable<IAfterHitRule>? rules, AfterHitContext context)
    {
        if (rules is null)
        {
            return;
        }

        foreach (var rule in rules)
        {
            rule.Apply(context);
        }
    }

    private static void ApplyRules(IEnumerable<IAfterDefenseRule>? rules, AfterDefenseContext context)
    {
        if (rules is null)
        {
            return;
        }

        foreach (var rule in rules)
        {
            rule.Apply(context);
        }
    }
}
