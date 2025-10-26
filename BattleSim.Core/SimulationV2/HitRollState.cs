namespace BattleSim.Core.SimulationV2;

/// <summary>
/// Raw results of the hit rolls
/// </summary>
public record HitRollResult(byte Ones, byte Twos, byte Threes, byte Fours, byte Fives, byte Sixes);


public record Hits(byte NormalSuccesses, byte DirectWounds, byte SelfWounds, byte WoundsOnNaturalOneDefense, byte RendingHits);