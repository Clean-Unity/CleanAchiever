using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IObjectivesObserver
{
    void update(Objectives.ObjectiveTypes objectiveType);
}

public class ObjectivesController : MonoBehaviour, IObjectivesObserver
{
    public ObjectivesInteractor interactor { private get; set; }

    private void OnEnable()
    {
        //entry point: fetch game wide objectives on enable so we can launch animation if needed
        fetchObjectives(Objectives.ObjectiveSet.Game, 3);
    }

    // MARK: Fetch

    public void fetchObjectives(Objectives.ObjectiveSet set, int numberOfObjectives = 3)
    {
        var request = new Objectives.FetchRequest();
        request.set = set;
        request.numberOfObjectives = numberOfObjectives;
        interactor.doFetch(request);
    }

    // MARK: Flush

    public void flushObjectives()
    {
        var request = new Objectives.FlushRequest();
        interactor.doFlush(request);
    }

    // MARK: Step

    public void step(int id)
    {
        Debug.Log("Stepping objective " + id);
        var request = new Objectives.StepRequest();
        request.id = id;
        interactor.doStep(request);
    }

    // MARK: Claim

    public void claimReward(int id)
    {
        Debug.Log("Claiming reward for objective " + id);
        var request = new Objectives.ClaimRequest();
        request.id = id;
        interactor.doClaim(request);
    }

    // MARK: Reset

    public void resetProgress()
    {
        var request = new Objectives.ResetRequest();
        interactor.doReset(request);
    }

    // MARK: IObjectivesObserver

    public void update(Objectives.ObjectiveTypes objectiveType)
    {
        //foreach objective in fetched list, we increment the progress and trigger the Step use case if needed
        for (int id = 0; id < interactor.fetchedObjectives.Length; id++)
        {
            if (interactor.fetchedObjectives[id].type == objectiveType)
            {
                step(id);
            }
        }
    }
}