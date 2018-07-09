using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ScoreManager : MonoBehaviour
{

    [Header("Roads")]
    [SerializeField]
    Road _roadPlayer1;
    [SerializeField]
    Road _roadPlayer2;

    [Header("Score texts")]
    [SerializeField]
    private Text _scoreTextPlayer1;
    [SerializeField]
    private Text _scoreTextPlayer2;
    [SerializeField]
    private Text _winLosePlayer1;
    [SerializeField]
    private Text _winLosePlayer2;
    [SerializeField]
    private GameObject _player1ScorePlace;
    [SerializeField]
    private GameObject _player2ScorePlace;

    [Header("Final info table")]
    [SerializeField]
    private Text _pointsPlayer1;
    [SerializeField]
    private Text _pointsPlayer2;
    [SerializeField]
    private Text _collectablesPlayer1;
    [SerializeField]
    private Text _collectablePointsPlayer1;
    [SerializeField]
    private Text _collectablesPlayer2;
    [SerializeField]
    private Text _collectablePointsPlayer2;
    [SerializeField]
    private Text _pickUpsPlayer1;
    [SerializeField]
    private Text _pickUpsPointsPlayer1;
    [SerializeField]
    private Text _pickUpsPlayer2;
    [SerializeField]
    private Text _pickUpsPointsPlayer2;
    [SerializeField]
    private Text _secondsOnShadowPlayer1;
    [SerializeField]
    private Text _secondsOnShadowPlayer2;
    [SerializeField]
    private GameObject _fullStarsPlayer1;
    [SerializeField]
    private GameObject _fullStarsPlayer2;
    

    [Header("Score events prefab")]
    [SerializeField]
    GameObject _roadEventPlayer1;
    [SerializeField]
    GameObject _roadEventPlayer2;
    [SerializeField]
    GameObject _collectibleEventPlayer1;
    [SerializeField]
    GameObject _collectibleEventPlayer2;

    private float _totalScorePlayer1 = 0f;
    private float _totalScorePlayer2 = 0f;

    private Queue<GameObject> _freePlayer1RoadScoreEvents = new Queue<GameObject>();
    private Queue<GameObject> _freePlayer1CollectiblesScoreEvents = new Queue<GameObject>();

    private Queue<GameObject> _freePlayer2RoadScoreEvents = new Queue<GameObject>();
    private Queue<GameObject> _freePlayer2CollectiblesScoreEvents = new Queue<GameObject>();


    //placeholder
    [Header("Placeholders-Playtesting")]
    [SerializeField]
    GameObject[] _buttons;
    private bool _finalAnimationDone = false;
    [SerializeField]
    GameObject _restartButton;



    private void Start()
    {
        // _scoreTextPlayer1 = _scorePlayer1.GetComponent<Text>();
        // _scoreTextPlayer2 = _scorePlayer2.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_roadPlayer1.FinishReached == false && _roadPlayer2.FinishReached == false)
        {
            UpdateScore();
            PrintScoreInHud();
        }
        else
        {
            FindObjectOfType<InputTimer>().ResetTimer();
            _roadPlayer1.RoadSpeed = 0f;
            _roadPlayer2.RoadSpeed = 0f;
            PrintResults();
            PrintTableResults();
            PutStarsToPlayers();
            StorePoints();
            GetComponent<Animator>().SetTrigger("EndGame");
        }

        /*if(Input.GetKeyDown(KeyCode.Space))
        {
            PrintResults();
            GetComponent<Animator>().SetTrigger("EndGame");
            _roadPlayer1.FinishReached = true;
            _roadPlayer1.FinishReached = true;
        }*/

    }

    private void PutStarsToPlayers()
    {

        int _starsP1 = NumberOfStars(1);
        for (int i = 0; i < _starsP1; i++)
        {
            _fullStarsPlayer1.transform.GetChild(i).gameObject.SetActive(true);
        }

        _starsP1 = NumberOfStars(2);
        for (int i = 0; i < _starsP1; i++)
        {
            _fullStarsPlayer2.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private int NumberOfStars(int player)
    {
        int number = 0;
        if (player == 1)
        {
            if (_totalScorePlayer1 > 4500)
                number++;
            if (StaticDataContainer._player1SecondsInShadow < 0.5f)
                number++;
            if (StaticDataContainer._player1Collectables > 3)
                number++;
        }
        else if (player == 2)
        {
            if (_totalScorePlayer2 > 4500)
                number++;
            if (StaticDataContainer._player2SecondsInShadow < 0.5f)
                number++;
            if (StaticDataContainer._player2Collectables > 3)
                number++;
        }
        return number;
    }

    private void StorePoints()
    {
        StaticDataContainer._player1Points = Convert.ToInt32(_totalScorePlayer1);
        StaticDataContainer._player2Points = Convert.ToInt32(_totalScorePlayer2);
    }

    private void DisableButtons() //Unused ?
    {
        foreach (GameObject button in _buttons)
        {
            button.SetActive(false);
        }
    }

    private void PrintResults()
    {
        if (StaticDataContainer._firstInFinish == 1)
        {
            _winLosePlayer1.text = "Jij wint";
            _winLosePlayer1.color = Color.green;

            _winLosePlayer2.text = "Jij verliest";
            _winLosePlayer2.color = Color.red;

        }
        else
        {
            _winLosePlayer1.text = "Jij verliest";
            _winLosePlayer1.color = Color.red;

            _winLosePlayer2.text = "Jij wint";
            _winLosePlayer2.color = Color.green;
        }
        /*else //no draw in new win condition
        {
            _winLosePlayer1.text = "Draw";
            _winLosePlayer1.color = Color.black;

            _winLosePlayer2.text = "Draw";
            _winLosePlayer2.color = Color.black;
        }*/
    }

    private void PrintTableResults()
    {
        _pointsPlayer1.text = _totalScorePlayer1.ToString();
        _pointsPlayer2.text = _totalScorePlayer2.ToString();

        _collectablesPlayer1.text = "X " + StaticDataContainer._player1Collectables.ToString();
        _collectablesPlayer2.text = "X " + StaticDataContainer._player2Collectables.ToString();

        _collectablePointsPlayer1.text = (Mathf.CeilToInt(StaticDataContainer._player1Collectables / 3) * StaticDataContainer._pointsPerCollectableGroup).ToString();
        _collectablePointsPlayer2.text = (Mathf.CeilToInt(StaticDataContainer._player2Collectables / 3) * StaticDataContainer._pointsPerCollectableGroup).ToString();

        _pickUpsPlayer1.text = "X " + StaticDataContainer._player1PickUps.ToString();
        _pickUpsPlayer2.text = "X " + StaticDataContainer._player2PickUps.ToString();

        _pickUpsPointsPlayer1.text = (StaticDataContainer._player1PickUps * StaticDataContainer._pointsPerPickUp).ToString();
        _pickUpsPointsPlayer2.text = (StaticDataContainer._player2PickUps * StaticDataContainer._pointsPerPickUp).ToString();

        _secondsOnShadowPlayer1.text = "Je hebt voor " + StaticDataContainer._player1SecondsInShadow.ToString("F1") + " seconden vervuild";
        _secondsOnShadowPlayer2.text = "Je hebt voor " + StaticDataContainer._player2SecondsInShadow.ToString("F1") + " seconden vervuild";
    }

    private void UpdateScore()
    {
        AddPointsToPlayer(_roadPlayer1.ModifiedSpeed / 100.0f, 1);
        AddPointsToPlayer(_roadPlayer2.ModifiedSpeed / 100.0f, 2);
    }

    private void PrintScoreInHud()
    {
        _scoreTextPlayer1.text = _totalScorePlayer1.ToString();
        _scoreTextPlayer2.text = _totalScorePlayer2.ToString();
    }

    public void LaunchEventInScore(float points, int player, bool roadPoints)
    {
        if (_roadPlayer1.FinishReached == false && _roadPlayer2.FinishReached == false)
        {
            GameObject tmpGO;
            tmpGO = GetEvent(player,roadPoints);

            AddPointsToPlayer(points, player);

            if (points >= 0)
            {
                //green text appear
                tmpGO.GetComponent<Text>().color = Color.green;
                tmpGO.GetComponent<Text>().text = "+" + points;
            }
            else
            {
                //red text appear
                tmpGO.GetComponent<Text>().color = Color.red;
                tmpGO.GetComponent<Text>().text = points.ToString(); //not needed add - before because negative number
            }
        }
    }

    public void AddPointsToPlayer(float points, int player)
    {
        if (player == 1)
            _totalScorePlayer1 += points;
        else
            _totalScorePlayer2 += points;

        if (_totalScorePlayer1 < 0)
        {
            _totalScorePlayer1 = 0;
        }

        if (_totalScorePlayer2 < 0)
        {
            _totalScorePlayer2 = 0;
        }

        _totalScorePlayer1 = (float)Math.Round(_totalScorePlayer1, 2);
        _totalScorePlayer2 = (float)Math.Round(_totalScorePlayer2, 2);
    }

    private GameObject GetEvent(int player, bool road)
    {
        GameObject tmpGO;
        if (player == 1)
        {
            if (road)
            {
                if (_freePlayer1RoadScoreEvents.Count == 0)
                    tmpGO = Instantiate(_roadEventPlayer1, transform);
                else
                {
                    tmpGO = _freePlayer1RoadScoreEvents.Dequeue();
                    tmpGO.SetActive(true);
                }
            }
            else
            {
                if (_freePlayer1CollectiblesScoreEvents.Count == 0)
                    tmpGO = Instantiate(_collectibleEventPlayer1, transform);
                else
                {
                    tmpGO = _freePlayer1CollectiblesScoreEvents.Dequeue();
                    tmpGO.SetActive(true);
                }
            }
        }
        else
        {
            if (road)
            {
                if (_freePlayer2RoadScoreEvents.Count == 0)
                    tmpGO = Instantiate(_roadEventPlayer2, transform);
                else
                {
                    tmpGO = _freePlayer2RoadScoreEvents.Dequeue();
                    tmpGO.SetActive(true);
                }
            }
            else
            {
                if (_freePlayer2CollectiblesScoreEvents.Count == 0)
                    tmpGO = Instantiate(_collectibleEventPlayer2, transform);
                else
                {
                    tmpGO = _freePlayer2CollectiblesScoreEvents.Dequeue();
                    tmpGO.SetActive(true);
                }
            }
        }
        return tmpGO;
    }

    public void AddToFreeScoreEvent(GameObject toAdd, int player, bool road)
    {
        if (player == 1)
        {
            if (road)
                _freePlayer1RoadScoreEvents.Enqueue(toAdd);
            else
                _freePlayer1CollectiblesScoreEvents.Enqueue(toAdd);
        }
        else
        {
            if (road)
                _freePlayer2RoadScoreEvents.Enqueue(toAdd);
            else
                _freePlayer2CollectiblesScoreEvents.Enqueue(toAdd);
        }
    }




    public bool FinalAnimationDone
    {
        set
        {
            _finalAnimationDone = value;
        }
    }
}
