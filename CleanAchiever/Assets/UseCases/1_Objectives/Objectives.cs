using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Objectives
{

    /// <summary>
    /// Types of Objectives.
    /// </summary>
    public enum ObjectiveTypes
    {
        // Countables
        Scores,
        Kills,
        Hits,
        Boss,
        PowerItems,
        Time,

        // Special Events
        Power,
        Currency,
        UnlockLevel,
        UnlockCharacter,
        Stats,

        // Color Hits Events
        Yellow,
        Green,
        Blue,

        Other,
        _COUNT
    }

    public enum ObjectiveSet
    {
        Game,
        World1,
        World2,
        World3,
        World4,
        World5
    }

    /// <summary>
    /// An Objective is defined by its status, description and progress.
    /// We should fetch objectives per level.
    /// </summary>
    public struct ObjectivesModel
    {
        public int id;
        public ObjectiveTypes type;
        public string title;
        public string description;
        public bool isLocal;

        public int currentStep;
        public int numberOfSteps;
        public bool isRewardClaimed; //false by default, true when claim has been requested

        public int reward;
    }

    /// <summary>
    /// Objectives view model.
    /// </summary>
    [System.Serializable]
    public struct ObjectivesViewModel
    {
        public Text titleLabel;
        public Text descriptionLabel;
        public Text completionLabel;
        public Text rewardLabel;
        public Button claimButton;
        public Image starImage;
    };

    /// <summary>
    /// Fetch Objectives list
    /// </summary>
    public struct FetchRequest
    {
        public ObjectiveSet set;
        public int numberOfObjectives;
    }
    public struct FetchResponse
    {
        /// <summary>
        /// Objectives Index
        /// </summary>
        public int currentIndex;
        /// <summary>
        /// Objectives to be done
        /// </summary>
        public ObjectivesModel[] objectives;
    }

    /// <summary>
    /// Flush Objectives and return new ones
    /// </summary>
    public struct FlushRequest
    {
    }
    public struct FlushResponse
    {
        /// <summary>
        /// Objectives to be done
        /// </summary>
        public ObjectivesModel[] objectives;
    }

    /// <summary>
    /// Increase objective completion status by one step.
    /// @param id : objective index to be stepped
    /// </summary>
    public struct StepRequest
    {
        public int id;
    }
    public struct StepResponse
    {
        /// <summary>
        /// The updated objective.
        /// You can choose any custom presentation logic based on step and completion status values. 
        /// </summary>
        public ObjectivesModel objective;
    }

    public struct ClaimRequest
    {
        public int id;
    }
    public struct ClaimResponse
    {
        public bool isRewardGranted;
    }

    public struct ResetRequest
    {
    }
    public struct ResetResponse
    {
    }
}