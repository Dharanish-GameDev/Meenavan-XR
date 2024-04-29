using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using System.Collections.Generic;

public class WormBucket : XRBaseInteractable
{
    public static WormBucket Instance { get; private set; }

    [SerializeField]
    private GameObject grabbableWorm;
    [SerializeField]
    private Transform transformToInstantiate;
    [SerializeField] private XRGrabInteractable aliveWorm = null;
    
    public XRGrabInteractable AliveWorm { get { return aliveWorm ; } }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        grabbableWorm.SetActive(false);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        EnableWorm(args);
    }

    public void EnableWorm(SelectEnterEventArgs args)
    {
        if (aliveWorm != null) return;
        grabbableWorm.SetActive(true);
        aliveWorm = grabbableWorm.GetComponent<XRGrabInteractable>();
        grabbableWorm.transform.position = transformToInstantiate.position;
        interactionManager.SelectEnter(args.interactorObject, aliveWorm);
    }

    public void DisableWorm()
    {
        if (aliveWorm == null) return;
        aliveWorm = null;
        grabbableWorm.SetActive(false);
        FishCaught.instance.EnableWormVisual();
    }
}