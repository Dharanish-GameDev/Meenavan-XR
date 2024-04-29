using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RodHolderSocket : XRSocketInteractor
{
    [SerializeField] private GameObject hoverObjectVisual;
    [SerializeField] private GameObject placeYourRodHereNote;

    protected override void Awake()
    {
        base.Awake();
        hoverObjectVisual.SetActive(false);
    }

    public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractor(updatePhase);
        if(hasSelection)
        {
            hoverObjectVisual.SetActive(false);
            placeYourRodHereNote.SetActive(false);
        }
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        hoverObjectVisual.SetActive(true);
    }
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        hoverObjectVisual.SetActive(false);
        placeYourRodHereNote.SetActive(true);
    }
}
