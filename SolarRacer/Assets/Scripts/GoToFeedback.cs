using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToFeedback : MonoBehaviour {
    public void OnClick()
    {
        SceneManager.LoadScene("FeedbackScene");
    }
}
