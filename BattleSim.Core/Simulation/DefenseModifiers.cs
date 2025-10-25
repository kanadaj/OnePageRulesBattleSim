namespace BattleSim.Core.Simulation;

/// <summary>
/// Encapsulates adjustments to defensive rolls produced by weapon special rules.
/// </summary>
public sealed class DefenseModifiers
{
    /// <summary>
    /// Additional armor penetration applied to all hit rolls beyond the base value.
    /// </summary>
    public int AdditionalArmorPenetration { get; set; }

    /// <summary>
    /// Additional armor penetration applied only to hits generated from natural sixes.
    /// </summary>
    public int AdditionalArmorPenetrationOnNaturalSix { get; set; }

    /// <summary>
    /// Forces defenders to reroll natural six saves once; the reroll result stands.
    /// </summary>
    public bool ForceDefenseSixReroll { get; set; }

    /// <summary>
    /// True when the defender cannot use regeneration or similar effects after saves.
    /// </summary>
    public bool SuppressRegeneration { get; set; }
}
