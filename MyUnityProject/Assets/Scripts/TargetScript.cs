using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    private MovingBall _ball;

    private void Awake()
    {
        _ball = FindObjectOfType<MovingBall>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (_ball.isShoot) return;
        _ball._myOctopus.NotifyShoot();
        _ball._sliderForce.canShoot = false;
        _ball.isShoot = true;
    }
}
