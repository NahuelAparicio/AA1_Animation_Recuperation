using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    public LineRenderer _lRenderer;
    public int iterations = 10000;
    public int segCount = 300;
    public float segStepMod = 10f;

    private Vector3[] segments;
    private int numSegments = 0;

    public bool Actived
    {
        get
        {
            return _lRenderer.enabled;
        }
        set
        {
            _lRenderer.enabled = value;
        }
    }

    public void Start()
    {
        Actived = false;
    }

    public void SimulatePath(GameObject gameObject, Vector3 forceDirection, float mass, float drag, float _m)
    {
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();

        float timestep = Time.fixedDeltaTime;

        float stepDrag = 1 - drag * timestep;
        Vector3 velocity = forceDirection / mass * timestep;
        Vector3 velocity2 = Vector3.left * _m / mass * timestep;
        //Vector3 gravity = Physics.gravity * timestep * timestep;
        Vector3 position = gameObject.transform.position + rigidbody.centerOfMass;

        if (segments == null || segments.Length != segCount)
        {
            segments = new Vector3[segCount];
        }

        segments[0] = position;
        numSegments = 1;

        for (int i = 0; i < iterations && numSegments < segCount && position.y > 0f; i++)
        {
            //velocity += gravity;
            velocity += velocity2;
            velocity *= stepDrag;
            position += velocity;

            if (i % segStepMod == 0)
            {
                segments[numSegments] = position;
                numSegments++;
            }
        }

        DrawTrajectory();
    }

    private void DrawTrajectory()
    {
        Color startColor = Color.magenta;
        Color endColor = Color.magenta;
        startColor.a = 1f;
        endColor.a = 1f;

        _lRenderer.transform.position = segments[0];

        _lRenderer.startColor = startColor;
        _lRenderer.endColor = endColor;

        _lRenderer.positionCount = numSegments;

        for (int i = 0; i < numSegments; i++)
        {
            _lRenderer.SetPosition(i, segments[i]);
        }
    }
}

//DIDNT WORK

//private static Vector3 GetProjectilePositionAtTime(Vector3 start, Vector3 startVelocity, float time, float m)
//{
//    return start + startVelocity * time + (Vector3.left * m) * time * time * 0.5f;
//}


//public void SimulatePath(GameObject gameObject, Vector3 forceDirection, float mass, float drag, float _m, float maxTime, float timeStep)
//{
//    Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
//    float stepDrag = 1 - drag;
//    Vector3 _velocity = forceDirection / mass;

//    for (int i = 0; ; i++)
//    {
//        float time = timeStep * i;

//        if (time > maxTime) break;
//        stepDrag *= timeStep;
//        _velocity *= timeStep;
//        _velocity *= stepDrag;
//        _m *= timeStep;

//        Vector3 pos = GetProjectilePositionAtTime(gameObject.transform.position, _velocity, time, _m);
//        points.Add(pos);
//    }

//    DrawTrajectory();
//}