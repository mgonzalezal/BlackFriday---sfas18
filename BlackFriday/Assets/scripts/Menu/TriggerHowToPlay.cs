using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Controls the trigger to activate the how to play canvas.
/// </summary>
public class TriggerHowToPlay : MonoBehaviour {

    public GameObject howToPlayCanvas;
    bool isShowingHowToPlay;

	// Use this for initialization
	void Start () {
        isShowingHowToPlay = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (isShowingHowToPlay)
        {
            if (Input.GetButtonUp("ExitMenu"))
            {
                isShowingHowToPlay = false;
                howToPlayCanvas.GetComponent<Animator>().SetBool("ShowHowToPlay", false);
                Time.timeScale = 1.0f;
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isShowingHowToPlay)
        {
            isShowingHowToPlay = true;
            howToPlayCanvas.GetComponent<Animator>().SetBool("ShowHowToPlay", true);
            Debug.Log("SHOW IT TO PAPPI");
            Time.timeScale = 0.0f;
        }
    }
}
