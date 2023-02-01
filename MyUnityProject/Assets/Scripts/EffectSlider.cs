using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectSlider : MonoBehaviour
{
    private Slider _slider;
    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.value = 0;
    }

    private void Update()
    {
        ///*** EFFECT SLIDER INPUTS *** ///
        if (Input.GetKey(KeyCode.Z))
        {
            AddToSlider(-5);
        }
        if (Input.GetKey(KeyCode.X))
        {
            AddToSlider(+5);
        }
    }

    public void AddToSlider(int v)
    {
        _slider.value += v;
    }

    public float GetEffectValue()
    {
        return _slider.value / _slider.maxValue;
    }
}
