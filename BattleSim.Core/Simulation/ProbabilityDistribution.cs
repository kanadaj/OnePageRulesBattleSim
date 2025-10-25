using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleSim.Core.Simulation;

/// <summary>
/// Represents a discrete probability distribution over values of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The outcome type.</typeparam>
public sealed class ProbabilityDistribution<T>
    where T : notnull
{
    private readonly Dictionary<T, double> _probabilities;

    public ProbabilityDistribution()
        : this(new Dictionary<T, double>())
    {
    }

    public ProbabilityDistribution(IDictionary<T, double> probabilities, double epsilon = 1e-12)
    {
        if (probabilities is null)
        {
            throw new ArgumentNullException(nameof(probabilities));
        }

        _probabilities = new Dictionary<T, double>();
        foreach (var (value, probability) in probabilities)
        {
            if (probability <= epsilon)
            {
                continue;
            }

            if (_probabilities.TryGetValue(value, out var current))
            {
                _probabilities[value] = current + probability;
            }
            else
            {
                _probabilities[value] = probability;
            }
        }

        Normalize();
    }

    private ProbabilityDistribution(Dictionary<T, double> probabilities)
    {
        _probabilities = probabilities;
        Normalize();
    }

    /// <summary>
    /// Gets the probability mass function for this distribution.
    /// </summary>
    public IReadOnlyDictionary<T, double> Probabilities => _probabilities;

    /// <summary>
    /// Creates a distribution with a single outcome that occurs with certainty.
    /// </summary>
    public static ProbabilityDistribution<T> Certain(T value)
    {
        return new ProbabilityDistribution<T>(new Dictionary<T, double> { [value] = 1.0 });
    }

    /// <summary>
    /// Projects each value in the distribution to a new type while preserving probability mass.
    /// </summary>
    public ProbabilityDistribution<TResult> Select<TResult>(Func<T, TResult> selector)
        where TResult : notnull
    {
        if (selector is null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        var output = new Dictionary<TResult, double>();
        foreach (var (value, probability) in _probabilities)
        {
            var projected = selector(value);
            if (output.TryGetValue(projected, out var current))
            {
                output[projected] = current + probability;
            }
            else
            {
                output[projected] = probability;
            }
        }

        return new ProbabilityDistribution<TResult>(output);
    }

    /// <summary>
    /// Applies a probabilistic transformation to the distribution.
    /// </summary>
    public ProbabilityDistribution<TResult> SelectMany<TResult>(Func<T, ProbabilityDistribution<TResult>> selector)
        where TResult : notnull
    {
        if (selector is null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        var result = new Dictionary<TResult, double>();
        foreach (var (value, probability) in _probabilities)
        {
            var inner = selector(value);
            foreach (var (innerValue, innerProbability) in inner._probabilities)
            {
                var combinedProbability = probability * innerProbability;
                if (combinedProbability <= 0)
                {
                    continue;
                }

                if (result.TryGetValue(innerValue, out var current))
                {
                    result[innerValue] = current + combinedProbability;
                }
                else
                {
                    result[innerValue] = combinedProbability;
                }
            }
        }

        return new ProbabilityDistribution<TResult>(result);
    }

    /// <summary>
    /// Combines this distribution with another using the provided aggregator.
    /// </summary>
    public ProbabilityDistribution<TResult> Combine<TResult, TOther>(ProbabilityDistribution<TOther> other, Func<T, TOther, TResult> aggregator)
        where TResult : notnull
        where TOther : notnull
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        if (aggregator is null)
        {
            throw new ArgumentNullException(nameof(aggregator));
        }

        var combined = new Dictionary<TResult, double>();
        foreach (var (leftValue, leftProbability) in _probabilities)
        {
            foreach (var (rightValue, rightProbability) in other._probabilities)
            {
                var probability = leftProbability * rightProbability;
                if (probability <= 0)
                {
                    continue;
                }

                var value = aggregator(leftValue, rightValue);
                if (combined.TryGetValue(value, out var current))
                {
                    combined[value] = current + probability;
                }
                else
                {
                    combined[value] = probability;
                }
            }
        }

        return new ProbabilityDistribution<TResult>(combined);
    }

    /// <summary>
    /// Aggregates the expectation value of the given selector.
    /// </summary>
    public double Expectation(Func<T, double> selector)
    {
        if (selector is null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        double sum = 0d;
        foreach (var (value, probability) in _probabilities)
        {
            sum += probability * selector(value);
        }

        return sum;
    }

    /// <summary>
    /// Gets the total probability mass contained in the distribution.
    /// </summary>
    public double TotalProbability => _probabilities.Values.Sum();

    /// <summary>
    /// Returns a new distribution that only keeps outcomes with probability above the provided threshold.
    /// </summary>
    public ProbabilityDistribution<T> Prune(double threshold = 1e-9)
    {
        if (threshold < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(threshold), "Threshold must be non-negative.");
        }

        var filtered = _probabilities.Where(pair => pair.Value >= threshold)
                                     .ToDictionary(pair => pair.Key, pair => pair.Value);
        return new ProbabilityDistribution<T>(filtered);
    }

    /// <summary>
    /// Enumerates the outcomes and their probabilities.
    /// </summary>
    public IEnumerable<KeyValuePair<T, double>> Enumerate() => _probabilities;

    private void Normalize()
    {
        var total = _probabilities.Values.Sum();
        if (total <= 0)
        {
            return;
        }

        if (Math.Abs(total - 1d) <= 1e-12)
        {
            return;
        }

        var scale = 1d / total;
        foreach (var key in _probabilities.Keys)
        {
            _probabilities[key] *= scale;
        }
    }
}
