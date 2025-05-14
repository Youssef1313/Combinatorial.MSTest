// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Ms-PL license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Combinatorial.MSTest;

// Adoped from https://github.com/AArnott/Xunit.Combinatorial/blob/78ee72cb6ad53ebdbf85c824ec38205ccd5986e7/src/Xunit.Combinatorial/CombinatorialDataAttribute.cs

/// <summary>
/// Provides a test method decorated with a <see cref="TestMethodAttribute"/>
/// with arguments to run every possible combination of values for the
/// parameters taken by the test method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class CombinatorialDataAttribute : Attribute, ITestDataSource
{
    /// <inheritdoc />
    public IEnumerable<object?[]> GetData(MethodInfo methodInfo)
    {
        ParameterInfo[]? parameters = methodInfo.GetParameters();
        if (parameters.Length == 0)
        {
            return [];
        }

        var values = new List<object?>[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            values[i] = ValuesUtilities.GetValuesFor(parameters[i]).ToList();
        }

        object[]? currentValues = new object[parameters.Length];
        return [.. FillCombinations(parameters, values, currentValues, 0)];
    }

    /// <summary>
    /// Produces a sequence of argument arrays that capture every possible
    /// combination of values.
    /// </summary>
    /// <param name="parameters">The parameters taken by the test method.</param>
    /// <param name="candidateValues">An array of each argument's list of possible values.</param>
    /// <param name="currentValues">An array that is being recursively initialized with a set of arguments to pass to the test method.</param>
    /// <param name="index">The index into <paramref name="currentValues"/> that this particular invocation should rotate through <paramref name="candidateValues"/> for.</param>
    /// <returns>A sequence of all combinations of arguments from <paramref name="candidateValues"/>, starting at <paramref name="index"/>.</returns>
    private static IEnumerable<object?[]> FillCombinations(ParameterInfo[] parameters, List<object?>[] candidateValues, object?[] currentValues, int index)
    {
        foreach (object? value in candidateValues[index])
        {
            currentValues[index] = value;

            if (index + 1 < parameters.Length)
            {
                foreach (object?[] result in FillCombinations(parameters, candidateValues, currentValues, index + 1))
                {
                    yield return result;
                }
            }
            else
            {
                // We're the tail end, so just produce the value.
                // Copy the array before returning since we're about to mutate currentValues
                object[] finalSet = new object[currentValues.Length];
                Array.Copy(currentValues, finalSet, currentValues.Length);
                yield return finalSet;
            }
        }
    }

    /// <inheritdoc />
    public string? GetDisplayName(MethodInfo methodInfo, object?[]? data)
        => TestDataSourceUtilities.ComputeDefaultDisplayName(methodInfo, data);
}
