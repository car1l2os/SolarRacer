using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour {

    [SerializeField]
    private string _gameSceneName;
    [SerializeField]
    private Image _creditsImage;

    [SerializeField]
    private GameObject _mainMenuObjects;
    [SerializeField]
    private GameObject _modeSelectObjects;
    [SerializeField]
    private GameObject _difficultySelectObjects;

    private bool _creditsClicked = false;
    private bool _creditsReachedPosition = false;

    [SerializeField]
    AudioManager _audioManager;

    public void OnClickStart()
    {
        _mainMenuObjects.SetActive(false);
        _modeSelectObjects.SetActive(true);
        _audioManager.Play("UIButton");
        StaticDataContainer.ResetStats();
    }

    public void OnClickCredits()
    {
        _creditsClicked = true;
        _audioManager.Play("UIButton");
    }

    public void OnClickSinglePlayer()
    {
        StaticDataContainer._controledByIA = true;
        _mainMenuObjects.SetActive(false);
        _modeSelectObjects.SetActive(false);
        _difficultySelectObjects.SetActive(true);
        _audioManager.Play("UIButton");
    }

    public void OnSelectedDifficulty(float difficulty)
    {
        if(difficulty == 0.01f) //values on buttons
            StaticDataContainer.difficulty = StaticDataContainer.Difficulty.Hard;
        else if(difficulty == 0.5f)
            StaticDataContainer.difficulty = StaticDataContainer.Difficulty.Medium;
        else if (difficulty == 0.9f)
            StaticDataContainer.difficulty = StaticDataContainer.Difficulty.Easy;
        _audioManager.Play("UIButton");
        SceneManager.LoadScene(_gameSceneName);
    }

    public void OnClickMultiPlayer()
    {
        StaticDataContainer._controledByIA = false;
        SceneManager.LoadScene(_gameSceneName);
        _audioManager.Play("UIButton");
    }

    public void OnClickBack()
    {
        _modeSelectObjects.SetActive(false);
        _mainMenuObjects.SetActive(true);
        _audioManager.Play("UIButton");
    }

    private void Update()
    {
        if (!_creditsClicked)
        {
            return;
        }

        if (_creditsImage.fillAmount <= 1.0f && !_creditsReachedPosition)
        {
            _creditsImage.fillAmount += 2.5f * Time.deltaTime;
        }

        if (_creditsImage.fillAmount >= 1.0f && !_creditsReachedPosition)
        {
            _creditsImage.fillAmount = 1.0f;
            _creditsClicked = false;
            _creditsReachedPosition = true;
        }

        if(_creditsReachedPosition)
        {
            _creditsImage.fillAmount -= 2.5f * Time.deltaTime;
        }

        if (_creditsImage.fillAmount <= 0.0f && _creditsReachedPosition)
        {
            _creditsImage.fillAmount = 0.0f;
            _creditsReachedPosition = false;
            _creditsClicked = false;
        }
    }
}
