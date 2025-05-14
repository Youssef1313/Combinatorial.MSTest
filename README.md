# Combinatorial.MSTest

> [!IMPORTANT]
> This project is heavily adopted from [Xunit.Combinatorial](https://github.com/AArnott/Xunit.Combinatorial) by [Andrew Arnott](https://github.com/AArnott), but bringing it for MSTest. Lots of the logic is a direct copy/paste from there.

> [!IMPORTANT]
> This is **not** an official package produced by MSTest team at Microsoft.

For examples, see [SampleUseTests](https://github.com/Youssef1313/Combinatorial.MSTest/blob/main/tests/Combinatorial.MSTest.Tests/SampleUseTests.cs).

In short, the attribute `CombinatorialData` must be added along with `TestMethod`. The values used for each parameter are determined by the following rules:

1. If the parameter has an attribute that implements `ICombinatorialValuesProvider`, the attribute's `GetValues` method is called to determine the possible values for the given parameter.
    - The following attributes are already shipped and implementing `ICombinatorialValuesProvider`:
        - `CombinatorialValuesAttribute`
        - `CombinatorialRangeAttribute`
        - `CombinatorialRandomDataAttribute`  
2. Otherwise, the possible values are determined depending on the parameter type:
    - `bool`: `true` and `false`
    - `int`: `0` and `1`
    - `enum`: The values as returned by `Enum.GetNames`
    - `Nullable<T>`: The possible values of `T` plus the `null` value.
    - Otherwise, an exception is thrown.
