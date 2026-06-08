using System.Collections;
using NUnit.Framework;
using ScriptsToTest;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class PIDControllerTest
{
    private static PIDWalker walkerUnderTest;

    [UnityTest, Order(1)]
    public IEnumerator VerifySceneContents()
    {
        yield return EditorSceneManager.LoadSceneAsyncInPlayMode(
            "Assets/Scenes/SampleScene.unity",
            new LoadSceneParameters(LoadSceneMode.Single));

        yield return null; // Wait for 1 frame to resolve DI

        walkerUnderTest = GameObject.FindFirstObjectByType<PIDWalker>();

        Assert.That(walkerUnderTest, Is.Not.Null);
    }

    [UnityTest, Order(2)]
    public IEnumerator WalkerSetupTest()
    {
        float PGainTest = 1;
        float DGainTest = 1.25f;

        walkerUnderTest.Restart();
        walkerUnderTest._controller.ProportionalGain = PGainTest;
        walkerUnderTest._controller.DerivativeGain = DGainTest;

        yield return null;

        Assert.That(walkerUnderTest.transform.position.x, Is.EqualTo(0));
        Assert.That(walkerUnderTest._controller.ProportionalGain, Is.EqualTo(PGainTest));
        Assert.That(walkerUnderTest._controller.DerivativeGain, Is.EqualTo(DGainTest));
    }

    [UnityTest, Order(3)]
    public IEnumerator WalkerLongLeftWalkTest()
    {
        float destination = -6f;

        walkerUnderTest.SendWalkerTo(destination);

        yield return new WaitForSecondsRealtime(7);

        Assert.That(destination - walkerUnderTest.transform.position.x, Is.LessThan(0.1f));
        Assert.That(walkerUnderTest.GetLinearVel, Is.LessThan(0.01f));
    }
    
    [UnityTest, Order(4)]
    public IEnumerator WalkerLeftToRightFullWalkTest()
    {
        Assert.That(walkerUnderTest.transform.position.x, Is.LessThan(-5.9f).And.GreaterThan(-6.1f));
        float destination = 6f;

        walkerUnderTest.SendWalkerTo(destination);

        yield return new WaitForSecondsRealtime(10);

        Assert.That(destination - walkerUnderTest.transform.position.x, Is.LessThan(0.1f));
        Assert.That(walkerUnderTest.GetLinearVel, Is.LessThan(0.01f));
    }
    
    [UnityTest, Order(5)]
    public IEnumerator ResetWalkerAndMoveLowRightTest()
    {
        walkerUnderTest.Restart();
        float destination = 2f;

        yield return null;
        
        walkerUnderTest.SendWalkerTo(destination);

        yield return new WaitForSecondsRealtime(5);

        Assert.That(destination - walkerUnderTest.transform.position.x, Is.LessThan(0.1f));
        Assert.That(walkerUnderTest.GetLinearVel, Is.LessThan(0.01f));
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
    }
}