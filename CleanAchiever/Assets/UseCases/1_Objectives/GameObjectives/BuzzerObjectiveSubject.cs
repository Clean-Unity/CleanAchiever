using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IObjectivesSubject
{
    void attach(IObjectivesObserver o);
    void detach(IObjectivesObserver o);
    void notify(Objectives.ObjectiveTypes objectiveType);
}

public class BuzzerObjectiveSubject : MonoBehaviour, IObjectivesSubject
{
    //MARK: Variables
    [SerializeField]
    private ObjectivesController objectivesObserver = null;

    private List<IObjectivesObserver> observers = new List<IObjectivesObserver>();

    //MARK: Lifecycle
	void OnEnable()
	{
        //you can specify the objective is observed for example when the gameObject is enabled
        attach(objectivesObserver);
	}

    void OnDisable()
    {
        //you can specify the objective is not observed for example when the gameObject is disabled
        detach(objectivesObserver);
    }

    public void OnBuzzerHit(int index)
    {
        Objectives.ObjectiveTypes objectiveType;

        switch (index)
        {
            case 0:
                objectiveType = Objectives.ObjectiveTypes.Yellow;
                break;
            case 1:
                objectiveType = Objectives.ObjectiveTypes.Green;
                break;
            default:
                objectiveType = Objectives.ObjectiveTypes.Blue;
                break;
        }

        notify(objectiveType);
    }

    // MARK: IObjectivesSubject

    public void attach(IObjectivesObserver o)
    {
        observers.Add(o);
    }

    public void detach(IObjectivesObserver o)
    {
        observers.Remove(o);
    }

    public void notify(Objectives.ObjectiveTypes objectiveType)
    {
        foreach (var o in observers)
        {
            o.update(objectiveType);
        }
    }
}
