using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatisticsGroup : MonoBehaviour
{
    public float fillPercentage;
    public string title;
    public string progress;
    
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI progressText;
    public GameObject progressBarObject;

    // Start is called before the first frame update
    void Start()
    {
        titleText.text = title;
        progressText.text = progress;
        if (fillPercentage == -1f)
        {
            progressBarObject.SetActive(false);
        } else if (progressBarObject != null)
        {
            ProgressBar progressBar = progressBarObject.GetComponent<ProgressBar>();
            progressBar.fillPercentage = fillPercentage;
        }
    }

}
