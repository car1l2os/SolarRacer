using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class Feedback : MonoBehaviour {

    [SerializeField]
    private Slider[] _slider;

    private int _value;

    private bool _addExcelFormatting = false;

    // Use this for initialization
    void Start () {
		
	}
	

    public void FinishFeedback()
    {
        string path = Application.dataPath + "/SolarRacerFeedback.csv";

        FileStream file = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        file.Seek(0, SeekOrigin.End);

        file.Dispose();

        StreamReader reader = new StreamReader(path, true);
        string[] excelFormatting = new string[3];

        if (reader.ReadToEnd().Length <= 0)
        {
            excelFormatting[0] = "Aantal spelers,, Wat vind je van zonne energie? GEMIDDELDE,, Door dit spel weet ik meer over Zonne Energie GEMIDDELDE";
            excelFormatting[1] = " = COUNTA(A3: A10000),,= AVERAGE(C3: C10000),,= AVERAGE(E3: E10000)";
            excelFormatting[2] = ",,,,";
            _addExcelFormatting = true;
        }
        reader.Close();

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);

        if (_addExcelFormatting)
        {
          for(int i = 0; i < excelFormatting.Length; i++)
            {
                writer.WriteLine(excelFormatting[i]);
            }
        }

        writer.Write(System.DateTime.Now + ",");
        for (int i = 0; i < _slider.Length; i++)
        {
            string question = _slider[i].GetComponentInParent<Text>().text;
            int value = (int)_slider[i].value;

            writer.Write(question + "," + value + ",");
        }

        writer.WriteLine();

        writer.Close();

        Destroy(FindObjectOfType<AudioManager>());
        Destroy(FindObjectOfType<InputTimer>());

        SceneManager.LoadScene("MenuScene");
    }
}
