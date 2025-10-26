using System;
using BattleSim.Core.Models;
using BattleSim.Core.Simulation;

namespace BattleSim.Core.Rules;

public interface IRule
{
    string Name { get; }
}

public interface IAdditiveRule : IRule
{
    T Merge<T>(T rule) where T : IRule;
}

public interface IAttackRule : IRule {}
public interface IDefenseRule : IRule {}

public interface IBeforeHitRule : IRule
{
    void Apply(BeforeHitContext context);
}

public interface IBeforeHitOffensiveRule : IBeforeHitRule, IAttackRule
{
}

public interface IBeforeHitDefensiveRule : IBeforeHitRule, IDefenseRule
{
}

public interface IAfterHitOffensiveRule : IAttackRule
{
    void Apply(AfterHitContext context);
}

public interface IAfterHitDefensiveRule : IDefenseRule
{
    void Apply(AfterHitContext context);
}

public interface IAfterDefenseDefensiveRule : IDefenseRule
{
    void Apply(AfterDefenseContext context);
}

public interface IAfterDefenseOffensiveRule : IAttackRule
{
    void Apply(AfterDefenseContext context);
}

/// <summary>
/// Context information and mutable parameters accessible to special rules before hit rolls are made.
/// </summary>
public sealed class BeforeHitContext
{
    internal BeforeHitContext(UnitProfile attacker, UnitProfile defender, WeaponProfile weapon, int attackingModels, int attacksPerModel, int totalAttacks, int quality, int armorPenetration, bool isCharging)
    {
        Attacker = attacker;
        Defender = defender;
        Weapon = weapon;
        AttackingModels = attackingModels;
        AttacksPerModel = attacksPerModel;
        TotalAttacks = totalAttacks;
        Quality = quality;
        ArmorPenetration = armorPenetration;
        IsCharging = isCharging;
    }

    public UnitProfile Attacker { get; }

    public UnitProfile Defender { get; }

    public WeaponProfile Weapon { get; }

    public int AttackingModels { get; }

    /// <summary>
    /// Number of attack dice contributed per surviving model before modifiers.
    /// </summary>
    public int AttacksPerModel { get; }

    /// <summary>
    /// Gets or sets the total number of attack dice that will be rolled. Rules may adjust this value.
    /// </summary>
    public int TotalAttacks { get; set; }

    /// <summary>
    /// Gets or sets the quality target for the hit roll (minimum required result). Rules may adjust this value.
    /// </summary>
    public int Quality { get; set; }

    /// <summary>
    /// Gets or sets the armor penetration modifier applied to the defender's saves. Rules may adjust this value.
    /// </summary>
    public int ArmorPenetration { get; set; }

    /// <summary>
    /// True when the current attack is being made by the charging side.
    /// </summary>
    public bool IsCharging { get; }

    /// <summary>
    /// True if the weapon being used is a ranged weapon.
    /// </summary>
    public bool IsRanged { get; }
}

/// <summary>
/// Context provided to rules that trigger after hit rolls are resolved.
/// </summary>
public sealed class AfterHitContext
{
    internal AfterHitContext(
        UnitProfile attacker,
        UnitProfile defender,
        WeaponProfile weapon,
        ProbabilityDistribution<HitState> distribution,
        bool isCharging,
        int totalAttacks,
        int qualityTarget)
    {
        Attacker = attacker;
        Defender = defender;
        Weapon = weapon;
        Distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
        DefenseModifiers = new DefenseModifiers();
        IsCharging = isCharging;
        TotalAttacks = totalAttacks;
        QualityTarget = qualityTarget;
    }

    public UnitProfile Attacker { get; }

    public UnitProfile Defender { get; }

    public WeaponProfile Weapon { get; }

    public ProbabilityDistribution<HitState> Distribution { get; private set; }

    public DefenseModifiers DefenseModifiers { get; }

    public bool IsCharging { get; }

    /// <summary>
    /// Total number of attack dice that were resolved to produce the current hit distribution.
    /// </summary>
    public int TotalAttacks { get; }

    /// <summary>
    /// The quality target (minimum required die roll) used when computing the hit distribution.
    /// </summary>
    public int QualityTarget { get; }

    /// <summary>
    /// Completely replaces the current distribution. Use with care.
    /// </summary>
    public void ReplaceDistribution(ProbabilityDistribution<HitState> distribution)
    {
        Distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
    }

    /// <summary>
    /// Applies a transformation function to each state in the distribution.
    /// </summary>
    public void Map(Func<HitState, HitState> projector)
    {
        if (projector is null)
        {
            throw new ArgumentNullException(nameof(projector));
        }

        Distribution = Distribution.Select(projector);
    }

    /// <summary>
    /// Applies a probabilistic transformation to the distribution.
    /// </summary>
    public void Expand(Func<HitState, ProbabilityDistribution<HitState>> selector)
    {
        if (selector is null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        Distribution = Distribution.SelectMany(selector);
    }
}

/// <summary>
/// Context provided to rules that trigger after defensive saves have been made.
/// </summary>
public sealed class AfterDefenseContext
{
    internal AfterDefenseContext(UnitProfile attacker, UnitProfile defender, WeaponProfile weapon, ProbabilityDistribution<DefenseState> distribution)
    {
        Attacker = attacker;
        Defender = defender;
        Weapon = weapon;
        Distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
        RegenerationEnabled = true;
    }

    public UnitProfile Attacker { get; }

    public UnitProfile Defender { get; }

    public WeaponProfile Weapon { get; }

    public ProbabilityDistribution<DefenseState> Distribution { get; private set; }

    /// <summary>
    /// When set to false, defender-side regeneration effects should not be applied.
    /// </summary>
    public bool RegenerationEnabled { get; set; }

    public void ReplaceDistribution(ProbabilityDistribution<DefenseState> distribution)
    {
        Distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
    }

    public void Map(Func<DefenseState, DefenseState> projector)
    {
        if (projector is null)
        {
            throw new ArgumentNullException(nameof(projector));
        }

        Distribution = Distribution.Select(projector);
    }

    public void Expand(Func<DefenseState, ProbabilityDistribution<DefenseState>> selector)
    {
        if (selector is null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        Distribution = Distribution.SelectMany(selector);
    }
}
