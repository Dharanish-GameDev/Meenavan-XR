using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationAnchorWithFade : TeleportationArea
{
    [SerializeField] private Transform wormBucket;
    [SerializeField] private Transform slotForBucket;
    [SerializeField] private Transform wormBucketParent;
    [SerializeField] private GameObject teleportIndicator;
    #region Overridden Methods
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (teleportTrigger == TeleportTrigger.OnSelectEntered)
            StartCoroutine(FadeSequence(base.OnSelectEntered, args));
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (teleportTrigger == TeleportTrigger.OnSelectExited)
            StartCoroutine(FadeSequence(base.OnSelectExited, args));
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        if (teleportTrigger == TeleportTrigger.OnActivated)
            StartCoroutine(FadeSequence(base.OnActivated, args));
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        if (teleportTrigger == TeleportTrigger.OnDeactivated)
            StartCoroutine(FadeSequence(base.OnDeactivated, args));
    }
    #endregion

    #region Private Methods
    private IEnumerator FadeSequence<T>(UnityAction<T> action, T args)
        where T : BaseInteractionEventArgs
    {
        UI_Manager.instance.TeleportFadeIn();

        yield return UI_Manager.instance.currentCoroutine;
        action.Invoke(args);
        GameManager.Instance.ChangeCameraYOffset(GameManager.Instance.BoatCameraYOffset);

        UI_Manager.instance.TeleportFadeOut();
        wormBucket.position = slotForBucket.position;
        wormBucket.parent = wormBucketParent;
        teleportIndicator.SetActive(false);
    }

    #endregion
}
