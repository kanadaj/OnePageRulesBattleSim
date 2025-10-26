using System;

namespace BattleSim.Core.Simulation;

/// <summary>
/// Represents the outcome statistics for a batch of hit rolls.
/// </summary>
public readonly struct HitState
{
    public int ArmorPenetrationBonus { get; init; }
    public int SelfWounds { get; init; }
    public int DirectWounds { get; init; }
    
    public int TotalHits { get; init; } 
    public int NaturalSixes { get; init; } 
    public int NaturalOnes { get; init; }

    private readonly int _hashCode;

    public HitState(int totalHits, int naturalSixes, int naturalOnes)
    {
        TotalHits = totalHits;
        NaturalSixes = naturalSixes;
        NaturalOnes = naturalOnes;
        _hashCode = CalculateHashCode();
    }

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

    private int CalculateHashCode()
    {
        return HashCode.Combine(TotalHits, NaturalSixes, NaturalOnes);
    }

    public override bool Equals(object? o)
    {
        return  o is HitState other && _hashCode == other._hashCode;
    }

    public override int GetHashCode() => _hashCode;
}
