using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Power : MonoBehaviour
{

    [SerializeField]
    private Road _playerRoad;
    [SerializeField]
    private Image _powerBarImage;
    [SerializeField]
    private Image _powerBarOutlineImage;
    [SerializeField]
    private Image _powerBarTopOutlineImage;


    [SerializeField]
    private float _maxPower = 100;
    [SerializeField]
    private float _currentPower = 25f;
    [SerializeField]
    private float _powerGain = 0.2f;
    [SerializeField]
    private float _losePowerModifier = 2.5f;

    private bool _powerLost = false;

    [SerializeField]
    private MeshRenderer _solarPanelMeshRenderer;
    [SerializeField]
    private Material _lightUpSolarPanelMaterial;
    private Material _defaultMaterial;

    [SerializeField]
    private ParticleSystem[] _fullEnergyParticles;

    VehicleManager _car;

    private bool _ghettoTimerToStartUpdating = false;
    private bool _ghettoTimerPollution = true;


    // Use this for initialization
    void Start()
    {
        _currentPower = 25f;
        _defaultMaterial = _solarPanelMeshRenderer.materials[0];
        _car = _solarPanelMeshRenderer.GetComponentInParent<VehicleManager>();
        
        //Invoke("GhettoTimerStartUpdating", 4f);
        UpdateImage();
    }

    public void AddPower(float pPowerAdded)
    {
        _currentPower = _currentPower + pPowerAdded;

        if (_currentPower >= _maxPower)
        {
            _currentPower = _maxPower;
            foreach (ParticleSystem ps in _fullEnergyParticles)
            {
                if (ps.isPlaying == false)
                    ps.Play();
            }

        }
        else
        {
            foreach (ParticleSystem ps in _fullEnergyParticles)
                ps.Stop();
        }

        UpdateImage();
    }

    public void LosePower(float pPowerLost)
    {
        _currentPower = _currentPower - pPowerLost;

        if (_currentPower < 25f)
        {
            _currentPower = 25f;
        }

        UpdateImage();
    }

    private void UpdateImage()
    {
        float powerPercentage = _currentPower / _maxPower;
        _powerBarImage.fillAmount = powerPercentage;
        _powerBarOutlineImage.fillAmount = powerPercentage;

        if (powerPercentage >= 1.0f)
        {
            _powerBarTopOutlineImage.gameObject.SetActive(true);
        }
        else
        {
            _powerBarTopOutlineImage.gameObject.SetActive(false);
        }
    }

    private void LightUpSolarPanel()
    {
        _solarPanelMeshRenderer.material = _lightUpSolarPanelMaterial;
    }

    private void DarkenSolarPanel()
    {
        _solarPanelMeshRenderer.material = _defaultMaterial;
    }

    private void Update()
    {
        if(_car._onShadow)
        {
            DarkenSolarPanel();
        }
        else
        {
            LightUpSolarPanel();
        }
    }

    private void GhettoTimerPollution()
    {
        _ghettoTimerPollution = true;
    }

    private void GhettoTimerStartUpdating()
    {
        _ghettoTimerToStartUpdating = true;
    }

    public float MaxPower
    {
        get
        {
            return _maxPower;
        }
    }

    public float CurrentPower
    {
        get
        {
            return _currentPower;
        }
    }

    public float NormalizedPowerModifier
    {
        get
        {
            return _currentPower / _maxPower;
        }
    }

}
