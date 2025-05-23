﻿// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Ms-PL license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Combinatorial.MSTest;

// Adopted from https://github.com/AArnott/Xunit.Combinatorial/blob/78ee72cb6ad53ebdbf85c824ec38205ccd5986e7/src/Xunit.Combinatorial/ValuesUtilities.cs

/// <summary>
/// Utility methods for generating values for test parameters.
/// </summary>
internal static class ValuesUtilities
{
    /// <summary>
    /// Gets a sequence of values that should be tested for the specified parameter.
    /// </summary>
    /// <param name="parameter">The parameter to get possible values for.</param>
    /// <returns>A sequence of values for the parameter.</returns>
    internal static IEnumerable<object?> GetValuesFor(ParameterInfo parameter)
    {
        ICombinatorialValuesProvider? valuesSource = parameter.GetCustomAttributes().OfType<ICombinatorialValuesProvider>().SingleOrDefault();
        if (valuesSource is not null)
        {
            return valuesSource.GetValues(parameter);
        }

        return GetValuesFor(parameter.ParameterType);
    }

    /// <summary>
    /// Gets a sequence of values that should be tested for the specified type.
    /// </summary>
    /// <param name="dataType">The type to get possible values for.</param>
    /// <returns>A sequence of values for the <paramref name="dataType"/>.</returns>
    internal static IEnumerable<object?> GetValuesFor(Type dataType)
    {
        if (dataType == typeof(bool))
        {
            yield return true;
            yield return false;
        }
        else if (dataType == typeof(int))
        {
            yield return 0;
            yield return 1;
        }
        else if (dataType.GetTypeInfo().IsEnum)
        {
            foreach (string name in Enum.GetNames(dataType))
            {
                yield return Enum.Parse(dataType, name);
            }
        }
        else if (IsNullable(dataType, out Type? innerDataType))
        {
            yield return null;
            foreach (object? value in GetValuesFor(innerDataType))
            {
                yield return value;
            }
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Determines whether <paramref name="dataType"/> is <see cref="Nullable{T}"/>
    /// and extracts the inner type, if any.
    /// </summary>
    /// <param name="dataType">
    /// The type to test whether it is <see cref="Nullable{T}"/>.
    /// </param>
    /// <param name="innerDataType">
    /// When this method returns, contains the inner type of the Nullable, if the
    /// type is Nullable is found; otherwise, null.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the type is a Nullable type; otherwise <see langword="false"/>.
    /// </returns>
    private static bool IsNullable(Type dataType, [NotNullWhen(true)] out Type? innerDataType)
    {
        innerDataType = null;

        TypeInfo? ti = dataType.GetTypeInfo();

        if (!ti.IsGenericType)
        {
            return false;
        }

        if (ti.GetGenericTypeDefinition() != typeof(Nullable<>))
        {
            return false;
        }

        innerDataType = ti.GenericTypeArguments[0];
        return true;
    }
}