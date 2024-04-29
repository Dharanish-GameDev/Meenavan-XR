using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTowardsBait : FishBaseState
{
    private float UiEnableWaitTime;
    private float fishCaughtStateWaitTime;
    private Vector3 targetPosition;

    public override void Enter(SwimmableContext context)
    {
        UiEnableWaitTime = 0.25f;
        fishCaughtStateWaitTime = 4;
        UI_Manager.instance.SetFillColor(context.FillImage, Color.yellow);
    }
    public override void Update(SwimmableContext context)
    {
        EnableCaughtUI(context);
        CheckForBait(context);
        MoveFishTowardsBait(context);
    }
    public override void Exit(SwimmableContext context)
    {
        context.ResetAnimationSpeed();
    }
    private void CheckForBait(SwimmableContext context)
    {
        if (!FishingHook.Instance.IsHookInWater || FishCaught.instance.HasFish) context.SwitchState(context.schooling);
        if (Physics.OverlapSphereNonAlloc(context.transform.position, context.BaitSearchRadius, context.HookCollider, context.HookLayer) != 1)
        {
            context.SwitchState(context.schooling);
        }
    }
    private void MoveFishTowardsBait(SwimmableContext context)
    {
        if(context.FishMoveType == FishMovementType.Dynamic)
        {
            targetPosition = context.HookCollider[0].transform.position;
            context.transform.position = Vector3.MoveTowards(context.transform.position, targetPosition, context.BaitFollowingSpeed * Time.deltaTime);
            if (context.transform.position == targetPosition)
            {
                context.SwitchState(context.caughtInhook);
            }
            context.RotateTowardsObject(context.FishRotatingSpeed, targetPosition);
            UI_Manager.instance.SetUiFill(context.FillImage, context.transform.position, targetPosition, context.BaitSearchRadius);
        }
        else
        {
            if (fishCaughtStateWaitTime > 0)
            {
                fishCaughtStateWaitTime -= Time.deltaTime;
                UI_Manager.instance.SetUiFill(context.FillImage,0,4,fishCaughtStateWaitTime);
            }
            else context.SwitchState(context.caughtInhook);
        }
    }
    private void EnableCaughtUI(SwimmableContext context)
    {
        if (UiEnableWaitTime > 0) UiEnableWaitTime -= Time.deltaTime;
        else context.CaughtProgressUI.SetActive(true);
    }
}
