using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XRContent.Interaction;
using UnityEngine;

public class FishCaught : MonoBehaviour
{
    public static FishCaught instance { get; private set; }

    #region Private variables

    [SerializeField] private GameObject worm;
    [SerializeField] private GameObject koiFish;
    [SerializeField] private GameObject CrappiesFish;
    [SerializeField] private GameObject boot;
    [SerializeField] private GameObject tin;

    [SerializeField] private FishingRodAnimationController animationController;

    private bool hasFish = false;
    private bool hasWorm = false;


    #endregion

    public bool HasFish { get { return hasFish; } } 
    public bool HasWorm { get { return hasWorm; } }

    public FishingRodAnimationController RodAnimationController { get { return animationController; } }

    #region LifeCycleMethods

    private void Awake()
    {
        if(instance == null) instance = this;
        DisableAllVisuals();
    }
    private void Start()
    {
        RandomHookMovement.Instance.OnFishCaught += Instance_OnFishCaught;
        RandomHookMovement.Instance.OnFishEscaped += Instance_OnFishEscaped;
        animationController.ResetAnimationParameters();
    }
    #endregion

    #region Private Methods
    private void Instance_OnFishCaught(SwimmableContext context)
    {
        DisableAllVisuals();
        UI_Manager.instance.EnableFishComment(context, 0);
        switch (context.SwimmableType)
        {
            case SwimmableType.KoiFish:
                koiFish.SetActive(true);
                UI_Manager.instance.ChangeFishImageAndText(0, "Koi");
                GameManager.Instance.KoiCount += 1;
                break;
            case SwimmableType.CrappiesFish:
                CrappiesFish.SetActive(true);
                UI_Manager.instance.ChangeFishImageAndText(1, "Crappies");
                GameManager.Instance.CrappiesCount += 1;
                break;
            case SwimmableType.Boot:
                boot.SetActive(true);
                UI_Manager.instance.ChangeFishImageAndText(3, "Boot");
                GameManager.Instance.BootCount += 1;
                break;
            case SwimmableType.Tin:
                tin.SetActive(true);
                UI_Manager.instance.ChangeFishImageAndText(2, "Tin");
                GameManager.Instance.TinCount+= 1;
                break;
        }
        FishManager.instance.PlaceFishAtRandomPoints(context);
        animationController.ResetAnimationParameters();
        FishingHook.Instance.ResetHookRotation();
        GameManager.Instance.SendHapticImpulse(0.85f, 1.5f, GameManager.Instance.RightHandDirectController);
    }
    private void DisableAllVisuals()
    {
        worm.SetActive(false);
        koiFish.SetActive(false);
        CrappiesFish.SetActive(false);
        boot.SetActive(false);  
        tin.SetActive(false);
        hasWorm = false;
    }
    private void Instance_OnFishEscaped(SwimmableContext context)
    {
        DisableAllVisuals();
        UI_Manager.instance.EnableFishComment(context, 1);
        hasFish = false;
        FishManager.instance.PlaceFishAtRandomPoints(context);
        animationController.ResetAnimationParameters();
        FishingHook.Instance.ResetHookRotation();
    }

    #endregion

    #region Public Methods
    public void FishCaughtUiClose()
    {
        DisableAllVisuals();
        hasFish = false;
    }
    public void EnableWormVisual()
    {
        worm.SetActive(true);
        hasWorm = true;
    }
    public void SethasFishTrue()
    {
        hasFish = true;
    }

    #endregion

}
