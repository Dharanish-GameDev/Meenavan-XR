using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance { get; private set; } // Singleton

    #region Private Variables

    // Exposed in Editor
    [Header("Fade")]
    [SerializeField] private CanvasGroup teleportFadeCanvasGroup = null;
    [SerializeField] private GameObject boatFadeGameObject = null;
    [SerializeField] private BoatMotor boatMotor = null;

    [Header("Fish Caught")]
    [SerializeField] private Sprite[] fishSprites;
    [SerializeField] private Image fishImage;
    [SerializeField] private GameObject fishCaughtUI;
    [SerializeField] private Button fishCauhgtUiCloseButton;
    [SerializeField] private TextMeshProUGUI fishNameText;
    [SerializeField] private Transform mainCameraTransform;

    [Header("Score UI")]

    [SerializeField] private TextMeshProUGUI koiCountText;
    [SerializeField] private TextMeshProUGUI crappiesCountText;
    [SerializeField] private TextMeshProUGUI tinCountText;
    [SerializeField] private TextMeshProUGUI bootCountText;
    [SerializeField] private GameObject scoreUiCanvas;

    [Header("Pause UI")]
    [SerializeField] private GameObject pauseUIObject;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private Button resetScoreBtn;
    [SerializeField] private Button restartMenuCloseBtn;


    [Header("Fish Comment")]
    [SerializeField] private GameObject commentObject;
    [SerializeField] private GameObject whoopsObject;
    [SerializeField] private GameObject gotchaObject;
    [SerializeField] private AudioSource commentAudioSource;

    [Space(10)]
    [Header("Properties")]
    [SerializeField] private float SceneLoadFadeOutTime = 2;
    [SerializeField] private float teleportFadeTime = 0.25f;


    // Hidden in Editor
    private float fadeAlpha = 0.0f;

    #endregion

    public Coroutine currentCoroutine { get; private set; } = null;
    private bool IsFishcaughtUiIsActive { get { return fishCaughtUI.activeInHierarchy; } } 

    private void Awake()
    {
        if(instance == null) instance = this;
        StartFadeOutDuringSceneLoad();
        BoatFadeSetActive(false);
        scoreUiCanvas.SetActive(false);

        // Button Click Events
        fishCauhgtUiCloseButton.onClick.AddListener(() => 
        { 
            fishCaughtUI.SetActive(false);
            GameManager.Instance.ChangeCommonAudioSrcPos(fishCauhgtUiCloseButton.transform.position);
            AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.buttonClick, GameManager.Instance.CommonAudioSrc, 0.4f);
            FishCaught.instance.FishCaughtUiClose();
            FishingHook.Instance.OnReleasingTrigger();
            GameManager.Instance.SetAndDisplayScore();
        });
        restartBtn.onClick.AddListener(() => 
        { 
            currentCoroutine = StartCoroutine(FadeIn(teleportFadeTime));
            GameManager.Instance.ChangeCommonAudioSrcPos(restartBtn.transform.position);
            AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.buttonClick, GameManager.Instance.CommonAudioSrc, 0.4f);
            Invoke(nameof(RestartLevel), 0.5f); 
        });
        resetScoreBtn.onClick.AddListener(() => 
        {
            GameManager.Instance.ClearScore();
            GameManager.Instance.ChangeCommonAudioSrcPos(resetScoreBtn.transform.position);
            AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.buttonClick, GameManager.Instance.CommonAudioSrc, 0.4f);
            GameManager.Instance.SetAndDisplayScore();
        });
        mainMenuBtn.onClick.AddListener(() =>
        {
            currentCoroutine = StartCoroutine(FadeIn(teleportFadeTime));
            GameManager.Instance.ChangeCommonAudioSrcPos(mainMenuBtn.transform.position);
            AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.buttonClick, GameManager.Instance.CommonAudioSrc, 0.4f);
            Invoke(nameof(LoadMainMenu), 0.5f);
        });
        restartMenuCloseBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.ChangeCommonAudioSrcPos(restartMenuCloseBtn.transform.position);
            AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.buttonClick, GameManager.Instance.CommonAudioSrc, 0.4f);
            pauseUIObject.SetActive(false);
        });

        ActiveFishCongratUi(false);
        pauseUIObject.SetActive(false);
        commentObject.SetActive(false);
    }

    #region Public Methods
    public void TeleportFadeIn()
    {
        StopAllCoroutines();
        currentCoroutine = StartCoroutine(FadeIn(teleportFadeTime));
    }
    public void TeleportFadeOut()
    {
        StopAllCoroutines();
        currentCoroutine = StartCoroutine(FadeOut(teleportFadeTime));
    }
    public void BoatFadeSetActive(bool b)
    {
        boatFadeGameObject.SetActive(b);
    }
    public void EnableScoreUI()
    {
        if (boatMotor.isSelected || IsFishcaughtUiIsActive || pauseUIObject.activeInHierarchy) return;

        scoreUiCanvas.SetActive(true);
    }
    public void DisableScoreUI()
    {
        scoreUiCanvas.SetActive(false);
    }
    public void SetUiFill(Image image,Vector3 pos1,Vector3 pos2,float maxDist)
    {
        float distance = Vector3.Distance(pos1,pos2);
        float fillAmount = Mathf.Clamp01(distance / maxDist);
        image.fillAmount = 1 - fillAmount;
    }
    public void SetUiFill(Image image,float min,float max,float currentValue)
    {
        float fillAmount = Mathf.InverseLerp(min,max,currentValue);
        image.fillAmount = 1 - fillAmount;
    }
    public void SetFillColor(Image image,Color color)
    {
        image.color = color;
    }

    /// <summary>
    /// 0 - Koi , 1 - Crappiesh , 2 - Tin , 3 - Boot
    /// </summary>
    /// <param name="imageIndex"></param>
    /// <param name="text"></param>
    public void ChangeFishImageAndText(int imageIndex,string text)
    {
        fishImage.sprite = fishSprites[imageIndex];
        fishNameText.text = text;
    }
    public void DisplayScoreText(int koiCount,int crappiesCount,int tinCount,int bootCount)
    {
        koiCountText.text = "x" + koiCount;
        crappiesCountText.text = "x" + crappiesCount;
        tinCountText.text = "x" + tinCount;
        bootCountText.text = "x" + bootCount;
    }
    public void ActiveFishCongratUi(bool activity)
    {
        if (activity && !fishCaughtUI.activeInHierarchy)
        {
            MakeUIObjectAppearInfrontOfCamera(fishCaughtUI.transform,0.75f);
            GameManager.Instance.SaveScore();
        }
        fishCaughtUI.SetActive(activity);
    }
    public void ActivePauseUi()
    {
        if (fishCaughtUI.activeInHierarchy) return;
        pauseUIObject.SetActive(true);
        MakeUIObjectAppearInfrontOfCamera(pauseUIObject.transform,0.75f);
    }


    /// <summary>
    /// 0 For Gotcha , 1 for Whoops
    /// </summary>
    /// <param name="context"></param>
    /// <param name="commentRefInt"></param>
    public void EnableFishComment(SwimmableContext context,int commentRefInt)
    {
        commentObject.SetActive(true);
        gotchaObject.SetActive(commentRefInt.Equals(0));
        whoopsObject.SetActive(commentRefInt.Equals(1));
        commentObject.transform.position = new Vector3(context.transform.position.x, 0.156f,context.transform.position.z);
        commentObject.transform.forward = mainCameraTransform.forward;
        if(commentRefInt.Equals(0))
        {
            AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.gotchaSFX, commentAudioSource, 1);
        }
        else
        {
            AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.whoopsClip, commentAudioSource, 1);
        }
        Invoke(nameof(DisableFishComment), 1f);
    }
    #endregion

    #region Private Methods
    private void StartFadeOutDuringSceneLoad()
    {
        StopAllCoroutines();
        currentCoroutine = StartCoroutine(FadeOut(SceneLoadFadeOutTime));
    }
    private IEnumerator FadeIn(float timeDuration)
    {
        float temp = 0.0f;
        while (fadeAlpha <= 1)
        {
            SetFadeAlpha(temp / timeDuration);
            temp += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOut(float timeDuration)
    {
        float temp = 0.0f;
        while (fadeAlpha >= 0.0f)
        {
            SetFadeAlpha(1 - (temp / timeDuration));
            temp += Time.deltaTime;
            yield return null;
        }
    }
    private void SetFadeAlpha(float alpha)
    {
        fadeAlpha = alpha;
        teleportFadeCanvasGroup.alpha = fadeAlpha;
    }

    private void MakeUIObjectAppearInfrontOfCamera(Transform target,float distance)
    {
        Vector3 forwardNoY = mainCameraTransform.forward;
        forwardNoY.y = 0f; // Ignore vertical component
        Vector3 canvasPosition = mainCameraTransform.position + forwardNoY.normalized * distance; // Adjust 2f to desired distance
        
        Quaternion canvasRotation = Quaternion.LookRotation(forwardNoY, mainCameraTransform.up); // Ensure canvas faces the player
        canvasRotation.z = 0f;
        // Set canvas position and rotation
        target.transform.position = canvasPosition;
        target.transform.rotation = canvasRotation;
    }
    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void LoadMainMenu()
    {
        SceneManager.LoadScene(0); // 0 Means MainMenu
    }
    private void DisableFishComment()
    {
        commentObject.SetActive(false);
    }

    #endregion
}
