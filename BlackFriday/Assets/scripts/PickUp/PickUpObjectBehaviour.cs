using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EffectOnCheckout
{
    kEffectOnCheckout_GivePoints,
    kEffectOnCheckout_LoadLevel,
}

/// <summary>
/// Handles the behaviour of the pick ups.
/// </summary>
public class PickUpObjectBehaviour : MonoBehaviour
{

    public bool objectPicked = false;
    bool isBeingCheckOut = false;
    bool isReseting = false;
    SphereCollider colliderPickUp;
    Collider colliderObject;
    Rigidbody rigidbodyObject;
    Vector3 startScale;
    GameController gameController;

    [Header("Object config")]
    public float resistanceToPickUp = 0.0f;
    public EffectOnCheckout effecOnCheckOut;
    public bool isObjective = true;
    public bool startWithColliderActivated;

    [Header("GivePoints config")]
    public int originalPrice;
    public int discount;

    [Header("LoadLevel config")]
    public string LevelToLoad;

    Vector3 startPosition;
    Quaternion startRotation;

    GameObject viewZone;


    // Use this for initialization
    void Start()
    {
        objectPicked = false;
        colliderPickUp = GetComponent<SphereCollider>();
        colliderObject = GetComponent<BoxCollider>();
        rigidbodyObject = GetComponent<Rigidbody>();
        colliderPickUp.enabled = isObjective;
        rigidbodyObject.useGravity = startWithColliderActivated;
        colliderObject.enabled = startWithColliderActivated;
        startScale = transform.localScale;
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Awake()
    {
        GameObject controller = GameObject.Find("GameController");
        if (controller)
        {
            gameController = controller.GetComponent<GameController>();
        }

        Transform viewZoneTmp = transform.Find("ViewZone");
        if (viewZoneTmp)
        {
            viewZone = viewZoneTmp.gameObject;
        }
        else
        {
            Debug.Log(this.name);
        }
    }

    //Used by the camera to focus this object.
    public Vector3 getViewOffset()
    {
        return viewZone.transform.position;
    }

    //Used by the camera to focus this object.
    public Vector3 getPositionLook()
    {
        return (viewZone.transform.position) + viewZone.transform.forward * 2.0f;
    }
    
    // Update is called once per frame
    void Update()
    {
        //If the object is not picked, not being checkout and it is the objective, reset the scale to the starting one.
        if (!objectPicked && !isBeingCheckOut && isObjective)
        {
            transform.localScale = Vector3.Lerp(transform.lossyScale, startScale, Time.deltaTime * 5.0f);
        }
        //If the object is reseting, lerp the scale to zero, and when it is done, teleport it to the starting position.
        if (isReseting)
        {
            transform.localScale = Vector3.Lerp(transform.lossyScale, Vector3.zero, Time.deltaTime * 0.5f);
            if (transform.localScale.x <= 0.1f)
            {
                transform.position = startPosition;
                transform.rotation = startRotation;
                transform.localScale = startScale;
                rigidbodyObject.velocity = Vector3.zero;
                rigidbodyObject.angularVelocity = Vector3.zero;
                isReseting = false;
            }
        }
    }

    //returns the resitance of the object to be picked.
    public float getResitanceToPick()
    {
        return resistanceToPickUp;
    }

    //Sets the object to be checked out.
    public void CheckOut()
    {
        isBeingCheckOut = true;
    }

    //Old function not being used now.
    public void DoneCheckOut()
    {
        switch (effecOnCheckOut)
        {
            case EffectOnCheckout.kEffectOnCheckout_GivePoints:
                {
                    break;
                }
            case EffectOnCheckout.kEffectOnCheckout_LoadLevel:
                {
                    SceneManager.LoadScene(LevelToLoad);
                    break;
                }
        }
    }

    public Vector3 GetStartScale()
    {
        return startScale;
    }

    public bool CanBePicked()
    {
        return !objectPicked && isObjective;
    }

    //Disables all collisions, physics, because it is going to be controled by the player.
    public void PickUp()
    {
        colliderPickUp.enabled = false;
        colliderObject.enabled = false;
        rigidbodyObject.useGravity = false;
        transform.localScale = startScale / 2.0f;
        if(gameController) gameController.ObjectPickedUp();
        objectPicked = true;
        rigidbodyObject.angularVelocity = Vector3.zero;
        rigidbodyObject.velocity = Vector3.zero;
    }

    //Reenables the collisions and physics.
    public void Drop()
    {
        colliderPickUp.enabled = true;
        colliderObject.enabled = true;
        rigidbodyObject.useGravity = true;
        rigidbodyObject.angularVelocity = Vector3.zero;
        rigidbodyObject.velocity = Vector3.zero;
        objectPicked = false;
    }

    //Sets this object as the current sale.
    public void ActivateSale()
    {
        isObjective = true;
        colliderPickUp.enabled = true;
    }

    //Deactivates this object if it was the sale.
    public void DeactivateSale()
    {
        isObjective = false;
        colliderPickUp.enabled = false;
        isReseting = true;
        if (objectPicked)
        {
            objectPicked = false;
            colliderObject.enabled = true;
            rigidbodyObject.useGravity = true;
        }
    }

    //Gets the money that you can save with this item.
    public int GetMoneySaved()
    {
        return originalPrice * discount / 100;
    }

    //Gets the original price of the item.
    public int GetOriginalMoney()
    {
        return originalPrice;
    }

    //Sets a discount to the item.
    public void SetDiscount(int discountApply)
    {
        discount = discountApply;
        Debug.Log("Discount applied:" + discount);
    }
}
