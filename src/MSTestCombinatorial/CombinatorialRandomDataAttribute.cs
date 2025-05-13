// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Ms-PL license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace MSTestCombinatorial;

// Adopted from https://github.com/AArnott/Xunit.Combinatorial/blob/78ee72cb6ad53ebdbf85c824ec38205ccd5986e7/src/Xunit.Combinatorial/CombinatorialRandomDataAttribute.cs

/// <summary>
/// Specifies which range of values for this parameter should be used for running the test method.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class CombinatorialRandomDataAttribute : Attribute, ICombinatorialValuesProvider
{
    /// <summary>
    /// Special seed value to create System.Random class without seed.
    /// </summary>
    public const int NoSeed = 0;

    private object[]? _values;

    /// <summary>
    /// Gets or sets the number of values to generate. Must be positive.
    /// </summary>
    /// <value>The default value is 5.</value>
    public int Count { get; set; } = 5;

    /// <summary>
    /// Gets or sets the minimum value (inclusive) that may be randomly generated.
    /// </summary>
    /// <value>The default value is 0.</value>
    public int Minimum { get; set; }

    /// <summary>
    /// Gets or sets the maximum value (inclusive) that may be randomly generated.
    /// </summary>
    /// <value>The default value is <c><see cref="int.MaxValue"/> - 1</c>, which is the maximum allowable value.</value>
    public int Maximum { get; set; } = int.MaxValue - 1;

    /// <summary>
    /// Gets or sets the seed to use for random number generation.
    /// </summary>
    /// <value>The default value of <see cref="NoSeed"/> allows for a new seed to be used each time.</value>
    public int Seed { get; set; } = NoSeed;

    /// <inheritdoc />
    public object[] GetValues(ParameterInfo parameter)
    {
        return _values ??= this.GenerateValues();
    }

    private object[] GenerateValues()
    {
        if (this.Count < 1)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The value for {0} must be positive.", nameof(this.Count)));
        }

        if (this.Minimum > this.Maximum)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The value of {0} must not be greater than the value of {1}.", nameof(this.Minimum), nameof(this.Maximum)));
        }

        int maxPossibleValues = this.Maximum - this.Minimum + 1;
        if (this.Count > maxPossibleValues)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "{0} must not exceed the length of the range from {1} to {2}.", nameof(this.Count), nameof(this.Minimum), nameof(this.Maximum)));
        }

        Random random = this.Seed != NoSeed ? new Random(this.Seed) : new Random();

        HashSet<int> collisionChecker = new HashSet<int>();
        object[] values = new object[this.Count];
        int collisionCount = 0;
        int i = 0;
        while (collisionChecker.Count < this.Count)
        {
            int value = random.Next(this.Minimum, this.Maximum + 1);
            if (collisionChecker.Add(value))
            {
                values[i++] = value;
            }
            else
            {
                collisionCount++;
            }

            if (collisionCount > collisionChecker.Count * 5 && collisionCount > 1000)
            {
                // We have collided in random values far more than we have successfully generated values.
                // Rather than spin in this loop, throw.
                throw new InvalidOperationException("We are unable to generate the desired number of unique random values because too many non-unique random numbers are coming from the random number generator. Try reducing your target count or expanding your allowed range.");
            }
        }

        return values;
    }
}