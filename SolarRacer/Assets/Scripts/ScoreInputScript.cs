using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ScoreInputScript : MonoBehaviour {

    [SerializeField]
    private Text _nameTextField;
    [SerializeField]
    private GameObject[] _keyboard;
    [SerializeField]
    private GameObject _scoreBoard;
    [SerializeField]
    private Text[] _dailyTopTenNames;
    [SerializeField]
    private Text[] _dailyTopTenScores;
    [SerializeField]
    private Text[] _allTimeTopTenNames;
    [SerializeField]
    private Text[] _allTimeTopTenScores;
    
    //InputTimer _inputTimer;

    private string _name = "";

    private int _score = 1234;

    private Player _tempPlayer;

    string path;

    private List<Player> _dailyHighscoreList = new List<Player>();
    private List<Player> _allTimeHighscoreList = new List<Player>();

    AudioManager _audioManager;

    private void Awake()
    {
        FindObjectOfType<InputTimer>().ResetTimer();
        _audioManager = FindObjectOfType<AudioManager>();
        _audioManager.StopEverything();
         path = Application.dataPath + "/SolarRacer" + System.DateTime.Today.ToFileTime() + ".csv";
    }

    void Start () {
        
        //_inputTimer = GameObject.Find("InputTimer").GetComponent<InputTimer>(); //no really good but no other option


        if (StaticDataContainer._player1Points >= StaticDataContainer._player2Points)
        {
            _score = StaticDataContainer._player1Points;
        }
        else
        {
            _score = StaticDataContainer._player2Points;
        }
        

        if (StaticDataContainer._player2Points > StaticDataContainer._player1Points && StaticDataContainer._controledByIA)
        {
            _name = "Computer";
            SaveHighscore();
        }
    }

    public void AddLetter(Text text)
    {
        FindObjectOfType<InputTimer>().ResetTimer();
        //_inputTimer.ResetTimer();
        _name = _name + text.text;
        _nameTextField.text = _name;
        _audioManager.Play("LetterButton");
    }

    public void BackSpace()
    {
        FindObjectOfType<InputTimer>().ResetTimer();
        //_inputTimer.ResetTimer();
        if (_name.Length == 0) {
            return;
        }
        _name = _name.Remove(_name.Length-1);
        _nameTextField.text = _name;
        _audioManager.Play("LetterButton");
    }

    public void SaveHighscore()
    {
        FindObjectOfType<InputTimer>().ResetTimer();
        _audioManager.Play("UIButton");

        if (_name == "")
        {
            _name = "Pikachu";
        }
        FileStream file = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        file.Seek(0, SeekOrigin.End);
     
        file.Dispose();
        StreamReader reader = new StreamReader(path, true);

        string[] strings;
        string[] nameAndScore;
        
        strings = reader.ReadToEnd().Split('\n');
        
        
        for (int i = 0; i < strings.Length-1; i++)
        {
            nameAndScore = strings[i].Split(',');
            _tempPlayer = new Player(nameAndScore[0], int.Parse(nameAndScore[1]));
            _dailyHighscoreList.Add(_tempPlayer);
        }

        reader.Close();

        _dailyHighscoreList.Add(new Player(_name, _score));
        _dailyHighscoreList.Sort((s1, s2) => s1.Score.CompareTo(s2.Score));
        _dailyHighscoreList.Reverse();

        while (_dailyHighscoreList.Count > 10)
        {
            _dailyHighscoreList.RemoveAt(_dailyHighscoreList.Count - 1);
        }

        //ClearFile
        FileStream fileStream = File.Open(path, FileMode.Open);
        fileStream.SetLength(0);
        fileStream.Close();

        StreamWriter writer = new StreamWriter(path, true);
        
        for (int i = 0; i < _dailyHighscoreList.Count; i++)
        {
            writer.WriteLine(_dailyHighscoreList[i].Name + "," + _dailyHighscoreList[i].Score);
        }
        
        writer.Close();

        CheckAllTimeHighscores();
    }

    private void CheckAllTimeHighscores()
    {
        FindObjectOfType<InputTimer>().ResetTimer();
        _tempPlayer = null;
        path = Application.dataPath + "/SolarRacerAllTimeHighscores.csv";

        FileStream file = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        file.Seek(0, SeekOrigin.End);
     
        file.Dispose();
        StreamReader reader = new StreamReader(path, true);

        string[] strings;
        string[] nameAndScore;
        
        strings = reader.ReadToEnd().Split('\n');
        
        
        for (int i = 0; i < strings.Length-1; i++)
        {
            Debug.Log(i);
            nameAndScore = strings[i].Split(',');
            _tempPlayer = new Player(nameAndScore[0], int.Parse(nameAndScore[1]));
            _allTimeHighscoreList.Add(_tempPlayer);
        }

        reader.Close();

        _allTimeHighscoreList.Add(new Player(_name, _score));
        _allTimeHighscoreList.Sort((s1, s2) => s1.Score.CompareTo(s2.Score));
        _allTimeHighscoreList.Reverse();

        while (_allTimeHighscoreList.Count > 10)
        {
            _allTimeHighscoreList.RemoveAt(_dailyHighscoreList.Count - 1);
        }

        //ClearFile
        FileStream fileStream = File.Open(path, FileMode.Open);
        fileStream.SetLength(0);
        fileStream.Close();

        StreamWriter writer = new StreamWriter(path, true);
        
        for (int i = 0; i < _allTimeHighscoreList.Count; i++)
        {
            writer.WriteLine(_allTimeHighscoreList[i].Name + "," + _allTimeHighscoreList[i].Score);
        }
        
        writer.Close();

        DisplayHighscores();
    }

    private void DisplayHighscores()
    {
        FindObjectOfType<InputTimer>().ResetTimer();
        for (int i = 0; i < _keyboard.Length; i++)
        {
            _keyboard[i].SetActive(false);
        }

        for (int i = 0; i < _dailyHighscoreList.Count; i++) 
        {
            _dailyTopTenNames[i].text = _dailyHighscoreList[i].Name;
            _dailyTopTenScores[i].text = _dailyHighscoreList[i].Score.ToString();
        }
        
        for(int i = 0; i< _allTimeHighscoreList.Count; i++)
        {
            _allTimeTopTenNames[i].text = _dailyHighscoreList[i].Name;
            _allTimeTopTenScores[i].text = _dailyHighscoreList[i].Score.ToString();
        }

        _scoreBoard.SetActive(true);

        StaticDataContainer.ResetStats();
    }

    public class Player
    {
        public Player(string name, int score)
        {
            Name = name;
            Score = score;
        }

        public string Name { get; set; }
        public int Score { get; set; }
    }
}
