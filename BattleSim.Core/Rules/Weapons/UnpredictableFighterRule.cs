using BattleSim.Core.Simulation;

namespace BattleSim.Core.Rules.Weapons;

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
            return ProbabilityDistribution<HitState>.Certain(new HitState(0, 0, 0));
        }

        var singleAttack = BuildSingleAttackDistribution(qualityTarget);
        var distribution = ProbabilityDistribution<HitState>.Certain(new HitState(0, 0, 0));

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

        AddOutcomeLocal(new HitState(0, 0, 1), probabilityNatOne);
        AddOutcomeLocal(new HitState(1, 1, 0), probabilityNatSix);
        AddOutcomeLocal(new HitState(1, 0, 0), probabilityRegularSuccess);
        AddOutcomeLocal(new HitState(0, 0, 0), probabilityRegularFailure);

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