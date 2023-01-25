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

    void Update()
    {
        if(Input.GetKey(KeyCode.Z))
        {
            _slider.value -= 0.1f;
        }
        if(Input.GetKey(KeyCode.X))
        {
            _slider.value += 0.1f;
        }

    }

    public float GetEffectValue()
    {
        return _slider.value;
    }
}
