using System;
using UnityEngine;

public class RandomHookMovement : MonoBehaviour
{
    public static RandomHookMovement Instance { get; private set; }

    public event Action<SwimmableContext> OnFishCaught;
    public event Action<SwimmableContext> OnFishEscaped;

    [SerializeField] private Transform[] hookRandomPoints;
    [SerializeField] private Transform hookTransform;
    [SerializeField] private FishingRodGrab fishingRod;
    [SerializeField] private Transform randomHookPointTransform;
    [Range(0,10)]
    [SerializeField] private float hookMoveSpeed;
    [SerializeField]
    private int hookIndex = 0;
    private int fishEscapingRandomint = 0;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    public void CalculatePossibilityForFishEscaping()
    {
        fishEscapingRandomint = UnityEngine.Random.Range(1, 6);
    }
    public void MoveHookRandomly(SwimmableContext context)
    {
        if(hookIndex < 3)
        {
            hookTransform.position = Vector3.MoveTowards(hookTransform.position, hookRandomPoints[hookIndex].position, hookMoveSpeed * Time.deltaTime);
            UI_Manager.instance.SetUiFill(context.FillImage, hookTransform.position, hookRandomPoints[hookIndex].position, 1.5f);
            if(hookIndex == 2 && fishEscapingRandomint == 1 && context.FishMoveType == FishMovementType.Dynamic) // Checking For Fish Escaping
            {
                OnFishEscaped?.Invoke(context);
            }
            if (Vector3.Distance(transform.position, hookRandomPoints[hookIndex].position) < 0.05f)
            {
                hookIndex++;
            }
        }
        else
        {
            OnFishCaught?.Invoke(context);
        }
    }
    public void PlaceRandomPoints(SwimmableContext context)
    {
        hookRandomPoints[0].localPosition = new Vector3(UnityEngine.Random.Range(-1.2f,1.2f),0, UnityEngine.Random.Range(-1.2f, 1.2f));
        hookRandomPoints[1].localPosition = new Vector3(UnityEngine.Random.Range(-1.2f,1.2f),0, UnityEngine.Random.Range(-1.2f, 1.2f));
        hookRandomPoints[2].localPosition = new Vector3(UnityEngine.Random.Range(-1.2f,1.2f),0, UnityEngine.Random.Range(-1.2f, 1.2f));
    }
    public void InvokeOnFishCaught(SwimmableContext context)
    {
        OnFishCaught?.Invoke(context);
    }
    public void ExtendRope()
    {
        if (fishingRod.ReelRotator.Value < FishingHook.Instance.NeededReelValue) return;
        fishingRod.ReelRotator.Value = FishingHook.Instance.NeededReelValue;
    }
    public void MoveRandomPointsToHook()
    {
        randomHookPointTransform.position = new Vector3(hookTransform.position.x, -0.626f, hookTransform.position.z);
    }
}
