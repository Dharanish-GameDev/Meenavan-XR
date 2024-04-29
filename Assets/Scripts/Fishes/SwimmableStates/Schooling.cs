using UnityEngine;

public class Schooling : FishBaseState
{
    private float randomRestTime;
    private float reachedRestTime;
    private bool canRestaBit;
    private Vector3 actualRandomDestinationPos;

    public override void Enter(SwimmableContext context)
    {
        randomRestTime = 10;
        reachedRestTime = 2;
        canRestaBit = false;
        CalculateRandomPoint(context);
        context.CaughtProgressUI.SetActive(false);
    }
    public override void Update(SwimmableContext context)
    {
        CheckForBait(context);
        SchoolingFishes(context);
    }
    public override void Exit(SwimmableContext context)
    {
        base.Exit(context);
    }

    #region Private Methods
    public void SchoolingFishes(SwimmableContext context)
    {
        if (context.FishMoveType == FishMovementType.Static) return;
        CheckForRest(context);
        MoveFish(context);
    }
    private void CheckForBait(SwimmableContext context)
    {
        if (!FishingHook.Instance.IsHookInWater || FishCaught.instance.HasFish) return;
        if (Physics.OverlapSphereNonAlloc(context.transform.position, context.BaitSearchRadius, context.HookCollider, context.HookLayer) == 1)
        {
            context.SwitchState(context.movingTowardsBait);
        }
    }
    private void MoveFish(SwimmableContext context)
    {
        if (canRestaBit) return;
        context.transform.position = Vector3.MoveTowards(context.transform.position, actualRandomDestinationPos, context.FishSchoolingSpeed * Time.deltaTime);
        if (context.transform.position == actualRandomDestinationPos)
        {
            reachedRestTime -= Time.deltaTime;
            context.DecreaseAnimationSpeed();
            if (reachedRestTime <= 0)
            {
                CalculateRandomPoint(context);
                reachedRestTime = UnityEngine.Random.Range(2.0f, 3.0f);
            }
        }
        context.RotateTowardsObject(context.FishRotatingSpeed, actualRandomDestinationPos);
    }
    private void CalculateRandomPoint(SwimmableContext context)
    {
        actualRandomDestinationPos = (UnityEngine.Random.insideUnitSphere * context.FishSchoolingRadius) + context.InitialPos; // Calculates the Temp Random Point With all axes
        actualRandomDestinationPos.y = context.InitialPos.y;
        context.ResetAnimationSpeed(); // Resetting Animation Playing To its initial Value.
    }

    private void CheckForRest(SwimmableContext context)
    {
        randomRestTime -= Time.deltaTime;
        if(randomRestTime <= 0)
        {
            if(canRestaBit)
            {
                canRestaBit = false;
                context.ResetAnimationSpeed();
                randomRestTime = UnityEngine.Random.Range(15.0f, 20.0f);
            }
            else
            {
                canRestaBit = true;
                context.DecreaseAnimationSpeed();
                randomRestTime = UnityEngine.Random.Range(5.0f, 7.0f);
            }
        }
    }
    #endregion

}
