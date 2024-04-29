using System.Collections;
using System.Collections.Generic;
using Unity.XRContent.Interaction;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using WrappingRopeLibrary.Scripts;

public class FishingRodGrab : XRGrabInteractable
{

    #region Private Variables

    // Exposed in Editor

    [Header("References")]
    [SerializeField] private SphereCollider rodRotatorCollider;
    [SerializeField] private Rope rope;
    [SerializeField] private ReelRotator reelRotator; // From 0 to 1; min = 0.1f to max 1
    [SerializeField] private ActionBasedSnapTurnProvider snapTurnProvider;
    [SerializeField] private XRBaseInteractor rodInteractor;
    [SerializeField] private Rigidbody hookRb;

    [Space(10)]
    [Header("Properties")]
    [SerializeField] private float swingThreshold = 0.6f;
    [Header("Rope Properties")]
    [SerializeField] private float minRopeLength = 0.5f; //Default 1
    [SerializeField] private float maxRopeLength = 4;

    
    [Space(5)]



    // Hidden In Editor
    private float currentRopeLength = 0f;
    private Vector3 lastPosition;
    private float waitTime = 1;
    private float pushHookWaitTime = 1f;
    private IXRSelectInteractor selectInteractor = null;
    private Vector3 velocity;
    private float forwardVelocity;
    private Transform mainCameraTransform;

    #endregion

    public bool pushHook = false;

    public ReelRotator ReelRotator { get { return reelRotator; } }
    public float MinRopelength { get { return minRopeLength; } }
    public float MaxRopeLength { get { return maxRopeLength; } }
    public Rope FishingRope { get { return rope; } }

    public bool IsGrabbedByHands
    {
        get 
        { 
            if(selectInteractor != null)
            {
                if (selectInteractor == rodInteractor as IXRSelectInteractor) return false;
                else return true;
            }
            else return false;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        for(int i=0 ;i<colliders.Count;i++)
        {
            var collider = colliders[i];
            if(collider == rodRotatorCollider)
            {
                colliders.RemoveAt(i);
            }
        }

        lastPosition = transform.position;
        selectEntered.AddListener(ResetTimeRestrictionForSwing);
        mainCameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        rope.SetRopeLength(minRopeLength);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        selectInteractor = args.interactorObject;
        if (selectInteractor as XRBaseController != GameManager.Instance.RightHandDirectController) return;
        GameManager.Instance.ChangeCommonAudioSrcPos(selectInteractor.transform.position);
        AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.grabSfx, GameManager.Instance.CommonAudioSrc, 0.75f);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        selectInteractor = null;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        // Check if the object is currently grabbed
        if (isSelected)
        {
            if (!IsGrabbedByHands) return;
            currentRopeLength = Mathf.Lerp(minRopeLength, maxRopeLength, 1 - reelRotator.Value);
            rope.SetRopeLength(currentRopeLength);
            if (!DomainExpansion.Instance.IsDomainExpanded) return;
            if (waitTime > 0f)  // Setting Time Limit Cuz the Velocity exceeds the Swing Threshold When Grabbed
            {
                waitTime -= Time.deltaTime;
                return;
            }
            // Calculate the current velocity of the object
            velocity = (transform.position - lastPosition) / Time.deltaTime;

            // Check if the object is moving forward (in the direction of its forward axis)
            forwardVelocity = Vector3.Dot(velocity, transform.right);

            
            // If the forward velocity is above the threshold, consider it a swing forward
            if (forwardVelocity > swingThreshold)
            {
                
                if (snapTurnProvider.InputValue.x == 0f) // Prevents Increase of the Thread Length While Snap Turning
                {
                    reelRotator.Value -= 6f * Time.deltaTime;
                    hookRb.transform.forward = mainCameraTransform.forward;
                    hookRb.AddForce(hookRb.gameObject.transform.forward * 10, ForceMode.Impulse);
                    pushHook = true;
                }
            }
            if (pushHook)
            {
                if (pushHookWaitTime > 0)
                {
                    pushHookWaitTime -= Time.deltaTime;
                }
                else
                {
                    pushHook = false;
                }
            }
            // Update the last position for the next frame
            lastPosition = transform.position;
        }

    }

    #region Private Methods

    private void ResetTimeRestrictionForSwing(SelectEnterEventArgs args)
    {
        waitTime = 4;
        pushHookWaitTime = 3;
    }
    #endregion

}
