using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

public class MovingBall : MonoBehaviour
{
    public IK_tentacles _myOctopus { get; private set; }
    public IK_Scorpion _myScorpion { get; private set; }
    public MovingTarget _target { get; private set; }

    public bool isShoot = false;
    private Vector3 _startPosition, _startShootPos, _startVelocity;

    private float _shootTime = 0f;
    private float _shootTimeDuration;

    //Magnus 
    public float _radius { get; private set; }
    readonly float _mass = 1f;
    readonly float _maxAngularVelocityMagnitude = 10f;

    private Vector3 _linearVelocity, _angularVelocity, _magnusForce, _acceleration;
    private Vector3 _rotationAxis;

    bool _isRotatingClockwise;
    [SerializeField] private Transform _tailTarget;

    // Arrows + Trajectory
    private int _numArrows = 20;
    private int _numPoints = 40;
    private bool _enabledArrows = true;

    public GameObject _greyArrowPrefab, _bluePointsPrefab;
    public GameObject _arrowContainer, _ballForceArrowsContainer;

    private Transform[] _greyArrows, _bluePoints;

    [SerializeField] private Transform _velocityArrow, _gravityArrow, _magnusArrow, _greyMagnusArrow;

    public EffectSlider _effect { get; private set; }
    public ForceSlider _sliderForce { get; private set; }

    public Text angularText;

    private Utils _utils = new Utils();

    void Awake()
    {
        _radius = gameObject.transform.GetChild(0).GetComponent<SphereCollider>().radius;
        _myOctopus = FindObjectOfType<IK_tentacles>();
        _myScorpion = FindObjectOfType<IK_Scorpion>();
        _target = GameObject.Find("BlueTarget").GetComponent<MovingTarget>();
        _effect = FindObjectOfType<EffectSlider>();
        _sliderForce = FindObjectOfType<ForceSlider>();
        _greyArrows = new Transform[_numArrows];

        for (int i = 0; i < _numArrows; i++)
        {
            _greyArrows[i] = Instantiate(_greyArrowPrefab, _arrowContainer.transform).transform;
        }

        _bluePoints = new Transform[_numPoints];

        for (int i = 0; i < _numPoints; i++)
        {
            _bluePoints[i] = Instantiate(_bluePointsPrefab, _arrowContainer.transform).transform;
        }
    }

    void Start()
    {
        _startPosition = transform.position;
        _shootTime = 0f;
        isShoot = false;
        _enabledArrows = true;
        ResetArrows();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            _enabledArrows = !_enabledArrows;
            _arrowContainer.SetActive(_enabledArrows);
            _ballForceArrowsContainer.SetActive(_enabledArrows);
        }

        if (isShoot)
        {
            transform.position = _utils.GetEulerPos(transform.position, _linearVelocity, Time.deltaTime);
            _linearVelocity = _utils.GetEulerVelocity(_linearVelocity, _acceleration, Time.deltaTime);

            _acceleration = Acceleration(_angularVelocity, _linearVelocity);
            _shootTime += Time.deltaTime;
            RotateBall();

            if (_shootTime <= _shootTimeDuration)
            {
                SetPointsTrajectory();
            }
            if (_enabledArrows)
            {
                BallArrows();
            }
        }
        else
        {
            transform.rotation = Quaternion.identity;
            _acceleration = Physics.gravity;

            StartVelocity();

            _angularVelocity = AngularVelocity();
            RotAxis();

            _linearVelocity = _startVelocity;
            SetPointsTrajectory();

            if (_enabledArrows)
            {
                SetGreyArrowTrajectory();
                SetPointsTrajectory();

                BallArrows();

                SetMagnusRotation(_greyMagnusArrow);
            }
        }

    }

    private Vector3 GetDirectionNormalized()
    {
        return (_target.GetPosition() - transform.position).normalized;
    }

    private void BallArrows()
    {
        _velocityArrow.rotation = Quaternion.LookRotation(_linearVelocity.normalized, Vector3.up);

        _gravityArrow.rotation = Quaternion.LookRotation(Physics.gravity.normalized, Vector3.up);

        SetMagnusRotation(_magnusArrow);
    }

    public void Respawn()
    {
        transform.position = _startPosition;
        isShoot = false;
        _shootTime = 0f;
        ResetArrows();

        angularText.text = "rotation: 0";
    }

    private void ResetArrows()
    {
        _arrowContainer.SetActive(_enabledArrows);
        _ballForceArrowsContainer.SetActive(_enabledArrows);
    }

    public void StartVelocity()
    {
        _shootTimeDuration = Mathf.Lerp(2.5f, 0.5f, _sliderForce.GetForceValue());
        _startShootPos = transform.position;

        //Xf = pos + Vo*t + 0.5f * gravity * timePow(2)
        _startVelocity = _target.GetPosition() - _startShootPos - (0.5f * Physics.gravity * Mathf.Pow(_shootTimeDuration, 2));
        _startVelocity /= _shootTimeDuration;
    }

    public Vector3 Acceleration(Vector3 angular, Vector3 linear)
    {
        _magnusForce = _utils.MagnusForce(angular, linear);
        return (Physics.gravity + _magnusForce) / _mass;
    }

    private Vector3 AngularVelocity()
    {
        Vector3 twistTmp = Vector3.Cross((_tailTarget.position - transform.position).normalized * _radius, _startVelocity);
        Vector3 angularVelocity = twistTmp * Mathf.Lerp(0f, _maxAngularVelocityMagnitude, _sliderForce.GetForceValue());
        return angularVelocity;
    }

    private void RotAxis()
    {
        Vector3 ballHitToCenterDir = _myScorpion.HitDirection.normalized;

        float dot = Vector3.Dot(GetDirectionNormalized(), ballHitToCenterDir);
        _isRotatingClockwise = Vector3.Dot(-ballHitToCenterDir, transform.right) >= 0;

        if (dot > 0.99f)
        {
            _rotationAxis = Vector3.zero;
        }
        else
        {
            _rotationAxis = Vector3.Cross(GetDirectionNormalized(), ballHitToCenterDir);
        }
    }

    private void RotateBall()
    {
        float angleRot = _angularVelocity.magnitude * Mathf.Rad2Deg;

        transform.Rotate(_angularVelocity.normalized, angleRot * Time.deltaTime);

        if (!_isRotatingClockwise) angleRot *= -1;

        angularText.text = "rotation: " + angleRot;
    }

    public void SetTailTargetPos(Vector3 localPos)
    {
        _tailTarget.localPosition = localPos;
    }

    private void SetGreyArrowTrajectory()
    {
        float timeStep = _shootTimeDuration / _numArrows;
        float accumulatedTime = 0;
        Vector3 futurePosition = _utils.GetPos(_startShootPos, _startVelocity, Physics.gravity, accumulatedTime);

        foreach (var arrow in _greyArrows)
        {
            arrow.position = futurePosition;
            futurePosition = _utils.GetPos(_startShootPos, _startVelocity, Physics.gravity, accumulatedTime + timeStep);
            arrow.rotation = Quaternion.LookRotation((futurePosition - arrow.position).normalized, Vector3.up);
            accumulatedTime += timeStep;
        }

        //for (int i = 0; i < _greyArrows.Length; i++)
        //{
        //    _greyArrows[i].position = futurePosition;
        //    futurePosition = _utils.GetPos(_startShootPos, _startVelocity, Physics.gravity, accumulatedTime + timeStep);
        //    _greyArrows[i].rotation = Quaternion.LookRotation((futurePosition - _greyArrows[i].position).normalized, Vector3.up);

        //    accumulatedTime += timeStep;
        //}
    }

    private void SetPointsTrajectory()
    {
        float timeStep = _shootTimeDuration / _numPoints;
        Vector3 position = _startPosition;
        Vector3 velocity = _startVelocity;
        Vector3 acceleration = Acceleration(_angularVelocity, velocity);


        for (int i = 0; i < _numPoints; ++i)
        {
            _bluePoints[i].position = position;

            position = _utils.GetEulerPos(position, velocity, timeStep);
            velocity = _utils.GetEulerVelocity(velocity, acceleration, timeStep);
            acceleration = Acceleration(_angularVelocity, velocity);
        }
    }

    private void SetMagnusRotation(Transform trans)
    {
        if (_magnusForce.sqrMagnitude > 0.01f)
        {
            trans.rotation = Quaternion.LookRotation(_magnusForce.normalized, Vector3.up);
        }
    }

}
