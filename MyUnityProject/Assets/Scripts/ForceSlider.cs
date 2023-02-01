using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForceSlider : MonoBehaviour
{

    [SerializeField] private Slider _slider;
    public bool sign { get; private set; }
    public bool canShoot = false;

    private void Awake()
    {
        sign = true;
        _slider = GetComponent<Slider>();
    }

    void Start()
    {
        _slider.value = 0;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (sign)
            {
                AddToSlider(+5);
            }
            else
            {
                AddToSlider(-5);
            }
        }
    }


    public void AddToSlider(float v)
    {
        _slider.value += v;

        if (_slider.value <= _slider.minValue)
        {
            sign = true;
        }

        if (_slider.value >= _slider.maxValue)
        {
            sign = false;
        }
    }

    public float GetForceValue()
    {
        return _slider.value / _slider.maxValue;
    }
}
