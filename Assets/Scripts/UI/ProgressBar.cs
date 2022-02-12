using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public float fillPercentage;
    private Transform fillTransform;

    // Start is called before the first frame update
    void Start()
    {
        fillTransform = transform.Find("Fill");
        if (fillPercentage > 100.0f) fillPercentage = 100.0f;
        if (fillPercentage < 0.0f) fillPercentage = 0.0f;
        fillTransform.localScale = new Vector3(fillPercentage/100,1,1);
    }

}
