using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForceSlider : MonoBehaviour
{

    [SerializeField] private Slider _slider;
    private bool sign = true;
    public bool canShoot = false;
    public float strenghtForce = 0;

    IK_Scorpion _scorp;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _scorp = FindObjectOfType<IK_Scorpion>();
    }

    void Start()
    {
        _slider.value = 0;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (sign)
            {
                AddToSlider(+0.0005f);
            }
            else
            {
                AddToSlider(-0.0005f);
            }
            if (_slider.value == 0.15f)
            {
                sign = true;
            }
            if (_slider.value == 0.25f)
            {
                sign = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _scorp.SetPointTarget();
            canShoot = true;
            strenghtForce = _slider.value;
        }

    }

    void AddToSlider(float val)
    {
        _slider.value += val;
    }
}
