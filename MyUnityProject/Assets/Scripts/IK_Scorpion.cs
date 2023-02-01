using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OctopusController;

public class IK_Scorpion : MonoBehaviour
{
    MyScorpionController _myController = new MyScorpionController();

    public EffectSlider _effect { get; private set; }
    public ForceSlider _forceSlider { get; private set; }

    [Header("Body")]
    float animTime;
    public float animDuration = 5;
    bool animPlaying = false;
    public Transform Body;
    public Transform StartPos;
    public Transform EndPos;

    private Quaternion _desiredLookRotation;

    private Vector3 _lastBodyPosition = Vector3.zero;
    private Vector3 _currentForward = Vector3.zero;
    private Vector3 _moveOffset = Vector3.zero;

    [Header("Tail")]
    public Transform tailTarget;
    public Transform tail;
    private Transform[] _tailBones;
    private Quaternion[] _startTailRotations;
    private float tailTargetBallOffsetLength;

    private Vector3 _ballHit;
    public Vector3 HitDirection => _ballHit;

    [Header("Legs")]
    public Transform[] legs;
    public Transform[] legTargets;
    public Transform[] futureLegBases;
    public Transform _futureLegBasesHolder;

    [SerializeField] private MovingBall _ball;

    [SerializeField] private Transform mainBody;
    private Vector3 _bodyToLegsOffset;
    [SerializeField] private float _turnsWidth = 2f;
    [SerializeField] private int _numTurns = 2;

    readonly float _futureLegBaseOriginDisplacement = 2f;
    readonly float _futureLegBaseProbeDist = 5f;
    readonly Vector3 _futureLegBaseProbeDirection = Vector3.down;

    private bool canBeTailUpdate = false;

    private void Awake()
    {
        _effect = FindObjectOfType<EffectSlider>();
        _forceSlider = FindObjectOfType<ForceSlider>();
    }

    void Start()
    {
        _myController.InitLegs(legs, futureLegBases, legTargets);
        _myController.InitTail(tail);

        tailTargetBallOffsetLength = _ball._radius * 2;
        _ball.SetTailTargetPos(Vector3.forward);

        _bodyToLegsOffset = (mainBody.position.y - futureLegBases[0].position.y) * Vector3.up;
        _lastBodyPosition = mainBody.position;
        TailRotations();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StartShootBall();
            _forceSlider.canShoot = true;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Respawn();
        }

        if (!_ball.isShoot)
        {
            UpdateBallTrajectory();
        }

        if (animPlaying)
        {
            animTime += Time.deltaTime;
        }

        if (animTime < animDuration)
        {
            Move();
            TargetPos();
            UpdateScorpion();
            RotateBody();
        }

        else if (animTime >= animDuration && animPlaying)
        {
            Body.position = EndPos.position + _moveOffset;
            animPlaying = false;
        }

        _myController.UpdateIKLegs();
        if(canBeTailUpdate)
        {
            _myController.UpdateIKTail();
        }
    }

    private void Respawn()
    {
        canBeTailUpdate = false;
        _ball.Respawn();
        animTime = 0;
        animPlaying = false;
        ResetTailRotations();
        _moveOffset = Vector3.zero;
        _lastBodyPosition = mainBody.position;
    }

    //Function to send the tail target transform to the dll
    public void NotifyTailTarget()
    {
        canBeTailUpdate = true;
        _myController.NotifyTailTarget(tailTarget);
    }

    //Trigger Function to start the walk animation
    public void NotifyStartWalk()
    {
        _myController.NotifyStartWalk();
    }

    private void Move()
    {
        float t = animTime / animDuration;

        float sint = Mathf.Clamp01(t * 1.2f) * 2f * Mathf.PI * (_numTurns / 2f);
        _moveOffset.x = Mathf.Sin(sint) * _turnsWidth;

        _currentForward = mainBody.position - _lastBodyPosition;

        if (_currentForward.sqrMagnitude > 0.0001f)
        {
            _currentForward = _currentForward.normalized;
        }

        _lastBodyPosition = mainBody.position;

        Body.position = Vector3.Lerp(StartPos.position, EndPos.position, t) + _moveOffset;
    }

    public void StartShootBall()
    {
        NotifyTailTarget();
        animPlaying = true;
        NotifyStartWalk();
    }

    private void TargetPos()
    {
        float targetX = _ball._target.GetPosition().x;
        float ballX = _ball.transform.position.x;

        Vector3 right = _ball.transform.right * IsTargetRight(ballX, targetX);

        Vector3 offsetDir = Vector3.Lerp(_ball.transform.forward, right, _ball._effect.GetEffectValue()).normalized;

        _ballHit = -offsetDir;
        _ball.SetTailTargetPos(offsetDir * tailTargetBallOffsetLength);
    }

    private int IsTargetRight(float ball,  float target)
    {
        if(ball < target)
        {
            return -1;
        }
        return 1;
    }

    private void UpdateBallTrajectory()
    {
        _ball.StartVelocity();
    }

    //Raycast + calculating rotation deseada
    private void UpdateScorpion()
    {
        Vector3 bodyLegs = Vector3.zero;
        Vector3 leftLegs = Vector3.zero;
        Vector3 rightLegs = Vector3.zero;

        for (int i = 0; i < futureLegBases.Length; ++i)
        {
            Vector3 hitOrigin = futureLegBases[i].position + (-_futureLegBaseProbeDirection * _futureLegBaseOriginDisplacement);
            RaycastHit hit;
            if (Physics.Raycast(hitOrigin, _futureLegBaseProbeDirection, out hit, _futureLegBaseProbeDist))
            {
                futureLegBases[i].transform.position = new Vector3(futureLegBases[i].transform.position.x, hit.point.y, futureLegBases[i].transform.position.z);
            }
            bodyLegs += futureLegBases[i].position;

            if (i % 2 == 0)
                rightLegs += futureLegBases[i].position;
            else
                leftLegs += futureLegBases[i].position;
        }


        float numLegs = (float)futureLegBases.Length;
        bodyLegs /= numLegs;
        mainBody.position = bodyLegs + _bodyToLegsOffset;

        float numLegsEachSide = numLegs / 2f;
        rightLegs /= numLegsEachSide;
        leftLegs /= numLegsEachSide;

        if (_currentForward.sqrMagnitude > 0.0001f)
        {
            Vector3 rightAxis = (rightLegs - leftLegs).normalized;
            Vector3 upAxis = Vector3.Cross(_currentForward, rightAxis).normalized;
            _desiredLookRotation = Quaternion.LookRotation(-_currentForward, upAxis);
        }
    }

    private void RotateBody()
    {
        if (_currentForward.sqrMagnitude > 0.0001f)
        {
            mainBody.rotation = Quaternion.RotateTowards(mainBody.rotation, _desiredLookRotation, 200f * Time.deltaTime);

            _futureLegBasesHolder.rotation = Quaternion.AngleAxis(mainBody.rotation.eulerAngles.y, Vector3.up);
        }
    }

    private void TailRotations()
    {
        List<Quaternion> rots = new List<Quaternion>();
        List<Transform> bones = new List<Transform>();
        Transform tailBone = tail;
        while (tailBone.childCount > 0)
        {
            rots.Add(tailBone.rotation);
            bones.Add(tailBone);
            tailBone = tailBone.GetChild(1);
        }
        rots.Add(tailBone.rotation);
        bones.Add(tailBone);

        _startTailRotations = rots.ToArray();
        _tailBones = bones.ToArray();
    }

    private void ResetTailRotations()
    {
        for (int i = 0; i < _tailBones.Length; ++i)
        {
            _tailBones[i].rotation = _startTailRotations[i];
        }
    }


}
