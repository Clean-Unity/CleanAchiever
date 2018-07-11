using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Soft currency management interface
/// </summary>
public interface IMoneyWorker
{
    void Buy(int amount, Action<int> callback);
    void Sell(int amount, Action<int> callback);
    void Reset(Action<int> callback);
}

/// <summary>
/// Local money worker.
/// Not safe, account operations should be server side and not stored in local files.
/// </summary>
public sealed class LocalMoneyWorker: IMoneyWorker
{

    private static readonly LocalMoneyWorker instance = new LocalMoneyWorker();
    static LocalMoneyWorker()
    {
    }
    private LocalMoneyWorker()
    {
    }
    public static IMoneyWorker Instance
    {
        get
        {
            return instance;
        }
    }

    public void Buy(int amount, Action<int> callback)
    {
        int oldAmount = PlayerPrefs.GetInt(PlayerPrefsKeys.Money, 0);
        int difference = oldAmount - amount;

        if (difference >= 0)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.Money, difference);
            Debug.LogWarning("[MoneyWorker] Transaction Confirmed");
            callback(difference);
        }
        else
        {
            Debug.LogWarning("[MoneyWorker] Transaction Refused");
            callback(-1);
        }
    }

    public void Sell(int amount, Action<int> callback)
    {
        int oldAmount = PlayerPrefs.GetInt(PlayerPrefsKeys.Money, 0);
        int difference = oldAmount + amount;

        PlayerPrefs.SetInt(PlayerPrefsKeys.Money, difference);
        Debug.LogWarning("[MoneyWorker] Sell Confirmed");
        callback(difference);
    }

    public void Reset(Action<int> callback)
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.Money, 0);
        Debug.LogWarning("[MoneyWorker] Reset Confirmed");
        callback(0);
    }
}
