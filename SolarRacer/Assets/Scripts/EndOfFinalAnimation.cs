using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfFinalAnimation : MonoBehaviour {

	public void EndGame()
    {
        transform.parent.GetComponent<ScoreManager>().FinalAnimationDone = true;
    }
}
