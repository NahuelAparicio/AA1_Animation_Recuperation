using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OctopusController;
using UnityEditor.PackageManager;
using TMPro;

public class IK_Scorpion : MonoBehaviour
{
    MyScorpionController _myController= new MyScorpionController();

    public IK_tentacles _myOctopus;

    [Header("Body")]
    float animTime;
    public float animDuration = 5;
    public bool animPlaying = false;
    public Transform Body;
    public Transform StartPos;
    public Transform EndPos;

    [Header("Tail")]
    public Transform tailTarget;
    public Transform tail;

    [Header("Legs")]
    public Transform[] legs;
    public Transform[] legTargets;
    public Transform[] futureLegBases;

    public bool isShooting = false;
    public GameObject prefab;
    public MovingBall _ball;

    public GameObject _pointToRay;

    GameObject pointTarget;
    GameObject _lastGOHit;
    public Transform newBody;
    float dist, currentDist;

    void Start()
    {
        _myController.InitLegs(legs,futureLegBases,legTargets);
        _myController.InitTail(tail);
        pointTarget = Instantiate(prefab, tailTarget.position, Quaternion.identity);
    }

    void Update()
    {
        if(animPlaying)
            animTime += Time.deltaTime;

        if (animTime < animDuration)
        {
            Body.position = Vector3.Lerp(StartPos.position, EndPos.position, animTime / animDuration);
        }
        else if (animTime >= animDuration && animPlaying)
        {
            Body.position = EndPos.position;
            animPlaying = false;
        }

        if(isShooting)
        {
            _myController.UpdateIKTail();
        }

        //Exercice 3
        RaycastHit hit;
        if (Physics.Raycast(Body.position, -Vector3.up, out hit))
        {
            if (_lastGOHit == null)
            {
                _lastGOHit = hit.transform.gameObject;
                dist = GetValueRounded(newBody.position.y - futureLegBases[0].position.y);
            }
            if (_lastGOHit != hit.transform.gameObject)
            {
                //Ex 3.1
                foreach (Transform legBase in futureLegBases)
                {
                    legBase.transform.position = new Vector3(legBase.transform.position.x, hit.point.y, legBase.transform.position.z);
                }
                //Ex 3.4
                currentDist = GetValueRounded(newBody.position.y - futureLegBases[0].position.y);
                float temp = Mathf.Abs(dist) - Mathf.Abs(currentDist);
                newBody.position = new Vector3(newBody.position.x, newBody.position.y + temp, newBody.position.z);
                _lastGOHit = hit.transform.gameObject;
            }
        }

        _myController.UpdateIKLegs();
    }
    
    private float GetValueRounded(float val)
    {
        return (float)System.Math.Round(val, 3);
    }


    public void NotifyTailTarget()
    {
        _myController.NotifyTailTarget(pointTarget.transform);
    }

    //Trigger Function to start the walk animation
    public void NotifyStartWalk()
    {
        _myController.NotifyStartWalk();
    }

    public void ShootTail()
    {
        NotifyTailTarget();
        isShooting = true;
    }

    public void SetPointTarget()
    {
        pointTarget.transform.position += (Random.insideUnitSphere * 0.35f);
    }

    public void ResetPointTarget()
    {
        pointTarget.transform.position = tailTarget.position;
    }

    public void Respawn()
    {
        isShooting = false;
        _myController.NotifyTailTarget(null);
        NotifyStartWalk();
        animTime = 0;
        animPlaying = true;
    }


}
