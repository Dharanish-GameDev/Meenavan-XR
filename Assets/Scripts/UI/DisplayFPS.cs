using System.Collections;
using TMPro;
using UnityEngine;

public class DisplayFPS : MonoBehaviour
{
    public float updateInteval = 0.2f;

    [SerializeField]
    private TextMeshProUGUI textOutput = null;

    private float deltaTime = 0.0f;
    private float milliseconds = 0.0f;
    private int framesPerSecond = 0;

    private void Start()
    {
        StartCoroutine(ShowFPS());
    }

    private void Update()
    {
        CalculateCurrentFPS();
    }

    private void CalculateCurrentFPS()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        milliseconds = (deltaTime * 1000.0f);
        framesPerSecond = (int)(1.0f / deltaTime);
    }

    private IEnumerator ShowFPS()
    {
        while (true)
        {
            textOutput.text = "FPS:" + framesPerSecond + "\n" + "MS:" + milliseconds.ToString(".0");
            yield return new WaitForSeconds(updateInteval);
        }
    }
}
