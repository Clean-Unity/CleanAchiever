using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class ObjectivesWorker {

    // MARK: Variables

    /// <summary>
    /// Inner representation of game objectives index.
    /// </summary>
    private static int _gameObjectivesIndex
    {
        get
        {
            return PlayerPrefs.GetInt(PlayerPrefsKeys.gameObjectivesIndex, 0);
        }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.gameObjectivesIndex, value);
        }
    }

    /// <summary>
    /// Lv[1-N]Obj[1-3] is the inner representation of the corresponding objective for level
    /// </summary>
    private struct _lvObjAccessor
    {
        public int this[int level, int obj] 
        {  
            get
            {
                return PlayerPrefs.GetInt(PlayerPrefsKeys.buildObjectiveKey(level, obj), 0);
            }
            set
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.buildObjectiveKey(level, obj), value);
            }
        }  
    }

    /// <summary>
    /// Lv[1-N]Status[1-3] is the inner representation of the corresponding status for level
    /// </summary>
    private struct _lvStatusAccessor
    {
        public int this[int level, int status]
        {
            get
            {
                return PlayerPrefs.GetInt(PlayerPrefsKeys.buildObjectiveStatusKey(level, status), 0);
            }
            set
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.buildObjectiveStatusKey(level, status), value);
            }
        }
    }

    private static int GAME_OBJECTIVES_COUNT = 6;

    // MARK: Public Interface

    public static int ReadIndex(Objectives.ObjectiveSet set)
    {
        return (set == Objectives.ObjectiveSet.Game) ? _gameObjectivesIndex : 0;
    }

    public static Objectives.ObjectivesModel[] Read(int nb, Objectives.ObjectiveSet set)
    {
        var objectives = new List<Objectives.ObjectivesModel>();
        var objective = new Objectives.ObjectivesModel();

        //==== STEP 1: Build raw Objectives
        if (nb == 0)
            return objectives.ToArray();

        switch (set)
        {
            //----- GAME -----
            case Objectives.ObjectiveSet.Game:
                //game objectives begin at index 0
                objective.id = 0;
                objective.currentStep = 0;
                objective.numberOfSteps = 10;
                objective.title = "Hit the Yellow Buzzer";
                objective.description = "10 times !";
                objective.reward = 100;
                objective.type = Objectives.ObjectiveTypes.Yellow;
                objective.isLocal = true;
                objectives.Add(objective);

                objective.id = 1;
                objective.currentStep = 0;
                objective.numberOfSteps = 10;
                objective.title = "Hit the Green Buzzer";
                objective.description = "10 times !";
                objective.reward = 200;
                objective.type = Objectives.ObjectiveTypes.Green;
                objective.isLocal = true;
                objectives.Add(objective);

                objective.id = 2;
                objective.currentStep = 0;
                objective.numberOfSteps = 10;
                objective.title = "Hit the Blue Buzzer";
                objective.description = "10 times !";
                objective.reward = 300;
                objective.type = Objectives.ObjectiveTypes.Blue;
                objective.isLocal = true;
                objectives.Add(objective);

                objective.id = 3;
                objective.currentStep = 0;
                objective.numberOfSteps = 2;
                objective.title = "Hit the Yellow Buzzer";
                objective.description = "20 times !";
                objective.reward = 400;
                objective.type = Objectives.ObjectiveTypes.Yellow;
                objective.isLocal = true;
                objectives.Add(objective);

                objective.id = 4;
                objective.currentStep = 0;
                objective.numberOfSteps = 2;
                objective.title = "Hit the Green Buzzer";
                objective.description = "20 times !";
                objective.reward = 500;
                objective.type = Objectives.ObjectiveTypes.Green;
                objective.isLocal = true;
                objectives.Add(objective);

                objective.id = 5;
                objective.currentStep = 0;
                objective.numberOfSteps = 2;
                objective.title = "Hit the Blue Buzzer";
                objective.description = "20 times !";
                objective.reward = 600;
                objective.type = Objectives.ObjectiveTypes.Blue;
                objective.isLocal = true;
                objectives.Add(objective);

                GAME_OBJECTIVES_COUNT = objectives.Count;
                break;
            default:
                return new Objectives.ObjectivesModel[] { };
        }

        //==== STEP 2: Update indexed Objectives with stored progress
        var objectivesProgressAccessor = new _lvObjAccessor();
        var objectivesClaimAccessor = new _lvStatusAccessor();

        if (set == Objectives.ObjectiveSet.Game)
        {
            for (int i = 0; i < objectives.Count; i++)
            {
                var obj = objectives[i];
                obj.currentStep = objectivesProgressAccessor[0, (i + 1)];
                obj.isRewardClaimed = objectivesClaimAccessor[0, (i + 1)] == 1 ? true : false;
                objectives[i] = obj;
            }
        }
        else
        {
            for (int i = 0; i < objectives.Count; i++)
            {
                var obj = objectives[i];
                obj.currentStep = objectivesProgressAccessor[(int)set, (i + 1)];
                obj.isRewardClaimed = objectivesClaimAccessor[(int)set, (i + 1)] == 1 ? true : false;
                objectives[i] = obj;
            }
        }

        //==== STEP 3: Select at correct index if we are indexing game wide objectives
        var indexedObjectiveSet = (set == Objectives.ObjectiveSet.Game) ? objectives.ToArray().Skip(_gameObjectivesIndex).Take(nb).ToArray()
                                                                        : objectives.ToArray().Take(nb).ToArray();

        return indexedObjectiveSet;
    }

    public static void Update(int id, Objectives.ObjectiveSet set, int newValue)
    {
        var objectivesProgressAccessor = new _lvObjAccessor();

        switch (set)
        {
            case Objectives.ObjectiveSet.Game:
                //game objectives begin at index 0
                var computedIndex = _gameObjectivesIndex + id;
                objectivesProgressAccessor[0, computedIndex + 1] = newValue;
                break;
            default:
                //worlds objectives begin at index 100
                int level = (int)set;
                objectivesProgressAccessor[level, id + 1] = newValue;
                break;
        }
    }

    public static void UpdateStatus(int id, Objectives.ObjectiveSet set, bool newValue)
    {
        var objectivesClaimAccessor = new _lvStatusAccessor();

        switch (set)
        {
            case Objectives.ObjectiveSet.Game:
                //game objectives begin at index 0
                var computedIndex = _gameObjectivesIndex + id;
                objectivesClaimAccessor[0, computedIndex + 1] = newValue ? 1 : 0;
                break;
            default:
                //worlds objectives begin at index 100
                int level = (int)set;
                objectivesClaimAccessor[level, id + 1] = newValue ? 1 : 0;
                break;
        }
    }

    public static Objectives.ObjectivesModel[] Flush(int nb)
    {
        _gameObjectivesIndex = (_gameObjectivesIndex + nb) % GAME_OBJECTIVES_COUNT;
        Debug.LogWarning("[ObjectivesWorker] Index Flushed : " + _gameObjectivesIndex);
        return Read(nb, Objectives.ObjectiveSet.Game);
    }

    public static void Reset()
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
