using BattleSim.Core.Models;
using BattleSim.Core.SimulationV2;

namespace BattleSim.Core.Rules.Context;

public record BeforeDefenseContext(UnitProfile Attacker, UnitProfile Defender, HitRollState RollState, int ArmorPenetration, int Defense);