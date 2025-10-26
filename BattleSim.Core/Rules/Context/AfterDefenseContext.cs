using BattleSim.Core.Models;
using BattleSim.Core.SimulationV2;

namespace BattleSim.Core.Rules.Context;

public record AfterDefenseContext(UnitProfile Attacker, UnitProfile Defender, DefenseRollState RollState);