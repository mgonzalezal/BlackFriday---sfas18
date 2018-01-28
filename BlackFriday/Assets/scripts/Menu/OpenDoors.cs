using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the door animations when a player get close.
/// </summary>
public class OpenDoors : MonoBehaviour {

    Animator animator;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("IsPlayerNear", true);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("IsPlayerNear", false);
        }
    }

    public void OpenDoorsAudio()
    {
        transform.Find("OpenDoor").GetComponent<AudioSource>().Play();
    }
    public void CloseDoorsAudio()
    {
        transform.Find("CloseDoor").GetComponent<AudioSource>().Play();
    }
}
