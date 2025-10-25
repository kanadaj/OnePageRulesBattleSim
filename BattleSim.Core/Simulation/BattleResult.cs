using System.Collections.Generic;

namespace BattleSim.Core.Simulation;

/// <summary>
/// Aggregates the statistical results of a simulated battle between two units.
/// </summary>
public sealed record BattleResult(
    double AverageWoundsToUnitA,
    double AverageWoundsToUnitB,
    double AverageModelsLostByUnitA,
    double AverageModelsLostByUnitB,
    double ProbabilityUnitAWins,
    double ProbabilityUnitBWins,
    double ProbabilityOfTie,
    double ProbabilityUnitADestroysUnitB,
    double ProbabilityUnitBDestroysUnitA,
    double AlternativeOutcomeProbabilityForUnitA,
    double AlternativeOutcomeProbabilityForUnitB,
    double AverageHitsByUnitA,
    double AverageHitsByUnitB,
    IReadOnlyDictionary<BattleOutcome, double> OutcomeDistribution);
