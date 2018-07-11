using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectivesInteractor {
    void doFetch(Objectives.FetchRequest request);
    void doFlush(Objectives.FlushRequest request);
    void doStep(Objectives.StepRequest request);
    void doClaim(Objectives.ClaimRequest request);
    void doReset(Objectives.ResetRequest request);
}

public class ObjectivesInteractor : IObjectivesInteractor
{
    // MARK: Properties

    public IObjectivesPresenter presenter { private get; set; }

    private Objectives.ObjectiveSet fetchedSet;
    public Objectives.ObjectivesModel[] fetchedObjectives;

    private int numberOfObjectives = 3;

    // MARK: Fetch

    public void doFetch(Objectives.FetchRequest request)
    {
        var response = new Objectives.FetchResponse();

        numberOfObjectives = request.numberOfObjectives;
        fetchedSet = request.set;

        this.fetchedObjectives = ObjectivesWorker.Read(numberOfObjectives, fetchedSet);
        response.objectives = this.fetchedObjectives;
        response.currentIndex = ObjectivesWorker.ReadIndex(fetchedSet); 

        this.presenter.presentFetch(response);
    }

    // MARK: Flush

    public void doFlush(Objectives.FlushRequest request)
    {
        const int FLUSH_COST = 100;

        LocalMoneyWorker.Instance.Buy(FLUSH_COST, delegate (int amount)
        {
            var response = new Objectives.FlushResponse();
            if (amount >= 0)
            {
                //since we are flushing, we should reset progress dans status
                for (int obj = 0; obj < numberOfObjectives; obj++)
                {
                    ObjectivesWorker.Update(obj, fetchedSet, 0);
                    ObjectivesWorker.UpdateStatus(obj, fetchedSet, false);
                }

                response.objectives = ObjectivesWorker.Flush(numberOfObjectives);

                //since we flushed data we fetch objectives again
                var fetchRequest = new Objectives.FetchRequest();
                fetchRequest.numberOfObjectives = numberOfObjectives;
                fetchRequest.set = fetchedSet;
                doFetch(fetchRequest);
            }
            else //Flush failed
            {
                response.objectives = new Objectives.ObjectivesModel[] { };
            }
            this.presenter.presentFlush(response);
        });
    }

    // MARK: Step

    public void doStep(Objectives.StepRequest request)
    {
        var response = new Objectives.StepResponse();

        if (fetchedObjectives[request.id].currentStep < fetchedObjectives[request.id].numberOfSteps)
        {
            fetchedObjectives[request.id].currentStep = fetchedObjectives[request.id].currentStep + 1;
        }

        ObjectivesWorker.Update(request.id, fetchedSet, fetchedObjectives[request.id].currentStep);

        response.objective = fetchedObjectives[request.id];

        //since we stepped data we fetch objectives again
        var fetchRequest = new Objectives.FetchRequest();
        fetchRequest.numberOfObjectives = numberOfObjectives;
        fetchRequest.set = fetchedSet;
        doFetch(fetchRequest);

        presenter.presentStep(response);
    }

    // MARK: Claim

    public void doClaim(Objectives.ClaimRequest request)
    {
        var response = new Objectives.ClaimResponse();

        //invalid index
        if (request.id >= this.fetchedObjectives.Length)
        {
            response.isRewardGranted = false;
            this.presenter.presentClaim(response);
            return;
        }
        //max step not reached
        if (this.fetchedObjectives[request.id].currentStep != this.fetchedObjectives[request.id].numberOfSteps)
        {
            response.isRewardGranted = false;
            this.presenter.presentClaim(response);
            return;
        }

        var reward = this.fetchedObjectives[request.id].reward;
        LocalMoneyWorker.Instance.Sell(reward, delegate(int amount) {

            //everything is fine, we can update claim status
            //fetchedObjectives[request.id].isRewardClaimed = true;
            ObjectivesWorker.UpdateStatus(request.id, fetchedSet, true);

            //since we flushed data we fetch objectives again
            var fetchRequest = new Objectives.FetchRequest();
            fetchRequest.numberOfObjectives = numberOfObjectives;
            fetchRequest.set = fetchedSet;
            doFetch(fetchRequest);

            //if all Game Objectives are complete and claimed, we should autoflush
            if (fetchedSet == Objectives.ObjectiveSet.Game)
            {
                bool shouldAutoFlush = true;
                foreach (var objective in fetchedObjectives)
                {
                    if (false == objective.isRewardClaimed)
                    {
                        shouldAutoFlush = false;
                    }
                }

                if (shouldAutoFlush)
                {
                    var flushRequest = new Objectives.FlushRequest();
                    doFlush(flushRequest);
                }
            }

            response.isRewardGranted = true;
            this.presenter.presentClaim(response);
        });
    }

    // MARK: Reset

    public void doReset(Objectives.ResetRequest request)
    {
        ObjectivesWorker.Reset();

        //since we flushed data we fetch objectives again
        var fetchRequest = new Objectives.FetchRequest();
        fetchRequest.numberOfObjectives = numberOfObjectives;
        fetchRequest.set = fetchedSet;
        doFetch(fetchRequest);

        var response = new Objectives.ResetResponse();
        presenter.presentReset(response);
    }
}
