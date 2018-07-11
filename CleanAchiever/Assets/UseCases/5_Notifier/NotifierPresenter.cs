using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

public class NotifierPresenter : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// Singleton instance
    /// </summary>
    public static NotifierPresenter Instance = null;

    public GameObject Popup;
    public Image Sprite;
    public Text Label;

    [SerializeField]
    private Sprite OnProgressTexture;
    [SerializeField]
    private Sprite OnSuccessTexture;
    [SerializeField]
    private Sprite OnFailureTexture;
    [SerializeField]
    private Sprite TempTexture;

    /// <summary>
    /// Request models
    /// </summary>
    public enum RequestState
    {
        Started,
        InProgress,
        Success,
        Failure,
        _SIZE
    }

    private class Request
    {
        public int id;
        public string PendingText;
        public string SuccessText;
        public string FailureText;
        public RequestState State;
    }

    /// <summary>
    /// Requests List
    /// </summary>
    private List<Request> m_requests = new List<Request>();

    private bool m_isRequestPending = false;
    /// <summary>
    /// Is any Request Pending ?
    /// </summary>
    public bool IsRequestPending
    {
        get { return m_isRequestPending; }
    }
    private Coroutine m_pendingRequest = null;

    private int m_cptRequests = 0;
    #endregion

    #region Init
    void Awake()
    {
        if (Instance != null && Instance != this) // AsynchronousNotifier Singleton
        {
            Destroy(this);
            return;
        }
        if (transform.parent == null)
        {
            DontDestroyOnLoad(this.gameObject);
        }
        Instance = this;
        enabled = false;

        if (Popup == null || OnProgressTexture == null || OnSuccessTexture == null || OnFailureTexture == null)
        {
            Debug.LogWarning(" *** Warning : Please assign popup and default textures in the inspector");
        }
    }
    #endregion

    #region Engine
    #endregion

    #region Interface
    /// <summary>
    /// Perform An Instantaneous Request and Notify User
    /// </summary>
    /// <param name="OnProgressText"></param>
    /// <param name="OnSuccessText"></param>
    /// <param name="isSuccess"></param>
    public void PerformInstantaneousRequest(string OnProgressText, string OnSuccessText = "Success !", bool isSuccess = true)
    {
        StartCoroutine(Timeout(0.1f, OnProgressText, OnSuccessText, isSuccess));
    }

    public void PerformInstantaneousRequest(string text = "Success !")
    {
        StartCoroutine(Timeout(0.1f, text, text, true));
    }

	IEnumerator Timeout(float delay, string OnProgressText = "In progress ...", string OnSuccessText = "Success !", bool isSuccess = true)
    {
        int authRequestId;

        // Cache Unused Image
        TempTexture = OnFailureTexture;
		OnFailureTexture = OnSuccessTexture;

        authRequestId = StartRequest(OnProgressText, OnSuccessText, OnSuccessText);
        yield return new WaitForSeconds(delay);
        StopRequest(authRequestId, isSuccess);

		// Uncache Unused Image
        OnFailureTexture = TempTexture;
    }

    /// <summary>
    /// Call when starting a new Request
    /// </summary>
    /// <param name="OnProgressText">Text displayed during Request in Progress</param>
    /// <param name="OnSuccessText">Text displayed on Successful Request</param>
    /// <param name="OnFailureText">Text displayed on Failed Request</param>
    /// <returns>Id to the Registered Request</returns>
    public int StartRequest( string OnProgressText = "In progress ...", 
                            string OnSuccessText = "Success !", 
                            string OnFailureText = "Sorry, an error occured, please try again later ...")
    {
        // Add New Request to Requests List
        Request w_request = new Request();
        w_request.id = m_cptRequests++;
        w_request.PendingText = string.IsNullOrEmpty(OnProgressText) ? "In progress" : OnProgressText;
        w_request.SuccessText = string.IsNullOrEmpty(OnSuccessText) ? "Success !" : OnSuccessText;
        w_request.FailureText = string.IsNullOrEmpty(OnFailureText) ? "Sorry, an error occured, please try again later ..." : OnFailureText;
        w_request.State = RequestState.Started;
        m_requests.Add(w_request);

        // Launch Request if Available
        if (false == m_isRequestPending)
        {
            m_pendingRequest = StartCoroutine(LaunchRequest(m_requests[0]));
            if (m_pendingRequest != null)
            {
                m_isRequestPending = true;
            }
        }

        return w_request.id;
    }

    /// <summary>
    /// Call to Stop Request identified by given id
    /// </summary>
    /// <param name="id">Request identifier returned after creating request</param>
    /// <param name="isSuccess">Is the request successfull ?</param>
    public void StopRequest(int id, bool isSuccess = true)
    {
        // Stop Request if Running
        if (true == m_isRequestPending)
        {
            //w_request.State = (isSuccess) ? RequestState.Success : RequestState.Failure;
            m_requests[0].State = (isSuccess) ? RequestState.Success : RequestState.Failure;
            m_isRequestPending = false;
        }

        // Remove Request from Requests List
        //m_requests.Remove(w_request);
        if (m_requests.Count > 0)
            m_requests.RemoveAt(0);
    }

    IEnumerator LaunchRequest(Request request)
    {
        float w_i = 0;
        float w_rate = (1.0f / 2.0f);

        // Start ...
        request.State = RequestState.Started;
        Label.text = request.PendingText;
        //sprite.spriteName = OnProgressTexture;
        Sprite.sprite = OnProgressTexture;
        while (! m_isRequestPending)
        {
            yield return null;
        }

        // In Progress ...
        request.State = RequestState.InProgress;
        Label.text = request.PendingText;
        Sprite.sprite = OnProgressTexture;

        if (Popup)
        {
            Popup.SetActive(true);
        }

        while (m_isRequestPending)
        {
            w_i += w_rate * Time.deltaTime;
            if (w_i > 1) w_i = 0;
            Sprite.fillAmount = Mathf.Lerp(0, 1, w_i);
            yield return null;
        }
        Sprite.fillAmount = 1;

        // Finished ...
        Sprite.sprite = (request.State == RequestState.Success) ? OnSuccessTexture : OnFailureTexture;
        Label.text = (request.State == RequestState.Success) ? request.SuccessText : request.FailureText;
        yield return new WaitForSeconds(2.0f);

        if (Popup)
        {
            Popup.SetActive(false);
        }

        yield return new WaitForSeconds(1.0f);
        m_isRequestPending = false;
    }
    #endregion
}
