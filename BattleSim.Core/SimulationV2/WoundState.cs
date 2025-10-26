namespace BattleSim.Core.SimulationV2;

public record WoundState(int WoundsTaken = 0, int NaturalOnes = 0, int NaturalSixes = 0)
{
    public static WoundState operator +(WoundState left, WoundState right)
    {
        return left with
        {
            WoundsTaken = left.WoundsTaken + right.WoundsTaken,
            NaturalOnes = left.NaturalOnes + right.NaturalOnes,
            NaturalSixes = left.NaturalSixes + right.NaturalSixes
        };
    }
}