using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class HandAnimationManager : MonoBehaviour
{
    [SerializeField] private InputActionProperty pinchAnimationAction;
    [SerializeField] private InputActionProperty gripAnimationAction;
    [SerializeField] private Animator handAnimator;
    [SerializeField] private XRDirectInteractor lefthandDirectInteractor;
    [SerializeField] private WormGrab wormInteractable;
    [SerializeField] private bool canPinchForGrip = false;
    private readonly int Trigger = Animator.StringToHash("Trigger");
    private readonly int Grip = Animator.StringToHash("Grip");

    private float gripValue;
    private float triggerValue;
    // Update is called once per frame
    void Update()
    {
        if(wormInteractable != null)
        {
            canPinchForGrip = wormInteractable.isSelected ? true : false;
        }
        else canPinchForGrip = false;
        triggerValue = canPinchForGrip ? gripAnimationAction.action.ReadValue<float>() : pinchAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat(Trigger, triggerValue);

        gripValue = canPinchForGrip ? 0 : gripAnimationAction.action.ReadValue<float>();
        //float actualGripValue = 
        handAnimator.SetFloat(Grip, gripValue);

       // if (lefthandDirectInteractor.interactablesSelected[0].isSelected)
    }

    // During Holding Worm the Pinch Animation Must Play.
}
//-90 x 180 z