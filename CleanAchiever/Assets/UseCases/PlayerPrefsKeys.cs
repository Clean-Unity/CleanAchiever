using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsKeys
{

    /// <summary>
    /// The index for game wide objectives.
    /// </summary>
    public static string gameObjectivesIndex = "gameOjectivesIndex";

    /// <summary>
    /// The objectives keys.
    /// - Lv0Obj[1-N] is the inner representation of the corresponding objective for game
    /// - Lv[1-N]Obj[1-3] is the inner representation of the corresponding objective for worlds
    /// </summary>
    public static string buildObjectiveKey(int level, int objective) {
        return "Lv" + level + "Obj" + objective;
    }

    /// <summary>
    /// The objectives statuses keys.
    /// - Lv0Status[1-N] is the inner representation of the corresponding objective for game
    /// - Lv[1-N]Status[1-3] is the inner representation of the corresponding objective for worlds
    /// </summary>
    public static string buildObjectiveStatusKey(int level, int status)
    {
        return "Lv" + level + "Status" + status;
    }

    /// <summary>
    /// The Game Money.
    /// </summary>
    public static string Money = "Money";

}
