using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSlider : MonoBehaviour
{
    private Slider slider;
    private GameObject fillArea;

    // Start is called before the first frame update
    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate { OnValueChanged(); });
        fillArea = slider.fillRect.gameObject;
        fillArea.SetActive(false);
        OnValueChanged();
    }

    public void OnValueChanged()
    {
        fillArea.SetActive(slider.value == 1);
    }

    public void OnHandleClick()
    {
        if (slider.value == 0)
        {
            slider.value = 1;
        } else
        {
            slider.value = 0;
        }
    }
}
