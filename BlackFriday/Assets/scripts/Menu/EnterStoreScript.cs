using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to control the trigger inside the store, when in the menu we cross the doors.
/// </summary>
public class EnterStoreScript : MonoBehaviour {

    MenuControllerScript menuController;

	// Use this for initialization
	void Start () {
		
	}

    private void Awake()
    {
        menuController = GameObject.Find("MenuController").GetComponent<MenuControllerScript>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    //Deactivates the player that has entered the store and does the register in the menu controller.
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerBehaviour>().ActivatePlayer(false);
            menuController.PlayerEnterSupermarket();
        }
    }
}
