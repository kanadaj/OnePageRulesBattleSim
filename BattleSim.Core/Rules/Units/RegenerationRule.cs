using BattleSim.Core.Simulation;

namespace BattleSim.Core.Rules.Units;

public sealed class RegenerationRule : IAfterDefenseDefensiveRule
{
    public string Name => "Regeneration";
    
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