using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used in the exit screen, it is just a timer that scales down a gui bar to let the player know that
/// the exit menu is working and is it now frozen.
/// </summary>
public class ExitScript : MonoBehaviour {

    public float timeToExit;
    float timeToExitLeft;
    public GameObject timeLeftExit;

    // Use this for initialization
    void Start () {
        Time.timeScale = 1.0f;
        timeToExitLeft = timeToExit;
    }
	
	// Update is called once per frame
	void Update () {
        timeToExitLeft -= Time.deltaTime;
        Vector3 scaleNew = timeLeftExit.GetComponent<RectTransform>().localScale;
        scaleNew.x = timeToExitLeft / timeToExit;
        timeLeftExit.GetComponent<RectTransform>().localScale = scaleNew;
        if (timeToExitLeft <= 0.0f)
        {
            Application.Quit();
        }
	}
}
