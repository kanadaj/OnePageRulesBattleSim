using System;
using System.Collections.Generic;

namespace BattleSim.Core.Simulation;

/// <summary>
/// Utilities for computing probability distributions over unordered d6 dice outcomes (multisets).
/// </summary>
public static class DiceCombinationDistribution
{
    /// <summary>
    /// Represents the multiset of results for N six-sided dice as counts per face (1..6).
    /// </summary>
    public readonly record struct DiceCounts(int Ones, int Twos, int Threes, int Fours, int Fives, int Sixes)
    {
        public int Total => Ones + Twos + Threes + Fours + Fives + Sixes;

        /// <summary>
        /// Expands the counts into a sorted array of face values, e.g., [1,1,5].
        /// </summary>
        public int[] ToFacesArraySorted()
        {
            var list = new List<int>(Math.Max(0, Total));

            void Add(int face, int count)
            {
                for (int i = 0; i < count; i++) list.Add(face);
            }

            Add(1, Ones);
            Add(2, Twos);
            Add(3, Threes);
            Add(4, Fours);
            Add(5, Fives);
            Add(6, Sixes);

            return list.ToArray();
        }

        /// <summary>
        /// Returns true if this multiset matches the given counts array [c1..c6].
        /// </summary>
        public bool Matches(params int[] counts)
        {
            if (counts is null || counts.Length != 6) return false;
            return Ones == counts[0] && Twos == counts[1] && Threes == counts[2] && Fours == counts[3] && Fives == counts[4] && Sixes == counts[5];
        }
    }

    /// <summary>
    /// Computes the multinomial probability distribution of unordered outcomes for rolling <paramref name="dice"/> fair d6.
    /// Keys are DiceCounts (counts of 1..6); values are probabilities that sum to ~1.
    /// </summary>
    public static ProbabilityDistribution<DiceCounts> GetDistribution(int dice)
    {
        if (dice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dice), "Number of dice must be non-negative.");
        }

        var outcomes = new Dictionary<DiceCounts, double>();
        var current = new int[6];
        GenerateCounts(dice, 0, current, outcomes);
        return new ProbabilityDistribution<DiceCounts>(outcomes);
    }

    /// <summary>
    /// Convenience: returns the distribution as face arrays (sorted) with probabilities.
    /// </summary>
    public static IReadOnlyList<(int[] Faces, double Probability)> GetFaceCombinations(int dice)
    {
        var dist = GetDistribution(dice);
        var list = new List<(int[] Faces, double Probability)>(dist.Probabilities.Count);
        foreach (var (counts, p) in dist.Probabilities)
        {
            list.Add((counts.ToFacesArraySorted(), p));
        }
        return list;
    }

    private static void GenerateCounts(int remaining, int index, int[] current, Dictionary<DiceCounts, double> outcomes)
    {
        if (index == 5)
        {
            current[5] = remaining;
            var counts = new DiceCounts(current[0], current[1], current[2], current[3], current[4], current[5]);
            var p = MultinomialProbability(counts, counts.Total);
            if (p > 0)
            {
                outcomes[counts] = p;
            }
            return;
        }

        for (int k = 0; k <= remaining; k++)
        {
            current[index] = k;
            GenerateCounts(remaining - k, index + 1, current, outcomes);
        }
    }

    private static double MultinomialProbability(DiceCounts c, int n)
    {
        if (n == 0)
        {
            return 1d;
        }

        // multinomial coefficient * (1/6)^n
        // n! / (c1! c2! ... c6!) * (1/6)^n
        var logCoef = LogFactorial(n)
                      - (LogFactorial(c.Ones) + LogFactorial(c.Twos) + LogFactorial(c.Threes) + LogFactorial(c.Fours) + LogFactorial(c.Fives) + LogFactorial(c.Sixes));
        var coef = Math.Exp(logCoef);
        return coef * Math.Pow(1d / 6d, n);
    }

    private static double LogFactorial(int n)
    {
        if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));
        // Use Stirling for large n to keep it stable; exact sum for small n
        if (n <= 1) return 0d;
        if (n < 64)
        {
            double s = 0d;
            for (int i = 2; i <= n; i++) s += Math.Log(i);
            return s;
        }
        // Stirling's approximation with 1/(12n) correction
        var x = n;
        return x * Math.Log(x) - x + 0.5 * Math.Log(2 * Math.PI * x) + 1.0 / (12.0 * x);
    }
}
