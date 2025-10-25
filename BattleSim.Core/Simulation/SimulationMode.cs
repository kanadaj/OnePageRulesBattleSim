namespace BattleSim.Core.Simulation;

/// <summary>
/// Specifies the engagement rules used when simulating combat between two units.
/// </summary>
public enum SimulationMode
{
    /// <summary>
    /// Resolves a full melee exchange where both units may attack.
    /// </summary>
    Melee,

    /// <summary>
    /// Resolves a ranged attack where only the initial attacker strikes.
    /// </summary>
    Ranged
}
