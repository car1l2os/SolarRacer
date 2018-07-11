using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _playerForIA;
    private VehicleManager _vehicleManager;
    [SerializeField]
    private float _reactionTime = 0.2f;
    [SerializeField]
    private float _movementCoolDown = 0.5f;
    private float _timerLastReaction = 0f;
    private float _timerLastMovement = 0f;
    private float[] _distancesToObstacles = new float[3];
    private Transform[] _lanesToCheck = new Transform[3];
    [SerializeField]
    private float _checkDistance = 25f;

    [SerializeField]
    private int _targetLane = 0;
    private int _currentLane = 0;

    [SerializeField]
    GameObject[] _hudToDisableOnIA;
    [SerializeField]
    GameObject[] _hudToEnableOnIA;

    // Use this for initialization
    void Start()
    {
        _vehicleManager = _playerForIA.GetComponent<VehicleManager>();
        _lanesToCheck = _vehicleManager.Lanes;
        Enabled = StaticDataContainer._controlledByIA; ;
        GetDifficulty();
    }

    // Update is called once per frame
    void Update()
    {
        if (StaticDataContainer._controlledByIA)
        {
            CheckObstacles();
            _timerLastReaction -= Time.deltaTime;
            _timerLastMovement -= Time.deltaTime;
            if (_timerLastReaction <= 0f)
            {
                Decide();
                _timerLastReaction = _reactionTime;
            }
            if (_timerLastMovement <= 0f)
            {
                MoveToTargetLane();
            }
        }
    }

    private void CheckObstacles()
    {
        RaycastHit hit;
        for (int i = 0; i < _lanesToCheck.Length; i++)
        {
            bool hitFlag = Physics.Raycast(_lanesToCheck[i].position, Vector3.left, out hit, _checkDistance, 1 << 13);
            if (hitFlag)
                Debug.DrawRay(_lanesToCheck[i].position, Vector3.left * _checkDistance, Color.red, 1f);
            else
                Debug.DrawRay(_lanesToCheck[i].position, Vector3.left * _checkDistance, Color.green, 1f);

            if (hitFlag == true)
                _distancesToObstacles[i] = hit.distance;
            else
                _distancesToObstacles[i] = 100000000f;
        }
    }

    private void Decide()
    {
        _targetLane = _currentLane;
        float max = 0;

        for (int i = 0; i < _distancesToObstacles.Length; i++)
        {
            if (_distancesToObstacles[i] > max)
            {
                max = _distancesToObstacles[i];
                _targetLane = i - 1;
            }
        }

        if (StaticDataContainer.difficulty == StaticDataContainer.Difficulty.Hard)
            CheckForRamps();
    }

    private void CheckForRamps()
    {
        RaycastHit hit;
        for (int i = 0; i < _lanesToCheck.Length; i++)
        {
            bool hitFlag = Physics.Raycast(_lanesToCheck[i].position, Vector3.left, out hit, 25f, 1 << 15);
            if (hitFlag)
                Debug.DrawRay(_lanesToCheck[i].position, Vector3.left * 25f, Color.blue, 1f);
            else
                Debug.DrawRay(_lanesToCheck[i].position, Vector3.left * 25f, Color.red, 1f);

            if (hitFlag == true)
            {
                _targetLane = i - 1;
                break;
            }
        }
    }

    private void MoveToTargetLane()
    {
        if (_targetLane != _currentLane)
        {
            if (_targetLane < _currentLane)
            {
                Debug.DrawRay(_playerForIA.transform.position, Vector3.forward * 5f, Color.blue, _movementCoolDown);
                if (Physics.Raycast(_playerForIA.transform.position, Vector3.back, 5f) == false)
                {
                    if (_vehicleManager.LeftWithConfirmation())
                    {
                        _currentLane -= 1;
                        _timerLastMovement = _movementCoolDown;
                    }
                }
            }
            else
            {
                Debug.DrawRay(_playerForIA.transform.position, Vector3.forward * 5f, Color.blue, _movementCoolDown);
                if (Physics.Raycast(_playerForIA.transform.position, Vector3.back, 5f) == false)
                {
                    if (_vehicleManager.RightWithConfirmation())
                    {
                        _currentLane += 1;
                        _timerLastMovement = _movementCoolDown;
                    }

                }
            }
        }
    }

    public bool Enabled
    {
        get
        {
            return StaticDataContainer._controlledByIA; ;
        }

        set
        {
            _vehicleManager.ControlledByAI = value;
            for (int i = 0; i < _hudToDisableOnIA.Length; i++)
            {
                _hudToDisableOnIA[i].SetActive(!value);
            }

            if (value == true)
            {
                switch (StaticDataContainer.difficulty)
                {
                    case StaticDataContainer.Difficulty.Easy:
                        _hudToEnableOnIA[0].transform.GetChild(0).gameObject.SetActive(true);
                        _hudToEnableOnIA[0].transform.GetChild(1).gameObject.SetActive(false);
                        _hudToEnableOnIA[0].transform.GetChild(2).gameObject.SetActive(false);
                        break;

                    case StaticDataContainer.Difficulty.Medium:
                        _hudToEnableOnIA[0].transform.GetChild(0).gameObject.SetActive(false);
                        _hudToEnableOnIA[0].transform.GetChild(1).gameObject.SetActive(true);
                        _hudToEnableOnIA[0].transform.GetChild(2).gameObject.SetActive(false);
                        break;

                    case StaticDataContainer.Difficulty.Hard:
                        _hudToEnableOnIA[0].transform.GetChild(0).gameObject.SetActive(false);
                        _hudToEnableOnIA[0].transform.GetChild(1).gameObject.SetActive(false);
                        _hudToEnableOnIA[0].transform.GetChild(2).gameObject.SetActive(true);
                        break;
                }
            }
            else
            {
                _hudToEnableOnIA[0].transform.GetChild(0).gameObject.SetActive(false);
                _hudToEnableOnIA[0].transform.GetChild(1).gameObject.SetActive(false);
                _hudToEnableOnIA[0].transform.GetChild(2).gameObject.SetActive(false);
            }

        }
    }

    private void Easy()
    {
        _reactionTime = 1.5f;
        _movementCoolDown = 2.5f;
        _checkDistance = 50f;
    }

    private void Medium()
    {
        _reactionTime = 0.2f;
        _movementCoolDown = 0.4f;
        _checkDistance = 250f;
    }

    private void Hard()
    {
        _reactionTime = 0.1f;
        _movementCoolDown = 0.1f;
        _checkDistance = 50f;
    }

    private void GetDifficulty()
    {
        switch (StaticDataContainer.difficulty)
        {
            case StaticDataContainer.Difficulty.Easy:
                Easy();
                break;

            case StaticDataContainer.Difficulty.Medium:
                Medium();
                break;

            case StaticDataContainer.Difficulty.Hard:
                Hard();
                break;
        }
    }
}
