using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBehaviour : MonoBehaviour {

    Animator animator;
    int idPlayer;
    int idCar;

    // Use this for initialization
    void Start () {
		
	}

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    //Set up the car with the player id and the car number that has been activated.
    public void PlayerEnter(int id, int car)
    {
        animator.SetBool("PlayerEnter", true);
        idPlayer = id;
        idCar = car;
    }

    //Called by the animator when the car parks to spawn the player corresponding to that car.
    public void CarParked()
    {
        animator.SetBool("Parked", true);
        GameObject.Find("MenuController").GetComponent<MenuControllerScript>().SpawnPlayer(idPlayer, idCar);
        TurnOffEngine();
    }

    //Function to call when we press start to join the game.
    public void PlayHorn()
    {
        transform.Find("Claxon").gameObject.GetComponent<AudioSource>().Play();
        Invoke("TurnOnEngine", 0.5f);
        TurnOnEngine();
    }

    //Plays an engine sound when the car is moving
    public void TurnOnEngine()
    {
        transform.Find("Engine").gameObject.GetComponent<AudioSource>().Play();
    }

    //Stops the engine sound when the car is not moving
    public void TurnOffEngine()
    {
        transform.Find("Engine").gameObject.GetComponent<AudioSource>().Stop();
    }
}
