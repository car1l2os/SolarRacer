using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleManager : MonoBehaviour
{

    private enum CURRENTLANE
    {
        LEFT = -1,
        MIDDLE = 0,
        RIGHT = 1
    }

    RoadManager _roadManager;

    //Lane swapping
    [SerializeField]
    private Transform _leftLane;

    //fly
    private float _powerValuePreFly = 0.0f;

    internal void FlyVelocity()
    {
        _powerValuePreFly = _linkedRoad.PowerSpeedModifier;
        _linkedRoad.PowerSpeedModifier = 1f;
        _linkedRoad._powerVelocityLocked = true;
        _flying = true;
    }

    internal void GroundVelocity()
    {
        _linkedRoad._powerVelocityLocked = false;
        _linkedRoad.PowerSpeedModifier = _powerValuePreFly;
        _flying = false;
    }

    [SerializeField]
    private Transform _middleLane;
    [SerializeField]
    private Transform _rightLane;
    private int _obstaclesCollisionLayerMask;
    private bool _isInvulnerable = false;
    private Vector3 _targetPosition;



    //Score related
    [Header("Score related")]
    [SerializeField]
    private ScoreManager _scoreManager;
    [SerializeField]
    [Range(1, 2)]
    private int _player;
    [SerializeField]
    private float _pointsPenaltyForCollision = 100.0f;
    private int _collectablesCount = 0;
    private int _collectablesToPoint = -1;
    [SerializeField]
    private float _losePointsOnShadowCoolDown = 0.25f;
    [SerializeField]
    private Text _collectablesCounter;
    public bool _onShadow = false;
    [SerializeField]
    private Power _powerBar;
    private float _powerChangeVelocity = 0.1f;
    private bool _flying = false;

    [Header("Speed penalty related")]
    [SerializeField]
    private Road _linkedRoad;
    [SerializeField]
    [Range(0f, 1f)]
    private float _speedPenaltyForCollision = 0.5f;
    private bool _canChangeRoad = false;


    [Header("Movement related")]
    [SerializeField]
    Button[] _movementButtons = new Button[2];
    private Camera _camera;
    private CURRENTLANE _currentLane = CURRENTLANE.MIDDLE;
    [SerializeField]
    private Animator _animator;
    private bool _controlledByAI = false;
    private Vector3 _clickPosition = Vector3.zero;
    [SerializeField]
    [Range(0f, 1f)]
    private float _minimumXDisplacementForDrag = 0.2f; //percentage of screen with/2
    //private InputTimer _inputTimer;

    //Particles
    ParticleSystem _smoke;
    ParticleSystem[] _crashParticles;

    //Audio
    AudioManager _audioManager;

    //timer 
    InputTimer _inputTimer;

    [Header("Debug")]
    [SerializeField]
    private GameObject _touchMarker;


    //Pollution visual
    [SerializeField]
    private Image _pollution;

    private void Awake()
    {
        _flying = false;
        _audioManager = FindObjectOfType<AudioManager>();
        _inputTimer = FindObjectOfType<InputTimer>();
        _inputTimer.ResetTimer();
        _audioManager.Play("Driving_" + _player);
        _smoke = transform.Find("Smoke").GetComponent<ParticleSystem>();
        _crashParticles = transform.Find("CrashParticle").GetComponentsInChildren<ParticleSystem>();
        _smoke.Stop();
        for (int i = 0; i < _crashParticles.Length; i++)
            _crashParticles[i].Stop();
    }

    private void Start()
    {
        _obstaclesCollisionLayerMask = LayerMask.GetMask("Obstacle");
        _camera = GetComponentInChildren<Camera>();
        _animator = GetComponentInChildren<Animator>();
        _targetPosition = _middleLane.position;

        _linkedRoad.SpeedModifier = 0.0f;
        _collectablesToPoint = _linkedRoad.CollectablesToPoints;
       // _inputTimer = GameObject.Find("InputTimer").GetComponent<InputTimer>(); //no really good but no other option

       // _inputTimer.ResetTimer();
    }

    public void StartRace()
    {
        _linkedRoad.SpeedModifier = 1.0f;
        _canChangeRoad = true;
        _inputTimer.ResetTimer();
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < Input.touchCount; ++i)
        {
            Touch t = Input.GetTouch(i);
            if (t.phase == TouchPhase.Began && IsThisPointInMyHalfScreen(new Vector3(t.position.x,t.position.y,0)))
            {
                _clickPosition = new Vector3(t.position.x, t.position.y,0);
            }

            if (t.phase == TouchPhase.Ended && IsThisPointInMyHalfScreen(new Vector3(t.position.x, t.position.y, 0)))
            {
                OnClickUp(new Vector3(t.position.x,t.position.y,0));
            }
        }
        /*if (Input.GetMouseButtonDown(0) && IsThisPointInMyHalfScreen(Input.mousePosition))
        {
            //ClickOnScreen(Input.mousePosition);
            _clickPosition = Input.mousePosition;
            //_inputTimer.ResetTimer();
        }

        if (Input.GetMouseButtonUp(0) && IsThisPointInMyHalfScreen(Input.mousePosition))
        {
            //ClickOnScreen(Input.mousePosition);
            //_inputTimer.ResetTimer();
        }*/

    }

    private void Update()
    {        
        if (_targetPosition != transform.position)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * 10f);
        }

        if (_onShadow)
        {
            AddShadowTimeToStaticContainter(Time.deltaTime);
            _powerBar.LosePower(_powerChangeVelocity*7.5f);
            _linkedRoad.PowerSpeedModifier = _powerBar.NormalizedPowerModifier;

            //Pollution visual
            Color pollutionColor = _pollution.color;
            if (pollutionColor.a < 0.75f)
            {
                pollutionColor.a += 1.0f * Time.deltaTime;
            }
            _pollution.color = pollutionColor;
        }
        else
        {
            _powerBar.AddPower(_powerChangeVelocity);
            _linkedRoad.PowerSpeedModifier = _powerBar.NormalizedPowerModifier;

            //Pollution visual
            Color pollutionColor = _pollution.color;
            if (pollutionColor.a > 0.0f)
            {
                pollutionColor.a -= 1.0f * Time.deltaTime;
            }
            _pollution.color = pollutionColor;
        }
    }



    public void OnClickLeft()
    {
        _inputTimer.ResetTimer();
        // _inputTimer.ResetTimer(); //Don't know if clicking on a button is going to trigger the Unity's input so, better be sure 
        if (_controlledByAI == false)
        {
            if (_canChangeRoad && _isInvulnerable == false && Physics.Raycast(transform.position, Vector3.forward, Mathf.Abs(_rightLane.position.z - _leftLane.position.z) * 1.5f, 1<<_obstaclesCollisionLayerMask) == false)
            {
                switch (_currentLane)
                {
                    case CURRENTLANE.LEFT:
                        break;
                    case CURRENTLANE.MIDDLE:
                        //transform.position = _leftLane.position;
                        _targetPosition = _leftLane.position;
                        _currentLane = CURRENTLANE.LEFT;
                        _animator.SetTrigger("Left");
                        break;
                    case CURRENTLANE.RIGHT:
                        //transform.position = _middleLane.position;
                        _targetPosition = _middleLane.position;
                        _currentLane = CURRENTLANE.MIDDLE;
                        _animator.SetTrigger("Left");
                        break;
                }
            }
        }
    }

    public void OnClickRight() //
    {
        _inputTimer.ResetTimer();
        // _inputTimer.ResetTimer(); //Don't know if clicking on a button is going to trigger the Unity's input so, better be sure 
        if (_controlledByAI == false)
        {
            if (_canChangeRoad && _isInvulnerable == false && Physics.Raycast(transform.position, Vector3.back, Mathf.Abs(_rightLane.position.z - _leftLane.position.z) * 1.5f, 1<<_obstaclesCollisionLayerMask) == false)
            {
                switch (_currentLane)
                {
                    case CURRENTLANE.LEFT:
                        //transform.position = _middleLane.position;
                        _targetPosition = _middleLane.position;
                        _currentLane = CURRENTLANE.MIDDLE;
                        _animator.SetTrigger("Right");
                        break;
                    case CURRENTLANE.MIDDLE:
                        //transform.position = _rightLane.position;
                        _targetPosition = _rightLane.position;
                        _currentLane = CURRENTLANE.RIGHT;
                        _animator.SetTrigger("Right");
                        break;
                    case CURRENTLANE.RIGHT:
                        break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {

        if (this.GetComponent<BoxCollider>().bounds.center.x > col.bounds.center.x - col.bounds.size.x * 0.25f) //lets make it easier for the kids 
        {
            if (col.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "GroundObstacle" && _canChangeRoad)
            {
                //launch collision animation
                //make it inmortal X seconds
                //speed penalty?
                _linkedRoad.SetVelocityPenaltyForTime(0f, 0.5f);
                _scoreManager.LaunchEventInScore(_pointsPenaltyForCollision, _player, true);
                _canChangeRoad = false;

                //animator
                _animator.SetTrigger("Crash");
                for (int i = 0; i < _crashParticles.Length; i++)
                    _crashParticles[i].Play();


                Renderer[] _toMakeInvisible = col.gameObject.transform.parent.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < _toMakeInvisible.Length; i++)
                {
                    _toMakeInvisible[i].enabled = false;
                }
                _audioManager.Play("Crash");
                _powerBar.LosePower(_powerBar.CurrentPower/2f);
            }

            if (col.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "SkyObstacle")
            {
                _linkedRoad.SpeedModifier = 0.5f;
                _smoke.Play();
                _onShadow = true;
            }
            //_scoreManager.LaunchEventInScore(_pointsPenaltyForCollision, _player);
            //_linkedRoad.SetVelocityPenaltyForTime(_speedPenaltyForCollision, 1.0f);

            if (col.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Collectable" && _flying == false)
            {
                //EditorApplication.isPaused = true;
                Destroy(col.transform.parent.transform.parent.gameObject);
                _collectablesCount += 1;
                CheckIfCollectablesPoints();
                UpdateCollectableVisualCounter();
                AddCollectableToStaticContainer();
                _audioManager.Play("Collectable");
            }

            if (col.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "PickUp")
            {
                Destroy(col.gameObject);
                _scoreManager.LaunchEventInScore(_linkedRoad.PointsPerPickUp, _player, true);
                AddPickUpToStaticContainer();
                _audioManager.Play("PickUp");
            }

            if (col.gameObject.tag == "Ramp")
            {
                _animator.SetTrigger("Jump");
                _canChangeRoad = false;
            }

            if (col.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Train" && _canChangeRoad == true)
            {
                _linkedRoad.SetVelocityPenaltyForTime(0f, 2f);
            }
        }
    }

    private void AddCollectableToStaticContainer()
    {
        if (StaticDataContainer._firstInFinish == 0)
        {
            if (_player == 1)
                StaticDataContainer._player1Collectables++;
            else
                StaticDataContainer._player2Collectables++;
        }
    }

    private void AddPickUpToStaticContainer()
    {
        if (StaticDataContainer._firstInFinish == 0)
        {
            if (_player == 1)
                StaticDataContainer._player1PickUps++;
            else
                StaticDataContainer._player2PickUps++;
        }
    }

    private void AddShadowTimeToStaticContainter(float time)
    {
        if (StaticDataContainer._firstInFinish == 0)
        {
            if (_player == 1)
                StaticDataContainer._player1SecondsInShadow += time;
            else
                StaticDataContainer._player2SecondsInShadow += time;
        }
    }

    private void CheckIfCollectablesPoints()
    {
        if (_collectablesCount == _collectablesToPoint)
        {
            _scoreManager.LaunchEventInScore(_linkedRoad.PoinstPerCollectableGroup, _player, false);
            _collectablesCount = 0;
        }

    }

    private void UpdateCollectableVisualCounter()
    {
        _collectablesCounter.text = _collectablesCount + " / " + _collectablesToPoint;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "GroundObstacle")
            _linkedRoad.SpeedModifier = 0.5f;

        if (other.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "SkyObstacle" && _canChangeRoad)
        {
            _losePointsOnShadowCoolDown -= Time.deltaTime;
            if (_losePointsOnShadowCoolDown <= 0.0f)
            {
                _scoreManager.LaunchEventInScore(-100, _player, true);
                _losePointsOnShadowCoolDown += 0.25f;
            }
        }

        if (other.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Train" && _canChangeRoad == true)
        {
            _linkedRoad.SpeedModifier = 0.1f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "GroundObstacle")
        {
            _linkedRoad.SpeedModifier = 1f;
            _canChangeRoad = true;
        }

        if (other.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "SkyObstacle")
        {
            _linkedRoad.SpeedModifier = 1.0f;
            _smoke.Stop();
            _onShadow = false;
        }

        if (other.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Train")
        {
            _linkedRoad.SpeedModifier = 1.0f;
        }
    }

    /*public void OnClickScreenButton(string message)
    {
        int _roadClicked;
        bool parsed = Int32.TryParse(message, out _roadClicked);

        if (!parsed)
            Debug.Log("Error-Value to go incorrect-VehicleManager.cs");
        else
        {
            GoDirection(_roadClicked);
        }
    }*/

    private void OnClickUp(Vector3 _upPosition)
    {
        Vector3 _diff = _upPosition - _clickPosition;
        if (Math.Abs(_diff.x) > Screen.width / 2 * _minimumXDisplacementForDrag)
        {
            if (_diff.x > 0)
                OnClickRight();
            else if (_diff.x < 0)
                OnClickLeft();
        }
        else
            ClickOnScreen(_upPosition);

    }

    private bool IsThisPointInMyHalfScreen(Vector3 _point)
    {
        _inputTimer.ResetTimer();

        if (_point.x < (_camera.rect.x + _camera.rect.width) * Screen.width && _point.x > _camera.rect.x * Screen.width)
            return true;
        return false;
    }

    private void ClickOnScreen(Vector3 mousePosition)
    {
        if (_controlledByAI == false)
        {
            RectTransform r1 = _movementButtons[0].GetComponent<RectTransform>();
            RectTransform r2 = _movementButtons[1].GetComponent<RectTransform>();

            bool res1 = RectTransformUtility.RectangleContainsScreenPoint(r1, new Vector2(mousePosition.x, mousePosition.y), null);
            bool res2 = RectTransformUtility.RectangleContainsScreenPoint(r2, new Vector2(mousePosition.x, mousePosition.y), null);

            if (res1 == false && res2 == false)
            {
                Ray ray = _camera.ScreenPointToRay(mousePosition);
                Vector3 direct = ray.direction * 200f;
                ray.direction *= 200f;
                Debug.DrawRay(ray.origin, direct, Color.red, 30f, true);
                RaycastHit hit;
                bool flag = Physics.Raycast(ray, out hit, 200f);
                if (flag)
                {
                    if (Debug.isDebugBuild) //develpment code 
                        Instantiate(_touchMarker, new Vector3(hit.point.x, hit.point.y + 0.15f, hit.point.z), Quaternion.Euler(90, 0, 0), _scoreManager.gameObject.transform);

                    if (hit.point.z > transform.position.z) //go right
                    {
                        OnClickRight();
                    }
                    else if (hit.point.z < transform.position.z)
                    {
                        OnClickLeft();
                    }
                }
            }
        }
    }

    /*private void GoDirection(int _roadClicked)
    {
        /*restar al que ir menos el actual
         ****si la resta menor que 0 vamos a la derecha
         ****si la resta mayor que 0 vamos a la izquierda
         ****llamar a la funcionright click or left click */

    /*int displacement = _roadClicked - (int)_currentLane;

    if (displacement > 0)
        OnClickRight();
    else if (displacement < 0)
        OnClickLeft();

}*/

    public int CollectablesToPoint
    {
        set
        {
            _collectablesToPoint = value;
        }
    }

    public bool IsInvulnerable
    {
        get
        {
            return _isInvulnerable;
        }
        set
        {
            _isInvulnerable = value;
        }
    }
    public Transform[] Lanes
    {
        get
        {
            Transform[] tmp = { _leftLane, _middleLane, _rightLane };
            return tmp;
        }
    }

    public bool ControlledByAI
    {
        get
        {
            return _controlledByAI;
        }
        set
        {
            _controlledByAI = value;
        }
    }

    public bool CanChangeRoad
    {
        get
        {
            return _canChangeRoad;
        }
        set
        {
            _canChangeRoad = value;
        }
    }

    //I need to know if lane changed or not
    public bool LeftWithConfirmation()
    {
        Debug.DrawRay(GetComponent<BoxCollider>().bounds.min, Vector3.back * Mathf.Abs(_rightLane.position.z - _leftLane.position.z) * 1.5f, Color.yellow, 1f);
        if (_canChangeRoad && _isInvulnerable == false && Physics.Raycast(GetComponent<BoxCollider>().bounds.min, Vector3.back, Mathf.Abs(_rightLane.position.z - _leftLane.position.z) * 1.5f, 1<<_obstaclesCollisionLayerMask) == false)
        {
            switch (_currentLane)
            {
                case CURRENTLANE.LEFT:
                    break;
                case CURRENTLANE.MIDDLE:
                    //transform.position = _leftLane.position;
                    _targetPosition = _leftLane.position;
                    _currentLane = CURRENTLANE.LEFT;
                    _animator.SetTrigger("Left");
                    return true;
                //break;
                case CURRENTLANE.RIGHT:
                    //transform.position = _middleLane.position;
                    _targetPosition = _middleLane.position;
                    _currentLane = CURRENTLANE.MIDDLE;
                    _animator.SetTrigger("Left");
                    return true;
                    //break;
            }
        }
        return false;
    }

    public bool RightWithConfirmation()
    {
        Debug.DrawRay(GetComponent<BoxCollider>().bounds.min, Vector3.forward * Mathf.Abs(_rightLane.position.z - _leftLane.position.z) * 1.5f, Color.yellow, 1f);
        if (_canChangeRoad && _isInvulnerable == false && Physics.Raycast(GetComponent<BoxCollider>().bounds.min, Vector3.forward, Mathf.Abs(_rightLane.position.z - _leftLane.position.z) * 1.5f, 1<<_obstaclesCollisionLayerMask) == false)
        {
            switch (_currentLane)
            {
                case CURRENTLANE.LEFT:
                    //transform.position = _middleLane.position;
                    _targetPosition = _middleLane.position;
                    _currentLane = CURRENTLANE.MIDDLE;
                    _animator.SetTrigger("Right");
                    return true;
                //break;
                case CURRENTLANE.MIDDLE:
                    //transform.position = _rightLane.position;
                    _targetPosition = _rightLane.position;
                    _currentLane = CURRENTLANE.RIGHT;
                    _animator.SetTrigger("Right");
                    return true;
                //break;
                case CURRENTLANE.RIGHT:
                    break;
            }
        }
        return false;
    }


    public int Player
    {
        get
        {
            return _player;
        }
    }
}
