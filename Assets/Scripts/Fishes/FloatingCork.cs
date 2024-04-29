using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCork : MonoBehaviour
{
    [SerializeField] private Transform fishingRodTip;
    [SerializeField] private Transform fishingHook;

    private float desiredDistance;
    private Vector3 targetPos;

    private void Update()
    {

        // Define a desired distance between the cork and the hook
        desiredDistance = 0.6f;

        // Calculate the target position based on the desired distance
        targetPos = fishingHook.position + (fishingRodTip.position - fishingHook.position).normalized * desiredDistance;

        // Assign the target position to the cork's position
        transform.position = targetPos;
    }
}