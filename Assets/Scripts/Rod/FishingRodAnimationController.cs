using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRodAnimationController : MonoBehaviour
{
    public Transform target; // Assign the target GameObject in the Inspector
    public float bendingSpeed; // Adjust the speed of the bending transition
    [SerializeField]
    private Animator animator;

    public bool canResetAnimationSpeed;

    private readonly int DirectionX = Animator.StringToHash("DirectionX");
    private readonly int DirectionY = Animator.StringToHash("DirectionY");

    private Vector3 targetDirection;
    float newHorizontal, newVertical,hor,ver;

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the fishing rod GameObject.");
        }
    }
    private void Update()
    {
        if (!canResetAnimationSpeed)
        {
            // Calculate the direction vector towards the target
            targetDirection = (target.position - transform.position).normalized;


            newHorizontal = Mathf.Lerp(animator.GetFloat(DirectionX), -Mathf.Cos(Mathf.Atan2(targetDirection.x, targetDirection.z)), Time.deltaTime * bendingSpeed);
            newVertical = Mathf.Lerp(animator.GetFloat(DirectionY), -Mathf.Sin(Mathf.Atan2(targetDirection.x, targetDirection.z)), Time.deltaTime * bendingSpeed);

            // Clamp values to the desired range
            newHorizontal = Mathf.Clamp(newHorizontal, -0.6f, 0.6f);
            newVertical = Mathf.Clamp(newVertical, -0.35f, 0f);

            // Update the blend tree parameters in the animator
            animator.SetFloat(DirectionX, newHorizontal);
            animator.SetFloat(DirectionY, newVertical);
        }
        else
        {
            hor = Mathf.Lerp(animator.GetFloat(DirectionX),0,Time.deltaTime * bendingSpeed);
            ver = Mathf.Lerp(animator.GetFloat(DirectionY), 0, Time.deltaTime * bendingSpeed);
            animator.SetFloat(DirectionX, hor);
            animator.SetFloat(DirectionY, ver);
        }
    }

    public void ResetAnimationParameters()
    {
        canResetAnimationSpeed = true;
    }
    public void EnableRodBend()
    {
        canResetAnimationSpeed = false;
    }
}