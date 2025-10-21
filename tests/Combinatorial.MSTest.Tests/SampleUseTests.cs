using System.Collections.Concurrent;

namespace Combinatorial.MSTest.Tests;

[TestClass]
public class SampleUseTests
{
    private static ConcurrentDictionary<(string, string), byte> s_test1Inputs = null!;
    private static ConcurrentDictionary<(bool, bool?), byte> s_test2Inputs = null!;
    private static ConcurrentDictionary<int, byte> s_test3Inputs = null!;
    private static ConcurrentDictionary<int, byte> s_test4Inputs = null!;

    [ClassInitialize]
    public static void ClassInit(TestContext _)
    {
        s_test1Inputs = new();
        s_test2Inputs = new();
        s_test3Inputs = new();
        s_test4Inputs = new();
    }

    [TestMethod]
    [CombinatorialData]
    public void Test1(
        [CombinatorialValues("1", "2", "3")] string x,
        [CombinatorialValues("A", "B", "C", "D")] string y)
    {
        s_test1Inputs.TryAdd((x, y), 0);
    }

    [TestMethod]
    [CombinatorialData]
    public void Test2(
        bool x,
        bool? y)
    {
        s_test2Inputs.TryAdd((x, y), 0);
    }

    [TestMethod]
    [CombinatorialData]
    public void Test3([CombinatorialRange(from: 0, count: 6)] int x)
    {
        s_test3Inputs.TryAdd(x, 0);
    }

    [TestMethod]
    [CombinatorialData]
    public void Test4([CombinatorialRandomData(Count = 8)] int x)
    {
        s_test4Inputs.TryAdd(x, 0);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Assert.AreEqual(12, s_test1Inputs.Count);
        Assert.AreEqual(6, s_test2Inputs.Count);
        Assert.AreEqual(6, s_test3Inputs.Count);
        Assert.AreEqual(8, s_test4Inputs.Count);
    }
}
