using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public Vector3 GetEulerPos(Vector3 pos, Vector3 velocity, float timeStep)
    {
        return pos + velocity * timeStep;
    }
    public Vector3 GetEulerVelocity(Vector3 velocity, Vector3 accel, float timeStep)
    {
        return velocity + accel * timeStep;
    }
    //Over time
    public Vector3 GetPos(Vector3 shootPos, Vector3 startVelocity, Vector3 accel, float time)
    {
        return shootPos + (startVelocity * time) + (0.5f * accel * Mathf.Pow(time, 2));
    }
    public Vector3 MagnusForce(Vector3 angularVelocity, Vector3 linearVelocity)
    {
        return Vector3.Cross(angularVelocity, linearVelocity);
    }

}
