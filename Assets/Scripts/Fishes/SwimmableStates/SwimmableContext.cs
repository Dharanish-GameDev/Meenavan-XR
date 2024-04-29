using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwimmableContext : MonoBehaviour
{
    #region States
    private FishBaseState currentState;
    public Schooling schooling = new();
    public MovingTowardsBait movingTowardsBait = new();
    public CaughtInhook caughtInhook = new();
    #endregion

    #region Private Variables
    [SerializeField] private FishMovementType fishMovementType;
    [SerializeField] private SwimmableType swimmableType;
    [SerializeField] private float fishSchoolingSpeed;
    [SerializeField] private float fishRotatingSpeed;
    [SerializeField] private float fishSchoolingRadius;
    [SerializeField] private float animationSpeedDivider;
    [SerializeField] private float baitSearchRadius;
    [SerializeField] private float baitFollowSpeed;

    private Vector3 initialPos;
    private readonly int SwimmingSpeed = Animator.StringToHash("SwimmingSpeed");
    private float initialSwimSpeedMultiplier;
    private Vector3 rotDir;

    // References
    [SerializeField] private Animator animator;
    [SerializeField] private Collider[] hookCollider;
    [SerializeField] private LayerMask hookLayer;
    [SerializeField] private GameObject caughtProgressUI;
    [SerializeField] private Image fillImage;



    [SerializeField] private string stateString;
    #endregion

    #region Properties
    public FishMovementType FishMoveType { get { return fishMovementType; }}
    public SwimmableType SwimmableType { get { return swimmableType; }}
    public float FishRotatingSpeed { get { return fishRotatingSpeed; }}
    public float FishSchoolingSpeed { get { return fishSchoolingSpeed; } }
    public float FishSchoolingRadius { get { return fishSchoolingRadius; } }
    public Vector3 InitialPos { get { return initialPos; }}
    public float BaitSearchRadius { get { return baitSearchRadius; }}
    public LayerMask HookLayer { get { return hookLayer; }}
    public Collider[] HookCollider { get { return hookCollider;} }
    public GameObject CaughtProgressUI { get { return caughtProgressUI; }}
    public float BaitFollowingSpeed { get { return baitFollowSpeed; } }
    public Image FillImage { get { return fillImage; }}

    #endregion

    private void OnEnable()
    {
        initialPos = transform.position;
        currentState = schooling;
        currentState.Enter(this);
    }
    private void Awake()
    {
       if(fishMovementType == FishMovementType.Dynamic) initialSwimSpeedMultiplier = animator.GetFloat(SwimmingSpeed);
    }
    private void Update()
    {
        currentState.Update(this);

#if UNITY_EDITOR
        stateString = currentState.ToString();
#endif
    }

    #region Public Methods

    public void SwitchState(FishBaseState state)
    {
        currentState.Exit(this);
        currentState = state;
        currentState.Enter(this);
    }
    public void RotateTowardsObject(float speed, Vector3 targetPos)
    {
        rotDir = Vector3.RotateTowards(transform.forward,(targetPos - transform.position), speed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(rotDir);
    }
    public void ResetAnimationSpeed()
    {
        if (animator != null)
            animator.SetFloat(SwimmingSpeed, initialSwimSpeedMultiplier);
    }
    public void DecreaseAnimationSpeed()
    {
        if (animator != null)
            animator.SetFloat(SwimmingSpeed, initialSwimSpeedMultiplier / animationSpeedDivider);
    }
    public void CheckForBait()
    {
        if (!FishingHook.Instance.IsHookInWater || FishCaught.instance.HasFish) return;
        if (Physics.OverlapSphereNonAlloc(transform.position,baitSearchRadius, hookCollider, hookLayer) == 1)
        {
            SwitchState(movingTowardsBait);
        }
        else
        {
            SwitchState(schooling);
        }
    }
    #endregion
}

public enum FishMovementType
{
    Static,
    Dynamic
}
public enum SwimmableType
{
    KoiFish,
    CrappiesFish,
    Tin,
    Boot
}