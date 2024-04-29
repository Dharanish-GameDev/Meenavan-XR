using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomainExpansion : MonoBehaviour
{
    public static DomainExpansion Instance { get; private set; }

    #region Private Variables

    //Exposed in Editor
    [Header("References")]
    [SerializeField] private Transform mainCameraTransform = null;
    [SerializeField] private TeleportAreaWithFade teleportAreaObject;
    [SerializeField] private TeleportationAnchorWithFade teleportationAnchorObject;
    [SerializeField] private GameObject oriLeftHand = null;
    [SerializeField] private GameObject domainExpansionLeftHand = null;

    [Header("Properties")]
    [SerializeField] private Vector3 expansionVec;
    [SerializeField] private float domainExpansionSpeed;

    // Hidden in Editor
    private bool domainExpansion = false;
    private Animator domainExpansionHandAnimator;
    private readonly int ExpandDomain = Animator.StringToHash("ExpandDomain");
    private float domainExpandWaitTime = 0;

    #endregion

    public bool IsDomainExpanded { get { return domainExpansion; } }
    public GameObject OriLeftHand { get { return oriLeftHand; } }
    public GameObject DomainExpansionLeftHand { get { return domainExpansionLeftHand; } }

    #region LifeCycle Methods


    private void Awake()
    {
        Instance = this;
        transform.localScale = Vector3.zero;
        domainExpansionHandAnimator =  domainExpansionLeftHand.GetComponent<Animator>();
        domainExpansionLeftHand.SetActive(false);
    }
    private void Update()
    {
        if (domainExpansion)
        {
            if(domainExpandWaitTime > 0)
            {
                domainExpandWaitTime -= Time.deltaTime;
                return;
            }
            if (transform.localScale.x < 10)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, expansionVec, domainExpansionSpeed * Time.deltaTime);
            }
        }
    }
    #endregion

    #region Public Methods

    public void ExpandDomainVisual ()
    {
        
        domainExpansion = !domainExpansion;
        transform.position = new Vector3(mainCameraTransform.position.x, 0.4f, mainCameraTransform.position.z);
        if (!domainExpansion)
        {
            transform.localScale = Vector3.zero;
            teleportAreaObject.enabled = true;
            teleportationAnchorObject.enabled = true;
            AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.domainClose, GameManager.Instance.XRrigAudioSource, 0.7f);
        }
        else
        {
            domainExpansionLeftHand.SetActive(true);
            oriLeftHand.SetActive(false);
            domainExpansionHandAnimator.SetTrigger(ExpandDomain);
            teleportAreaObject.enabled = false;
            teleportationAnchorObject.enabled = false;
            domainExpandWaitTime = 0.8f;
            AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.domainExpansion, GameManager.Instance.XRrigAudioSource, 1f);
            Invoke(nameof(PlayFingerSnap), 0.085f);
        }
    }

    #endregion
    private void PlayFingerSnap()
    {
        AudioManager.instance.PlayOneShot(AudioManager.instance.AudioClips.fingerSnap,GameManager.Instance.LHandAudioSource, 1);
    }

}
