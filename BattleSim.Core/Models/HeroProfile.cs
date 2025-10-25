using System;
using System.Collections.Generic;
using System.Linq;
using BattleSim.Core.Rules;

namespace BattleSim.Core.Models;

/// <summary>
/// Represents an individual hero that can be attached to a unit.
/// </summary>
public sealed record HeroProfile : UnitProfile
{
    public HeroProfile(
        string name,
        int quality,
        int defense,
        int toughness = 3,
        int fear = 0,
        IEnumerable<WeaponProfile>? weapons = null,
        IEnumerable<IBeforeHitDefensiveRule>? defensiveBeforeHitRules = null,
        IEnumerable<IBeforeHitOffensiveRule>? attackerBeforeHitRules = null,
        IEnumerable<IAfterDefenseRule>? defensiveAfterDefenseRules = null,
        IEnumerable<IBeforeHitOffensiveRule>? offensiveAuras = null,
        IEnumerable<IBeforeHitDefensiveRule>? defensiveBeforeHitAuras = null,
        IEnumerable<IAfterDefenseRule>? defensiveAfterDefenseAuras = null,
        IEnumerable<IAfterHitRule>? offensiveAfterHitAuras = null) : base(name, quality, defense, toughness, fear, 1, weapons, defensiveBeforeHitRules, attackerBeforeHitRules, defensiveAfterDefenseRules, null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        }

        if (quality < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(quality), "Quality must allow for natural 1 failures.");
        }

        if (toughness <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(toughness), "Toughness must be positive.");
        }

        OffensiveAuras = (offensiveAuras ?? Array.Empty<IBeforeHitOffensiveRule>()).ToArray();
        DefensiveBeforeHitAuras = (defensiveBeforeHitAuras ?? Array.Empty<IBeforeHitDefensiveRule>()).ToArray();
        DefensiveAfterDefenseAuras = (defensiveAfterDefenseAuras ?? Array.Empty<IAfterDefenseRule>()).ToArray();
        OffensiveAfterHitAuras = (offensiveAfterHitAuras ?? Array.Empty<IAfterHitRule>()).ToArray();
    }

    /// <summary>
    /// Rules that apply to the entire unit's outgoing attacks.
    /// </summary>
    public IReadOnlyList<IBeforeHitOffensiveRule> OffensiveAuras { get; init; }

    /// <summary>
    /// Rules that apply to the entire unit's outgoing attacks.
    /// </summary>
    public IReadOnlyList<IAfterHitRule> OffensiveAfterHitAuras { get; init; }

    /// <summary>
    /// Rules that apply to the entire unit when defending before hit rolls are made.
    /// </summary>
    public IReadOnlyList<IBeforeHitDefensiveRule> DefensiveBeforeHitAuras { get; init; }

    /// <summary>
    /// Rules that apply to the entire unit when resolving saves.
    /// </summary>
    public IReadOnlyList<IAfterDefenseRule> DefensiveAfterDefenseAuras { get; init; }
    
    public IReadOnlyList<IRule> Auras => OffensiveAuras
        .Cast<IRule>()
        .Concat(OffensiveAfterHitAuras)
        .Concat(DefensiveBeforeHitAuras)
        .Concat(DefensiveAfterDefenseAuras)
        .ToArray();

    public string? Mount { get; init; }

    public HeroProfile WithWeapons(params WeaponProfile[] weapons)
    {
        return this with { Weapons = weapons };
    }

    public HeroProfile WithPersonalRules(params IRule[] rules)
    {
        return this with
        {
            AttackerBeforeHitRules = rules.OfType<IBeforeHitOffensiveRule>().ToArray(),
            DefensiveBeforeHitRules = rules.OfType<IBeforeHitDefensiveRule>().ToArray(),
            DefensiveAfterDefenseRules = rules.OfType<IAfterDefenseRule>().ToArray()
        };
    }

    public HeroProfile WithAuras(params IRule[] rules)
    {
        return this with
        {
            OffensiveAuras = rules.OfType<IBeforeHitOffensiveRule>().ToArray(),
            OffensiveAfterHitAuras = rules.OfType<IAfterHitRule>().ToArray(),
            DefensiveBeforeHitAuras = rules.OfType<IBeforeHitDefensiveRule>().ToArray(),
            DefensiveAfterDefenseAuras = rules.OfType<IAfterDefenseRule>().ToArray()
        };
    }
    
    public HeroProfile WithMount(HeroMount mount)
    {
        return (this with
        {
            Mount = mount.Name,
            Toughness = Toughness + mount.AdditionalToughness,
            Weapons = Weapons.Concat(mount.Weapons).ToArray(),
            Defense = mount.Armor,
            Fear = Fear + mount.Fear
        }).WithPersonalRules(mount.Rules.ToArray());
    }
}

public record HeroMount(string Name, int AdditionalToughness, int Armor, int Fear, ICollection<WeaponProfile> Weapons, params IRule[] Rules);