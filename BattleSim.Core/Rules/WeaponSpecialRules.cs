using System;
using BattleSim.Core.Simulation;

namespace BattleSim.Core.Rules;

public sealed class BaneRule : IAfterHitRule, IWeaponKeyword
{
    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.DefenseModifiers.SuppressRegeneration = true;
        context.DefenseModifiers.ForceDefenseSixReroll = true;
    }
}

public sealed class RendingRule : IAfterHitRule, IWeaponKeyword
{
    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.DefenseModifiers.SuppressRegeneration = true;
        context.DefenseModifiers.AdditionalArmorPenetrationOnNaturalSix += 4;
    }
}

public sealed class SlayerRule : IBeforeHitOffensiveRule
{
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Defender.Toughness >= 3)
        {
            context.ArmorPenetration += 2;
        }
    }
}

public sealed class ReapRule : IBeforeHitOffensiveRule
{
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Defender.Defense is 2 or 3)
        {
            context.ArmorPenetration += 2;
        }
    }
}

public sealed class FuriousRule : IAfterHitRule
{
    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (!context.IsCharging)
        {
            return;
        }

        context.Map(state => state with { TotalHits = state.TotalHits + state.NaturalSixes });
    }
}

public sealed class RelentlessRule : IAfterHitRule
{
    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.Map(state => state with { TotalHits = state.TotalHits + state.NaturalSixes });
    }
}

public sealed class CounterRule : IBeforeHitOffensiveRule, IWeaponKeyword
{
    public void Apply(BeforeHitContext context)
    {
        // Counter modifies attack order, handled by the simulator; no per-attack adjustment required.
    }
}

// Hit rolls of 6 get 2 AP
public sealed class CrackRule : IAfterHitRule, IWeaponKeyword
{
    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.DefenseModifiers.AdditionalArmorPenetrationOnNaturalSix += 2;
    }
}

public sealed class ThrustRule : IBeforeHitOffensiveRule
{
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (!context.IsCharging)
        {
            return;
        }

        context.Quality = Math.Max(2, context.Quality - 1);
        context.ArmorPenetration += 1;
    }
}

public sealed class ReliableRule : IBeforeHitOffensiveRule
{
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.Quality = Math.Min(context.Quality, 2);
    }
}

public sealed class DeadlyRule(int maxWounds) : IAfterDefenseRule
{
    public void Apply(AfterDefenseContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        
        if(context.Defender.Toughness > 1)
        {
            // Deals at most as many wounds to a single model as Deadly
            context.Map(state => state with { UnsavedWounds = state.UnsavedWounds * Math.Min(context.Defender.Toughness, maxWounds) });
        }
    }
}

public sealed class BlastRule(int maxWounds) : IAfterHitRule
{
    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        
        if(context.Defender.ModelCount > 1)
        {
            // Deals at most as many wounds to a single model as Deadly
            context.Map(state => state with { TotalHits = state.TotalHits * Math.Min(context.Defender.ModelCount, maxWounds) });
        }
    }
}

public sealed class PreciseRule : IBeforeHitOffensiveRule
{
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.Quality = Math.Max(2, context.Quality - 1);
    }
}

public sealed class CavalryCapRule : IBeforeHitOffensiveRule, IWeaponKeyword
{
    private const int MaximumChargingModels = 5;

    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var cappedModels = Math.Min(MaximumChargingModels, context.AttackingModels);
        var baseAttacks = context.AttacksPerModel * context.AttackingModels;
        var cappedBase = context.AttacksPerModel * cappedModels;
        var bonusAttacks = Math.Max(0, context.TotalAttacks - baseAttacks);
        context.TotalAttacks = Math.Max(0, cappedBase + bonusAttacks);
    }
}

public sealed class PiercingTagRule(int armorPenetration = 1) : IBeforeHitOffensiveRule
{
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.ArmorPenetration += armorPenetration;
    }
}

public interface IWeaponKeyword
{
}

// Deals wounds to own unit on hit rolls of 1
public sealed class OvertunedRule : IAfterHitRule, IWeaponKeyword
{
    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.Map(state => state.AddSelfWounds(state.NaturalOnes));
    }
}

public sealed class MultiplyHitsRule(int hitMultiplier = 3) : IAfterHitRule
{

    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.Map(state =>
        {
            if (state.TotalHits <= 0)
            {
                return state;
            }

            var multipliedHits = state.TotalHits * hitMultiplier;
            var multipliedSixes = state.NaturalSixes * hitMultiplier;
            return state with { TotalHits = multipliedHits, NaturalSixes = multipliedSixes };
        });
    }
}

public sealed class UnpredictableFighterRule : IAfterHitRule
{
    private const double BranchProbability = 0.5d;

    public void Apply(AfterHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var totalAttacks = Math.Max(0, context.TotalAttacks);
        var qualityTarget = Math.Max(2, context.QualityTarget);

        var apDistribution = context.Distribution.Select(state => state.AddArmorPenetrationBonus(1));

        ProbabilityDistribution<HitState> qualityDistribution;
        if (qualityTarget <= 2 || totalAttacks <= 0)
        {
            qualityDistribution = context.Distribution;
        }
        else
        {
            var improvedQuality = Math.Max(2, qualityTarget - 1);
            qualityDistribution = BuildHitDistribution(totalAttacks, improvedQuality);
        }

        var blended = CombineWeighted(apDistribution, qualityDistribution, BranchProbability, BranchProbability);
        context.ReplaceDistribution(blended);
    }

    private static ProbabilityDistribution<HitState> CombineWeighted(
        ProbabilityDistribution<HitState> left,
        ProbabilityDistribution<HitState> right,
        double leftWeight,
        double rightWeight)
    {
        var outcomes = new Dictionary<HitState, double>();

        foreach (var (state, probability) in left.Probabilities)
        {
            AddOutcome(outcomes, state, leftWeight * probability);
        }

        foreach (var (state, probability) in right.Probabilities)
        {
            AddOutcome(outcomes, state, rightWeight * probability);
        }

        return new ProbabilityDistribution<HitState>(outcomes);
    }

    private static ProbabilityDistribution<HitState> BuildHitDistribution(int totalAttacks, int qualityTarget)
    {
        if (totalAttacks <= 0)
        {
            return ProbabilityDistribution<HitState>.Certain(new HitState(0, 0, 0, 0));
        }

        var singleAttack = BuildSingleAttackDistribution(qualityTarget);
        var distribution = ProbabilityDistribution<HitState>.Certain(new HitState(0, 0, 0, 0));

        for (var i = 0; i < totalAttacks; i++)
        {
            distribution = distribution.Combine(singleAttack, static (left, right) => left.Add(right));
        }

        return distribution;
    }

    private static ProbabilityDistribution<HitState> BuildSingleAttackDistribution(int qualityTarget)
    {
        var quality = Math.Max(2, qualityTarget);

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

        var outcomes = new Dictionary<HitState, double>();

        void AddOutcomeLocal(HitState state, double probability)
        {
            if (probability <= 0)
            {
                return;
            }

            outcomes[state] = probability;
        }

        AddOutcomeLocal(new HitState(0, 0, 1, 0), probabilityNatOne);
        AddOutcomeLocal(new HitState(1, 1, 0, 0), probabilityNatSix);
        AddOutcomeLocal(new HitState(1, 0, 0, 0), probabilityRegularSuccess);
        AddOutcomeLocal(new HitState(0, 0, 0, 0), probabilityRegularFailure);

        return new ProbabilityDistribution<HitState>(outcomes);
    }

    private static void AddOutcome(IDictionary<HitState, double> outcomes, HitState state, double probability)
    {
        if (probability <= 0)
        {
            return;
        }

        if (outcomes.TryGetValue(state, out var current))
        {
            outcomes[state] = current + probability;
        }
        else
        {
            outcomes[state] = probability;
        }
    }
}
