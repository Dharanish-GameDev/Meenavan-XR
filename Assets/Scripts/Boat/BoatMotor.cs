using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BoatMotor : XRBaseInteractable
{
    #region Private Variables

    //Exposed in Editor
    [Header("References")]
    [SerializeField] private Transform motorHandle;
    [SerializeField] private Transform boatTransform;
    [SerializeField] private Transform XROriginTransform;
    [SerializeField] private Transform XRParentTranform;
    [SerializeField] private Animator boatAnimator;
    [SerializeField] private GameObject waterSplash;
    [SerializeField] private AudioSource waterSplashAudioSource;

    [Space(10)]
    [Header("Properties")]
    [Range(0f, 25f)]
    [SerializeField] private float boatTurningSpeed;
    [Range(0f, 25f)]
    [SerializeField] private float boatFullTurningSpeed;
    [SerializeField] private float motorMinAngle;
    [SerializeField] private float motorMaxAngle;
    [Range(0f,5f)]
    [SerializeField] private float boatForwardSpeed;



    // Hidden in Editor
    private IXRSelectInteractor interactorController;
    private XRBaseController controller;
    private float boatRotation = 0;
    private float lookAngle;
    private Vector3 dir;
    private Quaternion targetRot;
    private Vector3 boatThrust = Vector3.zero;
    private Vector3 lastPositionVector;
    // Animation parameters
    private readonly int BoatAni = Animator.StringToHash("BoatAni");

    #endregion

    #region LifeCycle Methods

    protected override void Awake()
    {
        base.Awake();
        lastPositionVector = boatTransform.position;
        waterSplash.SetActive(false);
        waterSplashAudioSource.gameObject.SetActive(false);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        selectEntered.AddListener(OnGrab);
        selectExited.AddListener(OnRelease);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        selectEntered.RemoveListener(OnGrab);
        selectExited.RemoveListener(OnRelease);
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic) // Normal Update
        {
            
            if (isSelected)
            {
                UpdateAngle();
                UpdateBoatRotation();
                BoatSpeedAnimation(GetBoatSpeedCoefficient());
                BoatThrust();
                AudioManager.instance.SetVolume(waterSplashAudioSource, GetBoatSpeedCoefficient());
            }
            if(GetBoatSpeedCoefficient() == 0)
            {
                boatTransform.position = lastPositionVector;
            }
            lastPositionVector = boatTransform.position;

        }
    }
    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        UI_Manager.instance.BoatFadeSetActive(true);
        waterSplash.SetActive(true);
        waterSplashAudioSource.gameObject.SetActive(true);
    }
    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);
        UI_Manager.instance.BoatFadeSetActive(false);
        waterSplash.SetActive(false);
        waterSplashAudioSource.gameObject.SetActive(false);
    }
    #endregion

    #region Private Methods

    private void OnGrab(SelectEnterEventArgs args)
    {
        interactorController = args.interactorObject;
        var controllerInteractor = interactorController as XRBaseControllerInteractor;
        controller = controllerInteractor.xrController;
        SetXRoriginParentToBoat();
    }
    private void OnRelease(SelectExitEventArgs args)
    {
        interactorController = null;
        SetXROriginParentToXR();
    }
    private void UpdateAngle()   // Method Updates the angle of the Handle Towards interactor
    {
        lookAngle = Mathf.Atan2(CalculateDirection().x, CalculateDirection().z) * Mathf.Rad2Deg;
        lookAngle = Mathf.Clamp(lookAngle, motorMinAngle, motorMaxAngle);
        motorHandle.localRotation = Quaternion.Euler(0, 0, lookAngle); // Applying Angle
    }
    private Vector3 CalculateDirection() // Returns the Direction between the interactor and the Handle
    {
        dir = interactorController.GetAttachTransform(this).position - motorHandle.position;
        dir = transform.InverseTransformDirection(dir); // Converts WorldSpace Direction into LocalSpace Direction
        return dir.normalized; // Returning Normalized Direction
    }
    private void UpdateBoatRotation()  // Updates Boat Rotation as per Motor Rotation
    {
        if (GetBoatSpeedCoefficient() < 0.2f) return;
        if(Mathf.Abs(WrapAngle(motorHandle.localEulerAngles.z))!= 55 )
        {
            boatRotation = Mathf.Lerp(boatRotation, motorHandle.localEulerAngles.z, boatFullTurningSpeed * Time.deltaTime);
        }
        else if(Mathf.RoundToInt(motorHandle.localEulerAngles.z) == motorMaxAngle)
        {
            if (boatRotation > 360) boatRotation = 0;
            boatRotation += boatFullTurningSpeed * Time.deltaTime;
        }
        else if(Mathf.RoundToInt(motorHandle.localEulerAngles.z) ==  305)
        {
            if (boatRotation > 360) boatRotation = 0;
            boatRotation -= boatFullTurningSpeed * Time.deltaTime;
           
        }
        targetRot = Quaternion.Euler(0, boatRotation, 0);
        boatTransform.localRotation = Quaternion.Slerp(boatTransform.localRotation, targetRot, boatTurningSpeed * Time.deltaTime);

    }
    private void BoatThrust() // Moving Boat Forward
    {
         boatThrust.z = boatForwardSpeed * GetBoatSpeedCoefficient() * Time.deltaTime;
         boatTransform.Translate(boatThrust,Space.Self);
    }
    private float GetBoatSpeedCoefficient()  // Returns the Pressing strength of the Activate Trigger
    {
        if(interactorController!= null)
        {
            return controller.activateInteractionState.value;
        }
        else
        {
            return 0f;
        }
    }
    private void SetXRoriginParentToBoat() // Setting XR Origin's Parent to Boat For Moving with it and Setting its Parent again to its Previous parent When Not Moving
    {
        if (XROriginTransform.parent.Equals(boatTransform)) return;
        XROriginTransform.SetParent(boatTransform);
    }
    private void SetXROriginParentToXR() // i.e XR Parent Object where Xr Origin is Previously a child 
    {
        if (XROriginTransform.parent.Equals(XRParentTranform)) return;
        XROriginTransform.SetParent(XRParentTranform);
    }
    private void BoatSpeedAnimation(float speed)
    {
        boatAnimator.SetFloat(BoatAni, speed);
    } // Controlls the Speed of the Boat Floating on the Water
    private float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }
    #endregion
}
