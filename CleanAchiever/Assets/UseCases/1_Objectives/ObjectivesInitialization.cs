using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesInitialization : MonoBehaviour {

    // MARK: Controller Initialization

    [SerializeField]
    private ObjectivesController controller = null;

    // MARK: Presenter Initialization

    [SerializeField]
    private ObjectivesPresenter presenter = null;

    [SerializeField]
    public Text gameObjectivesTitle; 

    [SerializeField]
    public Objectives.ObjectivesViewModel[] objectivesView;

    [SerializeField]
    public Sprite starOn;

    [SerializeField]
    public Sprite starOff;

	
	// MARK: MonoBehaviour
	
	void Awake() {
	  this.init();
	}
	
	// MARK: Initialization

	public void init () {
          #if UNITY_EDITOR || UNITY_REMOTE
            if (this.controller == null)
                Debug.LogError("ObjectivesController not set properly !");
            if (this.presenter == null)
                Debug.LogError("ObjectivesPresenter not set properly !");
            if (this.objectivesView == null)
                Debug.LogError("objectivesView not set properly !");
          #endif

          this.initController();
	}

    private void initController() {

    	var interactor = new ObjectivesInteractor();
    	this.controller.interactor = interactor;
        interactor.presenter = presenter;

        //map claim buttons actions to corresponding use case
        for (int index = 0; index < objectivesView.Length; index++)
        {
            int currentIndex = index;
            objectivesView[currentIndex].claimButton.onClick.AddListener(delegate { controller.claimReward(currentIndex); });
        }
   	}
}