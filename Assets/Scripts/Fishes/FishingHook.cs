using System.Collections;
using System.Collections.Generic;
using Unity.XRContent.Interaction;
using UnityEngine;

public class FishingHook : MonoBehaviour
{
    public static FishingHook Instance { get; private set; }

    #region Private Variables

    // Exposed in Editor
    [Header("References")]
    [SerializeField] private LayerMask waterLayer;
    [SerializeField] private Collider[] waterCollider;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private FishingRodGrab fishingRodGrab;
    [SerializeField] private Transform leftHandTransform = null;

    [Header("Properties")]
    [SerializeField] private float maxRbVelocity;
    [SerializeField] private float hookInWaterDedectionSize;

    // Hidden In Editor
    private bool isHookInWater;
    private bool comeToHand = false;
    private float previousReelValue;
    private float neededReelValue = 0.2f;

    #endregion

    public bool IsHookInWater { get { return isHookInWater; } }
    public float NeededReelValue { get { return neededReelValue; } }

    public bool IsTouchingWater
    {
        get
        {
            return Physics.OverlapSphereNonAlloc(transform.position, hookInWaterDedectionSize, waterCollider, waterLayer) == 1 ? true : false;
        }
    }

    #region LifeCycle Methods
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!fishingRodGrab.IsGrabbedByHands) return;
        if (comeToHand)
        {
            transform.position = Vector3.MoveTowards(transform.position, leftHandTransform.position, 2 * Time.deltaTime);
            transform.localEulerAngles = Vector3.zero;
            if (Vector3.Distance(transform.position, leftHandTransform.position) < 0.05f)
            {
                rb.constraints = RigidbodyConstraints.FreezePosition;
                WormBucket.Instance.DisableWorm();
                if(FishCaught.instance.HasFish)
                {
                    UI_Manager.instance.ActiveFishCongratUi(true);
                }
            }
        }
        if(Input.GetKey(KeyCode.Space))
            {
            if (fishingRodGrab.ReelRotator.Value < 1)
            {
                fishingRodGrab.ReelRotator.Value += 2 * Time.deltaTime;
            }
        }

    }
    private void FixedUpdate()
    {
        HookInWaterDeduction();
    }

    private void HookInWaterDeduction()
    {
        if (Physics.OverlapSphereNonAlloc(transform.position, hookInWaterDedectionSize, waterCollider, waterLayer) == 1 && fishingRodGrab.IsGrabbedByHands && FishCaught.instance.HasWorm && DomainExpansion.Instance.IsDomainExpanded) isHookInWater = true;
        else isHookInWater = false;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hookInWaterDedectionSize);
    }

    public void OnPressingTrigger()
    {
        if(!CheckForHookObjects()) return;
        previousReelValue = fishingRodGrab.ReelRotator.Value;
        fishingRodGrab.ReelRotator.Value = neededReelValue;
        comeToHand = true;
        rb.useGravity = false;
    }
    public void OnReleasingTrigger()
    {
        comeToHand = false;
        if (!FishCaught.instance.HasFish)
                fishingRodGrab.ReelRotator.Value = previousReelValue;
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
    }
    public void ResetHookRotation()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private bool CheckForHookObjects()
    {
        if (FishCaught.instance.HasFish && fishingRodGrab.ReelRotator.Value > 0.9f) return true;
        if (WormBucket.Instance.AliveWorm != null && WormBucket.Instance.AliveWorm.isSelected) return true;
        return false;
    }
}
