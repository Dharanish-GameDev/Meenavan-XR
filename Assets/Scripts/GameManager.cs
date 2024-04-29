using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private AudioSource XrRigAudioSource;
    [SerializeField] private AudioSource l_HandAudioSource;
    [SerializeField] private AudioSource commonAudioSrc;
    [SerializeField] private float boatCameraYOffset;
    [SerializeField] private float groundCameraYOffset;
    [SerializeField] private XROrigin XROrigin;
    [SerializeField] private XRBaseController rightHandDirectController;

    #region Public Variables
    [HideInInspector]
    public int KoiCount, CrappiesCount, TinCount, BootCount;

    #endregion

    public AudioSource XRrigAudioSource { get { return XrRigAudioSource; } }
    public AudioSource LHandAudioSource { get { return l_HandAudioSource; } }
    public AudioSource CommonAudioSrc { get { return commonAudioSrc; } }
    public float BoatCameraYOffset { get { return boatCameraYOffset; } }
    public float GroundCameraYOffeset { get { return groundCameraYOffset; } }

    public XRBaseController RightHandDirectController { get { return rightHandDirectController; } }
    #region LifeCycleMethods
    private void Awake()
    {
        if (Instance == null) Instance = this;
        GetOldScore();
        SetAndDisplayScore();
        ChangeCameraYOffset(groundCameraYOffset);
    }
    #endregion

    #region Public Methods

    public void ClearScore()
    {
        KoiCount = 0; CrappiesCount = 0; TinCount = 0; BootCount = 0;
        PlayerPrefs.SetInt("KoiCount", 0);
        PlayerPrefs.SetInt("CrappiesCount", 0);
        PlayerPrefs.SetInt("TinCount", 0);
        PlayerPrefs.SetInt("Boot", 0);
    }
    public void GetOldScore()
    {
        KoiCount = PlayerPrefs.GetInt("KoiCount");
        CrappiesCount = PlayerPrefs.GetInt("CrappiesCount");
        TinCount = PlayerPrefs.GetInt("TinCount");
        BootCount = PlayerPrefs.GetInt("BootCount");
    }  
    public void SaveScore()
    {
        PlayerPrefs.SetInt("KoiCount", KoiCount);
        PlayerPrefs.SetInt("CrappiesCount",CrappiesCount);
        PlayerPrefs.SetInt("TinCount", TinCount);
        PlayerPrefs.SetInt("BootCount", BootCount);

        PlayerPrefs.Save();
    }
    public void SetAndDisplayScore()
    {
        UI_Manager.instance.DisplayScoreText(KoiCount, CrappiesCount,TinCount,BootCount);
    }
    public void SendHapticImpulse(float frequency,float duration,XRBaseController controller)
    {
        controller.SendHapticImpulse(frequency,duration);
    }

    public void ChangeCameraYOffset(float yOffset)
    {
        XROrigin.CameraYOffset = yOffset;
    }
    public void ChangeCommonAudioSrcPos(Vector3 targetPos)
    {
        commonAudioSrc.transform.position = targetPos;
    }
    #endregion

}
