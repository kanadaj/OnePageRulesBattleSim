using System;
using BattleSim.Core.Simulation;

namespace BattleSim.Core.Rules;

public sealed class EvasionRule : IBeforeHitDefensiveRule
{
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.Quality = Math.Min(6, context.Quality + 1);
    }
}

public sealed class StealthRule : IBeforeHitDefensiveRule
{
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if  (context.Weapon.IsRanged){
            context.Quality = Math.Min(6, context.Quality + 1);
        }
    }
}

public sealed class RegenerationRule : IAfterDefenseRule
{
    public void Apply(AfterDefenseContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (!context.RegenerationEnabled)
        {
            return;
        }

        context.ReplaceDistribution(context.Distribution.SelectMany(ApplyRegeneration));
    }

    private static ProbabilityDistribution<DefenseState> ApplyRegeneration(DefenseState state)
    {
        if (state.UnsavedWounds <= 0)
        {
            return ProbabilityDistribution<DefenseState>.Certain(state);
        }

        const double healProbability = 2d / 6d;
        var distribution = BuildBinomialDistribution(state.UnsavedWounds, healProbability);

        return distribution.Select(healed => state with { UnsavedWounds = state.UnsavedWounds - healed });
    }

    private static ProbabilityDistribution<int> BuildBinomialDistribution(int trials, double successProbability)
    {
        if (trials <= 0)
        {
            return ProbabilityDistribution<int>.Certain(0);
        }

        successProbability = Math.Clamp(successProbability, 0d, 1d);
        var outcomes = new System.Collections.Generic.Dictionary<int, double>();

        double combination = 1d;
        for (var k = 0; k <= trials; k++)
        {
            if (k == 0)
            {
                combination = 1d;
            }
            else
            {
                combination *= (double)(trials - (k - 1)) / k;
            }

            var probability = combination * Math.Pow(successProbability, k) * Math.Pow(1d - successProbability, trials - k);
            if (probability <= 0)
            {
                continue;
            }

            outcomes[k] = probability;
        }

        return new ProbabilityDistribution<int>(outcomes);
    }
}

public sealed class ResistanceRule : IAfterDefenseRule
{
    public void Apply(AfterDefenseContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.ReplaceDistribution(context.Distribution.SelectMany(ApplyResistance));
    }

    private static ProbabilityDistribution<DefenseState> ApplyResistance(DefenseState state)
    {
        if (state.UnsavedWounds <= 0)
        {
            return ProbabilityDistribution<DefenseState>.Certain(state);
        }

        const double healProbability = 1d / 6d;
        var distribution = BuildBinomialDistribution(state.UnsavedWounds, healProbability);

        return distribution.Select(healed => state with { UnsavedWounds = state.UnsavedWounds - healed });
    }

    private static ProbabilityDistribution<int> BuildBinomialDistribution(int trials, double successProbability)
    {
        if (trials <= 0)
        {
            return ProbabilityDistribution<int>.Certain(0);
        }

        successProbability = Math.Clamp(successProbability, 0d, 1d);
        var outcomes = new System.Collections.Generic.Dictionary<int, double>();

        double combination = 1d;
        for (var k = 0; k <= trials; k++)
        {
            if (k == 0)
            {
                combination = 1d;
            }
            else
            {
                combination *= (double)(trials - (k - 1)) / k;
            }

            var probability = combination * Math.Pow(successProbability, k) * Math.Pow(1d - successProbability, trials - k);
            if (probability <= 0)
            {
                continue;
            }

            outcomes[k] = probability;
        }

        return new ProbabilityDistribution<int>(outcomes);
    }
}

// Implements a shield that increases armor by one
public sealed class ShieldRule : IBeforeHitDefensiveRule
{
    public void Apply(BeforeHitContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.ArmorPenetration -= 1;
    }
}

public sealed class FortifiedRule : IAfterHitRule
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

            return state with
            {
                ArmorPenetrationBonus = state.ArmorPenetrationBonus >= 1 ? state.ArmorPenetrationBonus - 1 : 0,
            };
        });
    }
}