namespace BattleSim.Core.Simulation;

/// <summary>
/// Represents the damage dealt by an attacking weapon, including friendly fire.
/// </summary>
public readonly record struct AttackDamage(int DefenderWounds, int SelfInflictedWounds, int Hits)
{
    public AttackDamage Add(AttackDamage other)
    {
        return new AttackDamage(
            DefenderWounds + other.DefenderWounds,
            SelfInflictedWounds + other.SelfInflictedWounds,
            Hits + other.Hits);
    }
}
