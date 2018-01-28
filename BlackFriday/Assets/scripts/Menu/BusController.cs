using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BusState
{
    kBusState_Arrive,
    kBusState_Waiting,
    kBusState_Going,
    kBusState_Gone,
}

/// <summary>
/// Script that controls the bus at the main menu.
/// </summary>
public class BusController : MonoBehaviour {

    Animator animator;
    BusState busState;

    float timeWaiting;

    // Use this for initialization
    void Start () {
		
	}

    private void Awake()
    {
        animator = GetComponent<Animator>();
        busState = BusState.kBusState_Arrive;
    }

    // Update is called once per frame
    void Update () {
        switch (busState)
        {
            case BusState.kBusState_Waiting:
                {
                    //Debug.Log(timeWaiting);
                    timeWaiting -= Time.deltaTime;
                    if (timeWaiting <= 0.0f)
                    {
                        animator.SetBool("BusArrived", false);
                        animator.SetBool("BusEndWaiting", true);
                        busState = BusState.kBusState_Going;
                    }
                    break;
                }
            case BusState.kBusState_Gone:
                {
                    timeWaiting -= Time.deltaTime;
                    if (timeWaiting <= 0.0f)
                    {
                        animator.SetBool("BusGone", false);
                        animator.SetBool("BusEndWaiting", false);
                        busState = BusState.kBusState_Going;
                    }
                    break;
                }
        }
	}

    //Function called by the animation when the bus arrives to the stop
    public void BusArrive()
    {
        timeWaiting = Random.Range(5,10);
        busState = BusState.kBusState_Waiting;
        animator.SetBool("BusArrived", true);
    }

    //Function called by the animation when the bus is out of the screen
    public void BusGone()
    {
        timeWaiting = Random.Range(10, 20);
        busState = BusState.kBusState_Gone;
        animator.SetBool("BusGone", true);
    }
}
