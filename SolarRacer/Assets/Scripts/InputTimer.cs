using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputTimer : MonoBehaviour
{

    private float _timer;

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "MenuScene")
            _timer += Time.deltaTime;

        if (_timer >= 30.0f)
        {
            SceneManager.LoadScene("MenuScene");
            _timer = 0.0f;

            FindObjectOfType<AudioManager>().StopEverything();
            Destroy(FindObjectOfType<AudioManager>().gameObject);
            Destroy(gameObject);
        }

        Debug.Log(_timer);
    }

    public void ResetTimer()
    {
        _timer = 0.0f;
    }
}
