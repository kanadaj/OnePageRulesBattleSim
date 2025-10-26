using System;

namespace BattleSim.Core.Simulation;

/// <summary>
/// Represents the outcome statistics for a batch of hit rolls.
/// </summary>
public readonly record struct HitState
{
    public HitState(int totalHits, int naturalSixes, int naturalOnes, int directWounds, int selfWounds = 0, int armorPenetrationBonus = 0)
    {
        TotalHits = totalHits;
        NaturalSixes = naturalSixes;
        NaturalOnes = naturalOnes;
        DirectWounds = directWounds;
        SelfWounds = selfWounds;
        ArmorPenetrationBonus = armorPenetrationBonus;
    }

    public int TotalHits { get; init; }

    public int NaturalSixes { get; init; }

    public int NaturalOnes { get; init; }

    public int DirectWounds { get; init; }

    /// <summary>
    /// Wounds inflicted on the attacking unit by its own special rules.
    /// </summary>
    public int SelfWounds { get; init; }

    public int ArmorPenetrationBonus { get; init; }

    public HitState Add(HitState other)
    {
        return new HitState(
            TotalHits + other.TotalHits,
            NaturalSixes + other.NaturalSixes,
            NaturalOnes + other.NaturalOnes,
            DirectWounds + other.DirectWounds,
            SelfWounds + other.SelfWounds,
            ArmorPenetrationBonus + other.ArmorPenetrationBonus);
    }

    public HitState WithDirectWounds(int directWounds)
    {
        if (directWounds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(directWounds), "Direct wounds cannot be negative.");
        }

        return this with { DirectWounds = directWounds };
    }

    public HitState AddDirectWounds(int delta)
    {
        var updated = DirectWounds + delta;
        if (updated < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(delta), "Resulting direct wounds cannot be negative.");
        }

        return this with { DirectWounds = updated };
    }

    public HitState AddSelfWounds(int delta)
    {
        var updated = SelfWounds + delta;
        if (updated < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(delta), "Resulting self wounds cannot be negative.");
        }

        return this with { SelfWounds = updated };
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
}
