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

More complete minimized example:

```csharp
using System.Collections.Concurrent;
using Combinatorial.MSTest;

namespace YourAssembly.Tests;

[TestClass]
public class YourClassTests
{
    [TestMethod]
    [CombinatorialData]
    public void Test1(
        [CombinatorialValues("1", "2", "3")] string x,
        [CombinatorialValues("A", "B", "C", "D")] string y)
    {
        // This test is run with the following inputs:
        // x = "1", y = "A"
        // x = "1", y = "B"
        // x = "1", y = "C"
        // x = "1", y = "D"
        // x = "2", y = "A"
        // x = "2", y = "B"
        // x = "2", y = "C"
        // x = "2", y = "D"
        // x = "3", y = "A"
        // x = "3", y = "B"
        // x = "3", y = "C"
        // x = "3", y = "D"
    }

    [TestMethod]
    [CombinatorialData]
    public void Test2(
        bool x,
        bool? y)
    {
        // This test is run with the following inputs:
        // x = true, y = null
        // x = true, y = true
        // x = true, y = false
        // x = false, y = null
        // x = false, y = true
        // x = false, y = false
    }

    [TestMethod]
    [CombinatorialData]
    public void Test3([CombinatorialRange(from: 0, count: 6)] int x)
    {
        // This test is run with x values from 0 to 5.
    }

    [TestMethod]
    [CombinatorialData]
    public void Test4([CombinatorialRandomData(Count = 8)] int x)
    {
        // This test is run with 8 random integers.
    }
}
```
