using System;
using System.Collections.Generic;
using System.Linq;
using BattleSim.Core.Rules;

namespace BattleSim.Core.Models;

/// <summary>
/// Describes the weapon profile available to a unit's models.
/// </summary>
public sealed record WeaponProfile
{
    public WeaponProfile(
        string name,
        int attacksPerModel,
        int armorPenetration = 0,
        bool isRanged = false,
        IReadOnlyList<IRule>? rules = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Weapon name cannot be null or whitespace.", nameof(name));
        }

        if (attacksPerModel < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(attacksPerModel), "Attacks per model cannot be negative.");
        }

        Name = name;
        AttacksPerModel = attacksPerModel;
        ArmorPenetration = armorPenetration;
        IsRanged = isRanged;
        Rules = rules ?? [];
    }

    public string Name { get; }

    /// <summary>
    /// Number of attack dice rolled per surviving model.
    /// </summary>
    public int AttacksPerModel { get; }

    /// <summary>
    /// Armor penetration modifier applied against the defender's saves.
    /// </summary>
    public int ArmorPenetration { get; }

    /// <summary>
    /// True if the weapon is a ranged weapon.
    /// </summary>
    public bool IsRanged { get; }

    public IReadOnlyList<IBeforeHitOffensiveRule> BeforeHitRules => Rules.OfType<IBeforeHitOffensiveRule>().ToArray();

    public IReadOnlyList<IAfterHitOffensiveRule> AfterHitRules => Rules.OfType<IAfterHitOffensiveRule>().ToArray();

    public IReadOnlyList<IAfterDefenseDefensiveRule> AfterDefenseRules => Rules.OfType<IAfterDefenseDefensiveRule>().ToArray();
    public IReadOnlyList<IRule> Rules { get; init; }
}
