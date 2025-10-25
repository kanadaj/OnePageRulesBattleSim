namespace BattleSim.Core.Simulation;

/// <summary>
/// Bundles the defensive resolution outcome for a weapon, preserving the originating hit statistics.
/// </summary>
public readonly record struct DefenseState(HitState HitState, int UnsavedWounds)
{
    public DefenseState AdjustUnsaved(int delta)
    {
        var updated = UnsavedWounds + delta;
        if (updated < 0)
        {
            updated = 0;
        }

        return this with { UnsavedWounds = updated };
    }
}
