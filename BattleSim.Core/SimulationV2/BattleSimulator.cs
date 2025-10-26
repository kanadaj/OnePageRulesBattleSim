// using BattleSim.Core.Models;
// using BattleSim.Core.Rules;
// using BattleSim.Core.Simulation;
//
// namespace BattleSim.Core.SimulationV2;
//
// public class BattleSimulator
// {
//     /// <summary>
//     /// Simulates a single round of combat, allowing weapon special rules to adjust initiative order.
//     /// </summary>
//     public BattleResult Simulate(UnitProfile attacker, UnitProfile defender, SimulationMode mode = SimulationMode.Melee)
//     {
//         if (attacker is null)
//         {
//             throw new ArgumentNullException(nameof(attacker));
//         }
//
//         if (defender is null)
//         {
//             throw new ArgumentNullException(nameof(defender));
//         }
//
//         return mode switch
//         {
//             SimulationMode.Melee => SimulateMelee(attacker, defender),
//             SimulationMode.Ranged => SimulateRanged(attacker, defender),
//             _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
//         };
//     }
//
//     private BattleResult SimulateMelee(UnitProfile attacker, UnitProfile defender)
//     {
//         var attackerWeapons = GetWeapons(attacker);
//         var attackerHeroWeapons = GetWeapons(attacker.Hero);
//         var defenderWeapons = GetWeapons(defender);
//         var defenderHeroWeapons = GetWeapons(defender.Hero);
//
//         ProbabilityDistribution<WoundState> attackerWoundState = new();
//         ProbabilityDistribution<WoundState> defenderWoundState = new();
//
//         foreach (var attack in defenderWeapons.Where(x => x.Rules.Any(y => y.Name == "Counter")))
//         {
//             var woundState = ResolveAttacks(attack.Unit, attack.Weapon, attack.Rules, attacker, defender);
//             attackerWoundState = attackerWoundState.Combine(woundState);
//         }
//
//         foreach (var attack in defenderHeroWeapons.Where(x => x.Rules.Any(y => y.Name == "Counter")))
//         {
//             var woundState = ResolveAttacks(attack.Unit, attack.Weapon, attack.Rules, attacker, defender);
//             attackerWoundState = attackerWoundState.Combine(woundState);
//         }
//         
//         foreach (var attack in attackerWeapons)
//         {
//             var woundState = ResolveAttacks(attack.Unit, attack.Weapon, attack.Rules, defender, attacker);
//             defenderWoundState = defenderWoundState.Combine(woundState);
//         }
//
//         if (attacker.Hero is not null)
//         {
//             foreach (var attack in attackerHeroWeapons)
//             {
//                 var woundState = ResolveAttacks(attack.Unit, attack.Weapon, attack.Rules, defender, attacker);
//                 defenderWoundState = defenderWoundState.Combine(woundState);
//             }
//         }
//         
//         foreach (var attack in defenderWeapons.Where(x => x.Rules.Any(y => y.Name != "Counter")))
//         {
//             var woundState = ResolveAttacks(attack.Unit, attack.Weapon, attack.Rules, attacker, defender);
//             attackerWoundState = attackerWoundState.Combine(woundState);
//         }
//         
//         if (defender.Hero is not null)
//         {
//             foreach (var attack in defenderHeroWeapons.Where(x => x.Rules.Any(y => y.Name != "Counter")))
//             {
//                 var woundState = ResolveAttacks(attack.Unit, attack.Weapon, attack.Rules, attacker, defender);
//                 defenderWoundState = defenderWoundState.Combine(woundState);
//             }
//         }
//
//         return GenerateBattleReport(attackerWoundState, defenderWoundState);
//     }
//
//     private ProbabilityDistribution<WoundState> ResolveAttacks<TOther>(UnitProfile attackUnit, WeaponProfile weapon, IEnumerable<IAttackRule> attackRules, UnitProfile attacker, UnitProfile defender)
//     {
//         
//     }
//     
//
//     private static ProbabilityDistribution<DefenseState> ResolveDefense(
//         ProbabilityDistribution<HitState> hitDistribution,
//         UnitProfile defender,
//         int armorPenetration,
//         DefenseModifiers defenseModifiers)
//     {
//         return hitDistribution.SelectMany(hitState =>
//         {
//             var totalHits = Math.Max(0, hitState.TotalHits);
//             if (totalHits <= 0)
//             {
//                 var wounds = Math.Max(0, hitState.DirectWounds);
//                 return ProbabilityDistribution<DefenseState>.Certain(new DefenseState(hitState, wounds));
//             }
//
//             var naturalSixHits = Math.Clamp(hitState.NaturalSixes, 0, totalHits);
//             var regularHits = totalHits - naturalSixHits;
//
//             var baseArmorPenetration = armorPenetration + defenseModifiers.AdditionalArmorPenetration + hitState.ArmorPenetrationBonus;
//             var rerollSixes = defenseModifiers.ForceDefenseSixReroll;
//
//             var unsavedRegular = ComputeUnsavedProbability(defender.Defense, baseArmorPenetration, rerollSixes);
//             var unsavedSix = ComputeUnsavedProbability(
//                 defender.Defense,
//                 baseArmorPenetration + defenseModifiers.AdditionalArmorPenetrationOnNaturalSix,
//                 rerollSixes);
//
//             var regularDistribution = BuildBinomialDistribution(regularHits, unsavedRegular);
//             var sixDistribution = BuildBinomialDistribution(naturalSixHits, unsavedSix);
//
//             return regularDistribution.Combine(sixDistribution, static (regular, six) => regular + six)
//                 .Select(total => new DefenseState(hitState, total + hitState.DirectWounds));
//         });
//     }
//     
//     private static ProbabilityDistribution<int> BuildBinomialDistribution(int trials, double successProbability)
//     {
//         if (trials <= 0)
//         {
//             return ProbabilityDistribution<int>.Certain(0);
//         }
//     
//         successProbability = Math.Clamp(successProbability, 0d, 1d);
//         var outcomes = new Dictionary<int, double>();
//     
//         var combination = 1d;
//         for (var k = 0; k <= trials; k++)
//         {
//             if (k == 0)
//             {
//                 combination = 1d;
//             }
//             else
//             {
//                 combination *= (double)(trials - (k - 1)) / k;
//             }
//     
//             var probability = combination * Math.Pow(successProbability, k) * Math.Pow(1d - successProbability, trials - k);
//             if (probability <= 0)
//             {
//                 continue;
//             }
//     
//             outcomes[k] = probability;
//         }
//     
//         return new ProbabilityDistribution<int>(outcomes);
//     }
//
//     private List<(UnitProfile Unit, WeaponProfile Weapon, IEnumerable<IAttackRule> Rules)> GetWeapons(UnitProfile? unit)
//     {
//         if (unit is null)
//         {
//             return [];
//         }
//         return unit.Weapons.Select(weapon => (unit, x: weapon, CombinedRules(unit.AttackRules, weapon.Rules.OfType<IAttackRule>(), unit.Hero?.Auras.OfType<IAttackRule>()))).ToList();
//     }
//
//     private IEnumerable<T> CombinedRules<T>(params ICollection<IEnumerable<T>?> rules) where T : IRule
//     {
//         return rules.Where(x => x != null).SelectMany(x => x).Aggregate(new Dictionary<string, T>(), (results, currentRule) =>
//         {
//             var ruleType = currentRule.Name;
//             if (currentRule is IAdditiveRule additiveRule)
//             {
//                 if (results.TryGetValue(ruleType, out var r) && r is IAdditiveRule existingRule)
//                 {
//                     results[ruleType] = existingRule.Merge(currentRule);
//                 }
//                 else
//                 {
//                     results.Add(ruleType, currentRule);
//                 }
//             }
//             else
//             {
//                 results.Add(ruleType, currentRule);
//             }
//
//             return results;
//         }).Values.ToList();
//     } 
//     
//     private BattleResult SimulateRanged(UnitProfile attacker, UnitProfile defender)
//     {
//         throw new NotImplementedException();
//     }
// }