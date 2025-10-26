using System;

namespace BattleSim.Core.Simulation;

/// <summary>
/// Represents the outcome statistics for a batch of hit rolls.
/// </summary>
public readonly record struct HitState(int TotalHits, int NaturalSixes, int NaturalOnes)
{
    public int ArmorPenetrationBonus { get; init; }
    
    public int SelfWounds { get; init; }
    public int DirectWounds { get; init; }

    public HitState Add(HitState other)
    {
        return new HitState(
            TotalHits + other.TotalHits,
            NaturalSixes + other.NaturalSixes,
            NaturalOnes + other.NaturalOnes);
    }

    public HitState AddArmorPenetrationBonus(int delta)
    {
        var updated = ArmorPenetrationBonus + delta;
        if (updated < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(delta), "Resulting armor penetration bonus cannot be negative.");
        }

        return this with { ArmorPenetrationBonus = updated };
    }

    public HitState AddSelfWounds(int stateNaturalOnes)
    {
        var updated = SelfWounds + stateNaturalOnes;
        if (updated < 0)
        {
            updated = 0;
        }

        return this with { SelfWounds = updated };
    }

    public HitState AddDirectWounds(int wounds)
    {
        var updated = DirectWounds + wounds;
        if (updated < 0)
        {
            updated = 0;
        }

        return this with { DirectWounds = updated };
    }
}
