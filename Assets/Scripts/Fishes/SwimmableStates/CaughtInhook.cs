using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaughtInhook : FishBaseState
{
    private float caughtInHookWaitTime;
    private Vector3 targetPosition;
    public override void Enter(SwimmableContext context)
    {
        FishCaught.instance.SethasFishTrue();
        UI_Manager.instance.SetFillColor(context.FillImage, Color.red);
        if (context.FishMoveType == FishMovementType.Static)
        {
            caughtInHookWaitTime = 3f;
            return;
        }

        FishCaught.instance.RodAnimationController.EnableRodBend();
        RandomHookMovement.Instance.MoveRandomPointsToHook();
        RandomHookMovement.Instance.PlaceRandomPoints(context);
        RandomHookMovement.Instance.ExtendRope();
        RandomHookMovement.Instance.CalculatePossibilityForFishEscaping();
    }
    public override void Update(SwimmableContext context)
    {
        if (context.FishMoveType == FishMovementType.Static)
        {
            if(caughtInHookWaitTime > 0) caughtInHookWaitTime -= Time.deltaTime;
            else RandomHookMovement.Instance.InvokeOnFishCaught(context);
            UI_Manager.instance.SetUiFill(context.FillImage, 0, 3, caughtInHookWaitTime);
            return;
        }
        RandomHookMovement.Instance.MoveHookRandomly(context);
        MoveFishTowardsBait(context);
    }

    public override void Exit(SwimmableContext context)
    {
        base.Exit(context);
    }

    private void MoveFishTowardsBait(SwimmableContext context)
    {
        targetPosition = context.HookCollider[0].transform.position;
        context.transform.position = Vector3.MoveTowards(context.transform.position, targetPosition, context.BaitFollowingSpeed * 2f * Time.deltaTime);
        context.RotateTowardsObject(context.FishRotatingSpeed, targetPosition);
    }
}
