using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button controlsBtn;
    [SerializeField] private Button howToPlayBtn;
    [SerializeField] private Button quitBtn;

    [SerializeField] private Button controlsCloseButton;
    [SerializeField] private Button htpCloseButton;
    [SerializeField] private Button voiceBtn;
    [SerializeField] private AudioSource voiceSource;

    [SerializeField] private CanvasGroup fadeCanvas;

    [Header("Canvas Objects")]
    [SerializeField] private GameObject menuObject;
    [SerializeField] private GameObject controlsUiObject;
    [SerializeField] private GameObject howToPlayObject;
    [SerializeField] private Animator mainMenuAnimator;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource bgSource;
    private bool canEnterGameScene = false;
    private float buttonPopSfxVolume = 0.8f;
    float temp = 0.0f;

    private void Awake()
    {
        menuObject.SetActive(true);
        controlsUiObject.SetActive(false);
        howToPlayObject.SetActive(false);
        mainMenuAnimator.enabled = false;

        startButton.onClick.AddListener(() =>
        {
            fadeCanvas.alpha = 1;
            canEnterGameScene = true;
            AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.buttonClick,audioSource,buttonPopSfxVolume);
        });
        controlsBtn.onClick.AddListener(() => 
        {
            menuObject.SetActive(false);
            controlsUiObject.SetActive(true);
            AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.buttonClick, audioSource,buttonPopSfxVolume);
        });
        howToPlayBtn.onClick.AddListener(() => 
        {
            menuObject.SetActive(false);
            howToPlayObject.SetActive(true);
            AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.buttonClick, audioSource,buttonPopSfxVolume);
        });
        quitBtn.onClick.AddListener(() => { Application.Quit();});
        voiceBtn.onClick.AddListener(() =>
        {
            if (!voiceSource.isPlaying)
            {
                voiceSource.Play();
                bgSource.volume = 0.3f;
            }

        });

        // Close Buttons
        controlsCloseButton.onClick.AddListener(() => 
        {
            controlsUiObject.SetActive(false);
            menuObject.SetActive(true);
            mainMenuAnimator.enabled = true;
            AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.buttonClick, audioSource, buttonPopSfxVolume);
        });
        htpCloseButton.onClick.AddListener(() =>
        {
            howToPlayObject.SetActive(false);
            menuObject.SetActive(true);
            mainMenuAnimator.enabled = true;
            AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.buttonClick, audioSource, buttonPopSfxVolume);
            bgSource.volume = 1f;
        });
        fadeCanvas.alpha = 1f;
    }
    private void Update()
    {
        if (fadeCanvas.alpha >= 0.0f && !canEnterGameScene)
        {
            fadeCanvas.alpha = (1 - (temp / 4));
            temp += Time.deltaTime;
        }
        if (canEnterGameScene)
        {
            if(fadeCanvas.alpha <= 1)
            {
                fadeCanvas.alpha = (temp / 2);
                temp += Time.deltaTime;
            }
            if (fadeCanvas.alpha >= 1)
            {
                SceneManager.LoadScene(1); // 1 Represents Game Scene
            }
        }
    }
}
