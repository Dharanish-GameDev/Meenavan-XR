using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaughtProgressUI : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Image fillImage;

    private void Start()
    {
        fillImage.fillAmount = 0;
        HideUI();
    }

    private void LateUpdate()
    {
        transform.forward = cameraTransform.forward;
    }

    private void OnEnable()
    {
        fillImage.fillAmount = 0.5f;
    }

    private void HideUI()
    {
        gameObject.SetActive(false);
    }
}
