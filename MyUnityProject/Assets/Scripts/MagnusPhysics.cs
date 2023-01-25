using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnusPhysics : MonoBehaviour
{
    [SerializeField]
    private float _dragCoeff;
    [SerializeField]
    private float _crossSectionArea;

    private Rigidbody _rb;

    public float Drag
    {
        get
        {
            return _dragCoeff;
        }
    }

    public float CrossSection
    {
        get
        {
            return _crossSectionArea;
        }
    }

    public Rigidbody RigidBody
    {
        get
        {
            return _rb;
        }
    }


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _crossSectionArea = Mathf.Pow((Mathf.PI * GetComponent<SphereCollider>().radius), 2);
    }
}
