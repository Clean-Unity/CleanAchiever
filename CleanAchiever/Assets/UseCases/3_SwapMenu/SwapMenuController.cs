using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapMenuController : MonoBehaviour {

    [SerializeField]
    private GameObject[] menus;

	// Use this for initialization
	void Awake () {
        validate();
	}

    private void validate()
    {
        if(menus.Length == 0)
        {
            Debug.LogWarning("Swap Menus not set");
        }
    }

    public void selectMenu (int index)
    {
#if UNITY_ANDROID
        AndroidEscapeManager.Instance.UnRegisterCallback();
        doOnce = true;
#endif
        for (int i = 0; i < menus.Length; i++)
        {
            if (index == i)
            {
                menus[i].SetActive(true);
            }
            else
            {
                menus[i].SetActive(false);
            }
        }
    }
}
