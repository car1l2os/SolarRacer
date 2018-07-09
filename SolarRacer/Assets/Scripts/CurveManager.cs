using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveManager : MonoBehaviour
{
    private Material[] _materials;

    [SerializeField]
    private Vector2 _minMaxYCurvature;
    [SerializeField]
    private Vector2 _minMaxXCurvature;


    Vector2 displacement = Vector2.zero;
    float _targetYDisplacement;
    float _targetXDisplacement;
    float _xLerpTime = 0.0f;
    float _yLerpTime = 0.0f;
    float _timeToNewCurvature = 15f;
    float _rate;
    public static bool _gameRunning = false;

    // Use this for initialization
    void Start()
    {
        _rate = 1.0f / _timeToNewCurvature;
        Object[] _objects = Resources.LoadAll("CurvedMaterials", typeof(Material));
        _materials = new Material[_objects.Length];



        displacement.x = Random.Range(_minMaxXCurvature.x, _minMaxXCurvature.y);
        displacement.y = Random.Range(_minMaxYCurvature.x, _minMaxYCurvature.y);
        _targetXDisplacement = displacement.x;
        _targetYDisplacement = displacement.y;

        for (int i = 0; i < _objects.Length; i++)
        {
            _materials[i] = (Material)_objects[i];
            _materials[i].SetVector("_QOffset", new Vector4(0, _targetYDisplacement,_targetXDisplacement));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameRunning)
        {
            foreach (Material mat in _materials)
            {
                mat.SetVector("_QOffset", new Vector4(0, displacement.y, displacement.x));
            }
            UpdateDisplacement();
            UpdateLerpTimes(Time.deltaTime);
        }
    }

    public void ResetCurvature()
    {
        foreach (Material mat in _materials)
        {
            mat.SetVector("_QOffset", new Vector4(0, 0, 0));
        }
    }

    private void UpdateDisplacement()
    {
        if (displacement.x != _targetXDisplacement)
            displacement.x = Mathf.Lerp(displacement.x, _targetXDisplacement, _xLerpTime);
        else
        {
            _xLerpTime = 0.0f;
            _targetXDisplacement = Random.Range(Mathf.Clamp(_targetXDisplacement - 10f, _minMaxXCurvature.x, _minMaxXCurvature.y), Mathf.Clamp(_targetXDisplacement + 10f, _minMaxXCurvature.x, _minMaxXCurvature.y));
        }

    }

    private void UpdateLerpTimes(float time)
    {
        _xLerpTime += time * _rate;
        _yLerpTime += time * time;
    }
}

