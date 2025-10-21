using System.Linq;
using System.Reflection;

namespace Combinatorial.MSTest.Tests;

[TestClass]
public sealed class CombinatorialDataAttributeTests
{
    [TestMethod]
    public void TestWithTwoBooleans()
    {
        var attr = new CombinatorialDataAttribute();
        var data = attr.GetData(GetMethod(nameof(TwoBooleans))).ToArray();
        Assert.HasCount(4, data);
        CollectionAssert.AreEquivalent(new[] { true, true }, data[0]);
        CollectionAssert.AreEquivalent(new[] { true, false }, data[1]);
        CollectionAssert.AreEquivalent(new[] { false, true }, data[2]);
        CollectionAssert.AreEquivalent(new[] { false, false }, data[3]);
    }

    [TestMethod]
    public void TestWithBooleanAndRange()
    {
        var attr = new CombinatorialDataAttribute();
        var data = attr.GetData(GetMethod(nameof(BooleanAndRange))).ToArray();
        Assert.HasCount(6, data);
        CollectionAssert.AreEquivalent(new object[] { true, 1 }, data[0]);
        CollectionAssert.AreEquivalent(new object[] { true, 2 }, data[1]);
        CollectionAssert.AreEquivalent(new object[] { true, 3 }, data[2]);
        CollectionAssert.AreEquivalent(new object[] { false, 1 }, data[3]);
        CollectionAssert.AreEquivalent(new object[] { false, 2 }, data[4]);
        CollectionAssert.AreEquivalent(new object[] { false, 3 }, data[5]);
    }

    [TestMethod]
    public void TestWithBooleanAndValues()
    {
        var attr = new CombinatorialDataAttribute();
        var data = attr.GetData(GetMethod(nameof(BooleanAndValues))).ToArray();
        Assert.HasCount(6, data);
        CollectionAssert.AreEquivalent(new object[] { true, "1" }, data[0]);
        CollectionAssert.AreEquivalent(new object[] { true, "2" }, data[1]);
        CollectionAssert.AreEquivalent(new object[] { true, "3" }, data[2]);
        CollectionAssert.AreEquivalent(new object[] { false, "1" }, data[3]);
        CollectionAssert.AreEquivalent(new object[] { false, "2" }, data[4]);
        CollectionAssert.AreEquivalent(new object[] { false, "3" }, data[5]);
    }

    private static void TwoBooleans(bool a, bool b)
    {
    }

    private static void BooleanAndRange(bool a, [CombinatorialRange(from: 1, count: 3)] int b)
    {
    }

    private static void BooleanAndValues(bool a, [CombinatorialValues("1", "2", "3")] string b)
    {
    }

    private static MethodInfo GetMethod(string name)
        => typeof(CombinatorialDataAttributeTests).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)!;
}
