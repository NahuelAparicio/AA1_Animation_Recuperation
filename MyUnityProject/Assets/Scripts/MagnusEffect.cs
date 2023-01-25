using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagnusEffect : MonoBehaviour
{
    [SerializeField]
    private float areaDensity;

    public Slider _slide;

    private List<MagnusPhysics> magnusPs;

    void Start()
    {
        magnusPs = new List<MagnusPhysics>();
    }
    private void FixedUpdate()
    {
        if(magnusPs.Count > 0)
        {
            foreach (MagnusPhysics mp in magnusPs)
            {
                float planeVel = new Vector3(mp.RigidBody.velocity.x, 0, mp.RigidBody.velocity.z).magnitude;
                float forceM = (mp.Drag * areaDensity * mp.CrossSection * Mathf.Pow(planeVel, 2f)) / 2;
                mp.RigidBody.AddForce(Vector3.left * forceM);
            }
        }
    }
    private void Update()
    {
        areaDensity = _slide.value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<MagnusPhysics>() != null)
        {
            magnusPs.Add(other.GetComponent<MagnusPhysics>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<MagnusPhysics>() != null)
        {
            magnusPs.Remove(other.GetComponent<MagnusPhysics>());
        }
    }
}
