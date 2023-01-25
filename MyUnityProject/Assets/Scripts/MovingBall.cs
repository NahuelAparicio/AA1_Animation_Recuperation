using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.GlobalIllumination;

public class MovingBall : MonoBehaviour
{
    [SerializeField]
    MovingTarget _target;
    IK_tentacles _myOctopus;
    IK_Scorpion _myScorpion;


    [Range(-1.0f, 1.0f)]
    [SerializeField]
    private float _movementSpeed = 5f;
    private Rigidbody _rb;
    private MagnusPhysics _magPhysics;

    public float radius;
    public float forceToBeAplied;
    public ForceSlider _strengthSlider;
    public Slider _effectSlider;
    public Text _rotationText;

    public Trajectory _trajectory;
    private void Awake()
    {
        _magPhysics = GetComponent<MagnusPhysics>();
        radius = GetComponent<SphereCollider>().radius;
        _rb = GetComponent<Rigidbody>();
        _myOctopus = FindObjectOfType<IK_tentacles>();
        _target = GameObject.Find("BlueTarget").GetComponent<MovingTarget>();
        _myScorpion = FindObjectOfType<IK_Scorpion>();
    }

    void Update()
    {
        forceToBeAplied = _strengthSlider.strenghtForce;

        transform.rotation = Quaternion.identity;

        if (Input.GetKeyDown(KeyCode.L))
        {
            Respawn();
        }

        if(_strengthSlider.canShoot && Input.GetKeyUp(KeyCode.Space))
        {
            _myScorpion.ShootTail();
        }


        if (_effectSlider.value != 0)
        {
            if(Input.GetKeyDown(KeyCode.I))
            {
                float pV = new Vector3(GetDirectionNormalized().x * forceToBeAplied, 0, GetDirectionNormalized().z * forceToBeAplied).magnitude;
                float fM = (_magPhysics.Drag * _effectSlider.value * _magPhysics.CrossSection * Mathf.Pow(pV, 2f)) / 2;
                _trajectory.Actived = true; //forceM * Vector3.left
                _trajectory.SimulatePath(gameObject, GetDirectionNormalized() * forceToBeAplied, _rb.mass, _magPhysics.Drag, fM);

            }

            //MAGNUS FORCE APLIED
            _rotationText.text = GetRotation() + "degree / sec";
            float planeVel = new Vector3(_magPhysics.RigidBody.velocity.x, 0, _magPhysics.RigidBody.velocity.z).magnitude;
            float forceM = (_magPhysics.Drag * _effectSlider.value * _magPhysics.CrossSection * Mathf.Pow(planeVel, 2f)) / 2;
            _magPhysics.RigidBody.AddForce(Vector3.left * forceM);
        }
        else
        { // No magnus effect show
            _trajectory.Actived = false;
            Debug.DrawLine(transform.position, _target.GetPosition(), Color.gray);
        }
    }
    float GetRotation()
    {
        return (_rb.velocity.magnitude / radius) / (360 / 2 * Mathf.PI);
    }

    void ShootAction()
    {
        _rb.angularVelocity = Vector3.Cross(transform.position, GetDir()) * GetDir().magnitude * Mathf.Rad2Deg;
        _rb.AddForce(GetDirectionNormalized() * forceToBeAplied, ForceMode.Impulse);
        _myOctopus.NotifyShoot();
        _strengthSlider.canShoot = false;
    }

    //DIRECTION TO SHOOT
    Vector3 GetDirectionNormalized()
    {
        return (_target.GetPosition() - transform.position).normalized;
    }

    Vector3 GetDir()
    {
        return _target.GetPosition() - transform.position;

    }

    //private void CalculateMagnus()
    //{
    //    var direction = Vector3.Cross(_rb.angularVelocity, _rb.velocity);
    //    var magnitude = 4 / 3f * Mathf.PI * airDensity * Mathf.Pow(radius, 3);
    //    _rb.AddForce(magnitude * direction, ForceMode.Impulse);
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (_strengthSlider.canShoot)
        {
            ShootAction();
        }
    }

    public void Respawn()
    {
        _rb.angularVelocity = Vector3.zero;
        _rb.velocity = Vector3.zero;
        transform.position = new Vector3(-125, 22, -39);
        _myScorpion.Respawn();
        _myScorpion.ResetPointTarget();
    }
}
