using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class ObjectivesInteractorTests
{
    public ObjectivesInteractor sut;
    public ObjectivesPresenterSpy spy;

    [SetUp]
    public void setup()
    {
        sut = new ObjectivesInteractor();
        spy = new ObjectivesPresenterSpy();
        sut.presenter = spy;

        resetProgress();

        //ensure money is not an issue
        LocalMoneyWorker.Instance.Reset(delegate { });
        LocalMoneyWorker.Instance.Sell(1000, delegate { });
    }

    [TearDown]
    public void teardown()
    {
        resetProgress();
    }

    // MARK: - Test Doubles

    public class ObjectivesPresenterSpy : IObjectivesPresenter
    {
        public bool presentFetchCalled = false;
        public bool presentFlushCalled = false;
        public bool presentStepCalled = false;
        public bool presentClaimCalled = false;
        public bool presentResetCalled = false;
        public Objectives.FetchResponse lastFetchResponse;
        public Objectives.FlushResponse lastFlushResponse;
        public Objectives.StepResponse lastStepResponse;
        public Objectives.ClaimResponse lastClaimResponse;
        public Objectives.ResetResponse lastResetResponse;


        void IObjectivesPresenter.presentFetch(Objectives.FetchResponse response)
        {
            presentFetchCalled = true;
            lastFetchResponse = response;
        }

        void IObjectivesPresenter.presentFlush(Objectives.FlushResponse response)
        {
            presentFlushCalled = true;
            lastFlushResponse = response;
        }

        void IObjectivesPresenter.presentStep(Objectives.StepResponse response)
        {
            presentStepCalled = true;
            lastStepResponse = response;
        }

        void IObjectivesPresenter.presentClaim(Objectives.ClaimResponse response)
        {
            presentClaimCalled = true;
            lastClaimResponse = response;
        }

        void IObjectivesPresenter.presentReset(Objectives.ResetResponse response)
        {
            presentResetCalled = true;
            lastResetResponse = response;
        }
    }

    // MARK: - Fetch
    [Test]
    public void test_givenDefaultParameters_whenFetchCalled_thenAllObjectivesIdsAreUnique()
    {
        Debug.LogWarning("test_givenDefaultParameters_whenFetchCalled_thenAllObjectivesIdsAreUnique");
        buildFetchRequest(3, Objectives.ObjectiveSet.Game);
        var foundIds = new List<int>();
        foreach (var objective in spy.lastFetchResponse.objectives)
        {
            foundIds.Add(objective.id);
            Debug.LogWarning(objective.id);
        }

        Assert.IsTrue(foundIds.Distinct().Count() == foundIds.Count);
    }

    [Test]
    public void test_givenDefaultParameters_whenFetchCalled_thenExpectedNumberOfObjectivesAreReturned()
    {
        for (int i = 0; i < 3; i++)
        {
            // Game Set
            buildFetchRequest(i, Objectives.ObjectiveSet.Game);
        }
    }

    // MARK: - Flush

    [Test]
    public void test_givenDefaultParameters_whenFlushCalled_thenDifferentObjectivesListIsReturned()
    {
        var request = new Objectives.FlushRequest();
        sut.doFlush(request);

        Assert.IsTrue(spy.presentFlushCalled);
    }

    // MARK: - Step

    [Test]
    public void test_givenDefaultParameters_whenStepCalled_thenObjectiveIsUpdated()
    {
        buildFetchRequest(3, Objectives.ObjectiveSet.Game);

        var request = new Objectives.StepRequest();
        request.id = 0;
        sut.doStep(request);

        Assert.IsTrue(spy.presentStepCalled);
    }

    // MARK: - Integration Tests

    //--- GAME SET

    [Test]
    public void test_givenDefaultParameters_when_SCENARIO_1_ReadFlushUntilLoop_thenCorrectListObjectivesAreDisplayed()
    {
        var request = new Objectives.FlushRequest();
        var numberOfItems = 3;

        buildFetchRequest(numberOfItems, Objectives.ObjectiveSet.Game);

        Assert.IsTrue(spy.presentFetchCalled);
        Assert.AreEqual(spy.lastFetchResponse.objectives.Length, numberOfItems);
        Assert.AreEqual(spy.lastFetchResponse.objectives[0].id, 0);
        Assert.AreEqual(spy.lastFetchResponse.objectives[1].id, 1);
        Assert.AreEqual(spy.lastFetchResponse.objectives[2].id, 2);

        sut.doFlush(request);

        Assert.AreEqual(spy.lastFlushResponse.objectives.Length, numberOfItems);
        Assert.AreEqual(spy.lastFlushResponse.objectives[0].id, 3);
        Assert.AreEqual(spy.lastFlushResponse.objectives[1].id, 4);
        Assert.AreEqual(spy.lastFlushResponse.objectives[2].id, 5);

        buildFetchRequest(numberOfItems, Objectives.ObjectiveSet.Game);
        Assert.AreEqual(spy.lastFetchResponse.objectives[0].id, 3);
        Assert.AreEqual(spy.lastFetchResponse.objectives[1].id, 4);
        Assert.AreEqual(spy.lastFetchResponse.objectives[2].id, 5);

        sut.doFlush(request);

        //here we reach an objective overflow, looping to root objectives ...
        Assert.AreEqual(spy.lastFlushResponse.objectives.Length, numberOfItems);
        Assert.AreEqual(spy.lastFlushResponse.objectives[0].id, 0);
        Assert.AreEqual(spy.lastFlushResponse.objectives[1].id, 1);
        Assert.AreEqual(spy.lastFlushResponse.objectives[2].id, 2);

        buildFetchRequest(numberOfItems, Objectives.ObjectiveSet.Game);
        Assert.AreEqual(spy.lastFetchResponse.objectives[0].id, 0);
        Assert.AreEqual(spy.lastFetchResponse.objectives[1].id, 1);
        Assert.AreEqual(spy.lastFetchResponse.objectives[2].id, 2);
    }

    [Test]
    public void test_givenDefaultParameters_when_SCENARIO_2_ReadStepUntilMaxStep_thenAllObjectivesProgressesAreFull()
    {
        var request = new Objectives.StepRequest();
        var numberOfItems = 3;

        //READ
        buildFetchRequest(numberOfItems, Objectives.ObjectiveSet.Game);
        Assert.AreEqual(spy.lastFetchResponse.objectives[0].id, 0);
        Assert.AreEqual(spy.lastFetchResponse.objectives[1].id, 1);
        Assert.AreEqual(spy.lastFetchResponse.objectives[2].id, 2);

        //STEP 0
        request.id = 0;
        sut.doStep(request);

        //we should prevent stepping as we already reached max value
        int MAX = spy.lastStepResponse.objective.numberOfSteps;
        Assert.AreEqual(Mathf.Min(1, MAX), spy.lastStepResponse.objective.currentStep);
        sut.doStep(request);
        Assert.AreEqual(Mathf.Min(2, MAX), spy.lastStepResponse.objective.currentStep);
        sut.doStep(request);
        Assert.AreEqual(Mathf.Min(3, MAX), spy.lastStepResponse.objective.currentStep);
        sut.doStep(request);
        Assert.AreEqual(Mathf.Min(4, MAX), spy.lastStepResponse.objective.currentStep);
        sut.doStep(request);
        Assert.AreEqual(Mathf.Min(5, MAX), spy.lastStepResponse.objective.currentStep);
        Assert.IsTrue(spy.presentStepCalled);

        //READ

        //indexes are not flushed
        buildFetchRequest(numberOfItems, Objectives.ObjectiveSet.Game);
        Assert.AreEqual(spy.lastFetchResponse.objectives[0].id, 0);
        Assert.AreEqual(spy.lastFetchResponse.objectives[1].id, 1);
        Assert.AreEqual(spy.lastFetchResponse.objectives[2].id, 2);

        //STEP 1
        request.id = 1;
        sut.doStep(request);

        //we should prevent stepping as we already reached max value
        MAX = spy.lastStepResponse.objective.numberOfSteps;
        Assert.AreEqual(Mathf.Min(1, MAX), spy.lastStepResponse.objective.currentStep);
        sut.doStep(request);
        Assert.AreEqual(Mathf.Min(2, MAX), spy.lastStepResponse.objective.currentStep);
        sut.doStep(request);
        Assert.AreEqual(Mathf.Min(3, MAX), spy.lastStepResponse.objective.currentStep);
        sut.doStep(request);
        Assert.AreEqual(Mathf.Min(4, MAX), spy.lastStepResponse.objective.currentStep);
        sut.doStep(request);
        Assert.AreEqual(Mathf.Min(5, MAX), spy.lastStepResponse.objective.currentStep);
        Assert.IsTrue(spy.presentStepCalled);

        //STEP 2
        request.id = 2;
        sut.doStep(request);

        //we should prevent stepping as we already reached max value
        MAX = spy.lastStepResponse.objective.numberOfSteps;
        Assert.AreEqual(Mathf.Min(1, MAX), spy.lastStepResponse.objective.currentStep);
        sut.doStep(request);
        Assert.AreEqual(Mathf.Min(2, MAX), spy.lastStepResponse.objective.currentStep);
        sut.doStep(request);
        Assert.AreEqual(Mathf.Min(3, MAX), spy.lastStepResponse.objective.currentStep);
        sut.doStep(request);
        Assert.AreEqual(Mathf.Min(4, MAX), spy.lastStepResponse.objective.currentStep);
        sut.doStep(request);
        Assert.AreEqual(Mathf.Min(5, MAX), spy.lastStepResponse.objective.currentStep);
        Assert.IsTrue(spy.presentStepCalled);

        //READ
        buildFetchRequest(numberOfItems, Objectives.ObjectiveSet.Game);
        Assert.AreEqual(spy.lastFetchResponse.objectives[0].id, 0);
        Assert.AreEqual(spy.lastFetchResponse.objectives[1].id, 1);
        Assert.AreEqual(spy.lastFetchResponse.objectives[2].id, 2);
    }

    [Test]
    public void test_givenDefaultParameters_when_SCENARIO_3_ReadStepUntilMaxStepClaim_thenAllObjectiveSetIsFlushed()
    {
        var request = new Objectives.StepRequest();
        var numberOfItems = 3;

        //READ
        buildFetchRequest(numberOfItems, Objectives.ObjectiveSet.Game);
        Assert.AreEqual(spy.lastFetchResponse.objectives[0].id, 0);
        Assert.AreEqual(spy.lastFetchResponse.objectives[1].id, 1);
        Assert.AreEqual(spy.lastFetchResponse.objectives[2].id, 2);

        //STEP ALL TO MAX
        for (int index = 0; index < numberOfItems; index++)
        {
            request.id = index;
            sut.doStep(request);
            int MAX = spy.lastStepResponse.objective.numberOfSteps;
            Assert.AreEqual(Mathf.Min(1, MAX), spy.lastStepResponse.objective.currentStep);

            for (int i = 2; i < MAX + 5; i++)
            {
                sut.doStep(request);
                Assert.AreEqual(Mathf.Min(i, MAX), spy.lastStepResponse.objective.currentStep);
            }
            Assert.IsTrue(spy.presentStepCalled);
            Assert.IsTrue(spy.lastStepResponse.objective.numberOfSteps == spy.lastStepResponse.objective.currentStep);
        }

        //CLAIM ALL SHOULD TRIGGER FLUSH FOR GAME
        var claimRequest = new Objectives.ClaimRequest();
        for (int index = 0; index < numberOfItems; index++)
        {
            claimRequest.id = index;
            sut.doClaim(claimRequest);
            Assert.IsTrue(spy.lastClaimResponse.isRewardGranted);
        }
        Assert.IsTrue(spy.presentFlushCalled);

        //READ 2
        buildFetchRequest(numberOfItems, Objectives.ObjectiveSet.Game);
        Assert.AreEqual(spy.lastFetchResponse.objectives[0].id, 3);
        Assert.AreEqual(spy.lastFetchResponse.objectives[1].id, 4);
        Assert.AreEqual(spy.lastFetchResponse.objectives[2].id, 5);
    }

    [Test]
    public void test_givenDefaultParameters_when_SCENARIO_4_ClaimCalledWithoutPermission_thenErrorIsReturned()
    {
        var request = new Objectives.ClaimRequest();
        request.id = 0;

        //objectives progress is 0 thus cannot claim
        buildFetchRequest(3, Objectives.ObjectiveSet.Game);
        sut.doClaim(request);

        Assert.IsTrue(spy.presentClaimCalled);
        Assert.IsFalse(spy.lastClaimResponse.isRewardGranted);
    }

    //--- WORLDS SET
    //TODO

    // MARK: - Private Methods

    private void buildFetchRequest(int number, Objectives.ObjectiveSet set)
    {
        var request = new Objectives.FetchRequest();
        request.numberOfObjectives = number;
        request.set = set;
        sut.doFetch(request);

        Assert.IsTrue(spy.presentFetchCalled);
        Assert.AreEqual(spy.lastFetchResponse.objectives.Length, request.numberOfObjectives);
    }

    private void resetProgress()
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.gameObjectivesIndex, 0);

        //here we consider 1 section and 6 objectives per section
        for (int lv = 0; lv < 1; lv++)
        {
            for (int obj = 1; obj <= 6; obj++)
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.buildObjectiveKey(lv, obj), 0);
                PlayerPrefs.SetInt(PlayerPrefsKeys.buildObjectiveStatusKey(lv, obj), 0);
            }
        }
    }
}
