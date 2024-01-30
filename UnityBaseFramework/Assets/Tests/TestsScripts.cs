using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

public class TestsScripts
{
    // A Test behaves as an ordinary method
    [Test]
    public void StartTest()
    {
        // Use the Assert class to test conditions
        CommonTest();
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator StartCoroutineTest()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }

    public void CommonTest()
    {

    }
}
