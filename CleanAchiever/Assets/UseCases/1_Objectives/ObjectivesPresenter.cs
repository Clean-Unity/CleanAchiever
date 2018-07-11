using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IObjectivesPresenter {
	void presentFetch(Objectives.FetchResponse response);
    void presentFlush(Objectives.FlushResponse response);
    void presentStep(Objectives.StepResponse response);
    void presentClaim(Objectives.ClaimResponse response);
    void presentReset(Objectives.ResetResponse response);
}

public class ObjectivesPresenter : MonoBehaviour, IObjectivesPresenter {

    /// <summary>
    /// Quest Names - Standard Name is used on Overflow.
    /// </summary>
    private string[] QUEST_SET_NAMES =
    {
        "Quest 1 : \nThe Beginning !",
        "Quest 2 : \nA Great Adventure !",
        "Quest 3 : \nMastering my skills !",
        "Quest 4 : \nNot the Best yet !",
        "Quest 5 : \nKeeping focussed !",
        "Quest 6 : \nThe Best Bomber !",
        "Quest 7 : \nBomber Master !",
        "Quest 8 : \nSo Good !",
        "Quest 9 : \nLooking for challenges"
    };
    private const string DEFAULT_QUEST_SET_NAME = "Quest :;Quête :";

	// MARK: Fetch

    public void presentFetch(Objectives.FetchResponse response)
	{
        Debug.Log(response.objectives.Length + " Objectives Fetched !");

        var initializationScript = FindObjectOfType<ObjectivesInitialization>();
        var windowView = initializationScript.gameObjectivesTitle;

        var pageIndex = response.currentIndex / response.objectives.Length;
        windowView.text =   pageIndex < QUEST_SET_NAMES.Length ?
                            QUEST_SET_NAMES[pageIndex]
                            : DEFAULT_QUEST_SET_NAME;

        var objectivesViews = initializationScript.objectivesView;
        for (int i = 0; i < response.objectives.Length; i++)
        {
            objectivesViews[i].titleLabel.text = response.objectives[i].title;
            objectivesViews[i].descriptionLabel.text = response.objectives[i].description;
            objectivesViews[i].completionLabel.text = response.objectives[i].currentStep + " / " + response.objectives[i].numberOfSteps;
            objectivesViews[i].rewardLabel.text = "" + response.objectives[i].reward;
            objectivesViews[i].claimButton.interactable = response.objectives[i].currentStep == response.objectives[i].numberOfSteps && !response.objectives[i].isRewardClaimed;
            objectivesViews[i].starImage.sprite = response.objectives[i].currentStep == response.objectives[i].numberOfSteps ? initializationScript.starOn : initializationScript.starOff;
        }
	}

    // MARK: Flush

    public void presentFlush(Objectives.FlushResponse response)
    {
        if (response.objectives.Length > 0)
        {
            NotifierPresenter.Instance.PerformInstantaneousRequest("Flushed Objectives !");
        }
        else
        {
            NotifierPresenter.Instance.PerformInstantaneousRequest("You need more Money !");
        }
    }

    // MARK: Step

    public void presentStep(Objectives.StepResponse response)
    {
        if (response.objective.currentStep == response.objective.numberOfSteps)
        {
            NotifierPresenter.Instance.PerformInstantaneousRequest(""+ response.objective.title + " : Done !");
        }
        else
        {
            NotifierPresenter.Instance.PerformInstantaneousRequest("" + response.objective.title + " (" + response.objective.currentStep + " / " + response.objective.numberOfSteps + ") ");
        }
    }

    // MARK: Claim

    public void presentClaim(Objectives.ClaimResponse response)
    {
        if (response.isRewardGranted)
        {
            NotifierPresenter.Instance.PerformInstantaneousRequest("Cash Earned !");
        }
        else
        {
            NotifierPresenter.Instance.PerformInstantaneousRequest("Hum ... Cannot add more cash !");
        }
    }

    // MARK: Reset

    public void presentReset(Objectives.ResetResponse response)
    {
        NotifierPresenter.Instance.PerformInstantaneousRequest("Progress Reset !");
    }
}
