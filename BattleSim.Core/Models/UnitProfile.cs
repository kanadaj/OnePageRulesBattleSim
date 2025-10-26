using System;
using System.Collections.Generic;
using System.Linq;
using BattleSim.Core.Rules;

namespace BattleSim.Core.Models;

/// <summary>
/// Describes a combat unit in the fantasy battle simulation.
/// </summary>
public record UnitProfile
{
    public UnitProfile(
        string name,
        int quality,
        int defense,
        int toughness,
        int fear,
        int modelCount,
        IEnumerable<WeaponProfile>? weapons = null,
        IEnumerable<IRule>? modelRules = null,
        HeroProfile? hero = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        }

        if (quality < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(quality), "Quality must be at least 2 to allow for automatic failure on 1.");
        }

        if (defense < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(defense), "Defense must be at least 2 to allow for automatic failure on 1.");
        }

        if (toughness <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(toughness), "Toughness must be positive.");
        }

        if (modelCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(modelCount), "Model count cannot be negative.");
        }

        Name = name;
        Quality = quality;
        Defense = defense;
        Toughness = toughness;
        Fear = fear;
        ModelCount = modelCount;
        Weapons = (weapons ?? Array.Empty<WeaponProfile>()).ToArray();
        Rules = (modelRules ?? Array.Empty<IBeforeHitDefensiveRule>()).ToArray();
        Hero = hero;
    }

    public string Name { get; init; }

    /// <summary>
    /// Minimum die roll (on a D6) required to score a hit; 1 always fails and 6 always succeeds.
    /// </summary>
    public int Quality { get; init; }

    /// <summary>
    /// Minimum adjusted die roll (on a D6) required to save against a wound; 1 always fails, 6 always succeeds.
    /// </summary>
    public int Defense { get; init; }

    /// <summary>
    /// Number of wounds required to destroy a single model.
    /// </summary>
    public int Toughness { get; init; }

    /// <summary>
    /// Additional combat resolution points counted as wounds but not applied as damage.
    /// </summary>
    public int Fear { get; init; }

    public int ModelCount { get; init; }

    public IReadOnlyList<WeaponProfile> Weapons { get; init; }

    /// <summary>
    /// Special rules that modify attacks
    /// </summary>
    public IReadOnlyList<IRule> Rules { get; init; }

    public HeroProfile? Hero { get; init; }

    public int TotalFear => Fear + (Hero?.Fear ?? 0);

    public int RegularWoundCapacity => ModelCount * Toughness;

    public int TotalWoundCapacity => RegularWoundCapacity + (Hero?.Toughness ?? 0);

    public UnitProfile AsCombinedUnit => this with
    {
        ModelCount = this.ModelCount == 1
            ? throw new InvalidOperationException("Cannot combine single model units.")
            : this.ModelCount * 2,
        Combined = true
    };

    public bool Combined { get; private init; }
    public IEnumerable<IAttackRule> AttackRules => Rules.OfType<IAttackRule>();

    public string GetCombinedName()
    {
        return Name + Combined switch
        {
            true => " (Combined)",
            false => string.Empty
        } + (Hero is not null ? $" on {Hero.Name}" : string.Empty);
    }

    public string GetName()
    {
        return Name + (this is HeroProfile { Mount: not null } hp ? $" with {hp.Mount}" : string.Empty);
    }

    public UnitProfile WithHero(HeroProfile hero) => this with { Hero = hero };
}