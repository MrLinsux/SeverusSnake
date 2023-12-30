using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSMeter : MonoBehaviour
{
    int currentFPS = 0;
    TMP_Text text;
    float secondTime = 0;

    void Start()
    {
        text = GetComponent<TMP_Text>();
        Application.targetFrameRate = 100;
    }

    // Update is called once per frame
    void Update()
    {
        currentFPS++;
        secondTime += Time.deltaTime;
        if (secondTime >= 1)
        {
            text.text = currentFPS.ToString();
            currentFPS = 0;
            secondTime = 0;
        }
    }
}
