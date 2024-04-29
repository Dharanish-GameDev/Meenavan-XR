using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WormGrab : XRGrabInteractable
{
    private IXRSelectInteractor interactorController;
    private bool isSelectedByDirectController;


    public bool IsSelectedByDirectController { get { return isSelectedByDirectController; } }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        interactorController = args.interactorObject;
        isSelectedByDirectController = false;
        if (interactorController as XRBaseController != GameManager.Instance.RightHandDirectController) return;
        isSelectedByDirectController = true;
        GameManager.Instance.ChangeCommonAudioSrcPos(interactorController.transform.position);
        AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.grabSfx, GameManager.Instance.CommonAudioSrc, 0.75f);
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        interactorController = null;
        isSelectedByDirectController = false;
    }
}
