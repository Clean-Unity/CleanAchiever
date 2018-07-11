using UnityEngine;
using System.Collections;

public class CloseMenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
    }

    // Update is called once per frame
    #if UNITY_ANDROID
    bool doOnce = true;
    void Update () {
		if (doOnce && S_popup && S_popup.m_IsOnScreen)
        {
            AndroidEscapeManager.Instance.RegisterCallback(CloseAction);
            doOnce = false;
        }
    }
    #endif

    void OnPress (bool isPressed) {
		
		if (enabled && !isPressed)
		{
            CloseAction();
        }
	}

    private void CloseAction ()
    {

#if UNITY_ANDROID
        AndroidEscapeManager.Instance.UnRegisterCallback();
        doOnce = true;
#endif
        FindObjectOfType<SwapMenuController>().selectMenu(1);
    }
}
