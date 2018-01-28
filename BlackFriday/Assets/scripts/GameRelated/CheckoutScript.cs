using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CheckoutStatus { PlaceItem, PayItem, Done };

/// <summary>
/// This is the script for the behaviour of the checkout in the game.
/// </summary>
public class CheckoutScript : MonoBehaviour
{
    GameObject itemToCheckOut;
    GameObject checkoutPosition;
    GameController gameController;
    ParticleSystem particlesCheckout;

    public AudioSource CheckoutSound;
    public float timeToPlaceItem;
    public float timeToPay;

    float timeToPlaceItemLeft;
    float timeToPayLeft;

    CheckoutStatus checkOutStatus;

    Vector3 positionStart;
    Vector3 scaleStart;
    Quaternion rotationStart;

    Vector3 scaleDest;

    public bool isCheckoutGame;

    string playerIdCheckingout;

    // Use this for initialization
    void Start()
    {
        checkoutPosition = transform.parent.Find("CheckOutPoint").gameObject;
        particlesCheckout = transform.parent.Find("CheckOutParticles").gameObject.GetComponent<ParticleSystem>();
    }

    void Awake()
    {
        if (isCheckoutGame) gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        //If there is an item to check out registered, execute this.
        if (itemToCheckOut)
        {
            //Checkout switch, Place item just takes the item from the player and places it on the checkout.
            //Pay item is just a delay when paying the item, nothing really happening just a timer.
            //Done status plays sounds and particles, adds money the player and destroys the item.
            switch (checkOutStatus)
            {
                case CheckoutStatus.PlaceItem:
                    {
                        itemToCheckOut.transform.position = Vector3.Lerp(positionStart, checkoutPosition.transform.position, 1.0f - (timeToPlaceItemLeft / timeToPlaceItem));
                        //itemToCheckOut.transform.rotation = Quaternion.Lerp(rotationStart, checkoutPosition.transform.rotation, 1.0f - (timeToPlaceItemLeft / timeToPlaceItem));
                        itemToCheckOut.transform.localScale = Vector3.Lerp(scaleStart, scaleDest, 1.0f - (timeToPlaceItemLeft / timeToPlaceItem));
                        timeToPlaceItemLeft -= Time.deltaTime;
                        if (timeToPlaceItemLeft <= 0.0f)
                        {
                            checkOutStatus++;
                        }
                        break;
                    }
                case CheckoutStatus.PayItem:
                    {
                        timeToPayLeft -= Time.deltaTime;
                        if (timeToPayLeft <= 0.0f)
                        {
                            checkOutStatus++;
                        }
                        break;
                    }
                case CheckoutStatus.Done:
                    {
                        CheckoutSound.Play();
                        particlesCheckout.Play();
                        itemToCheckOut.GetComponent<PickUpObjectBehaviour>().DoneCheckOut();
                        if (isCheckoutGame)
                        {
                            gameController.DoneCheckout();
                            gameController.AddMoneyTo(playerIdCheckingout, itemToCheckOut.GetComponent<PickUpObjectBehaviour>().GetMoneySaved());
                        }
                        Destroy(itemToCheckOut);
                        itemToCheckOut = null;                        
                        break;
                    }
            }
        }
    }

    //When a player enters the collider, we check if there is an item being checkout at the moment
    //that is not supposed to happen, but it is just to avoid bugs.
    //Then we check if the player has an item to checkout, and we configure the checkout with that info.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !itemToCheckOut)
        {
            PlayerBehaviour playerScript = other.gameObject.GetComponent<PlayerBehaviour>();
            itemToCheckOut = playerScript.CheckoutItem();
            playerIdCheckingout = playerScript.GetPlayerNumber();
            if (itemToCheckOut)
            {
                timeToPlaceItemLeft = timeToPlaceItem;
                timeToPayLeft = timeToPay;
                checkOutStatus = CheckoutStatus.PlaceItem;
                positionStart = itemToCheckOut.transform.position;
                rotationStart = itemToCheckOut.transform.rotation;
                scaleStart = itemToCheckOut.transform.localScale;
                itemToCheckOut.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                itemToCheckOut.GetComponent<Rigidbody>().velocity = Vector3.zero;

                scaleDest = itemToCheckOut.GetComponent<PickUpObjectBehaviour>().GetStartScale();
                if(isCheckoutGame) gameController.CheckoutItem();
            }
        }
    }
}
