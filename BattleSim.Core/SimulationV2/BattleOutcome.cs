namespace BattleSim.Core.SimulationV2;

/// <summary>
/// Represents the outcome of a single combat resolution between two units.
/// </summary>
/// <param name="WoundsToUnitA">The wounds inflicted on unit A (the initial attacker in the simulation).</param>
/// <param name="WoundsToUnitB">The wounds inflicted on unit B.</param>
/// <param name="ScoreForUnitA">The combat resolution score for unit A (wounds inflicted plus fear).</param>
/// <param name="ScoreForUnitB">The combat resolution score for unit B (wounds inflicted plus fear).</param>
public readonly record struct BattleOutcome(int HitsToDefender, int WoundsToDefender, int HitsToAttacker, int WoundsToAttacker, int FearFromDefender, int FearFromAttacker)
{
    public int TotalScoreForDefender => WoundsToAttacker + FearFromDefender;
    public int TotalScoreForAttacker => WoundsToDefender + FearFromAttacker;
}
