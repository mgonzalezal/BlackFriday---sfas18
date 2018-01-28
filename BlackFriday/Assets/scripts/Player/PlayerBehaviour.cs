using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Player Config")]
    public string playerNumber;
    public bool activated;
    [Space(2)]

    [Header("Movement Config")]
    public float speed;
    Vector3 direction;
    Vector3 lastDirection;
    [Space(2)]


    [Header("Pick Up Config")]
    public GameObject pickUpPoint;
    public Image pickUpProgressBar;
    public Canvas pickUpCanvas;
    GameObject pickUpInRange;
    PickUpObjectBehaviour pickUpInRangeScript;
    float resistanceObject;
    float resistanceObjectLeft;
    GameObject objectPicked;

    Animator animator;

    bool canPushAgain;
    float speedActual;
    bool isPushed;
    PushBehaviour pushChild;

    AudioSource punchSound;


    // Use this for initialization
    void Start()
    {
        isPushed = false;
        direction = new Vector3(0.0f, 0.0f, 0.0f);
        lastDirection = new Vector3(0.0f, 0.0f, 0.0f);
        pushChild = GetComponentInChildren<PushBehaviour>();

        pickUpCanvas.enabled = false;
        resistanceObjectLeft = 0.0f;

        animator = GetComponent<Animator>();
        canPushAgain = true;

        punchSound = GetComponent<AudioSource>();
    }

    Vector3 GetInput()
    {
        float x_axis = Input.GetAxis("horizontal_" + playerNumber);
        float z_axis = Input.GetAxis("vertical_" + playerNumber);
        Vector3 direction = new Vector3(x_axis, 0.0f, -z_axis);
        direction.Normalize();
        return direction;
    }

    // Update is called once per frame
    void Update()
    {
        //If the player is activated or not pushed just update normally.
        if (activated && !isPushed)
        {
            MovementUpdate();
            PushUpdate();
            PickUpUpdate();
        }
    }

    //Use fixed update to move the rigidbody, because in the normal update I was having step problems.
    private void FixedUpdate()
    {
       animator.SetFloat("Speed", direction.magnitude);
       GetComponent<Rigidbody>().MovePosition(transform.position + direction * speed * Time.deltaTime);
        //IF OBJECT PICKED UP, MAKE IT FOLLOW TO THE PICK UP POINT OF THE PLAYER
        if (objectPicked)
        {
            objectPicked.GetComponent<Rigidbody>().transform.position = Vector3.Lerp(objectPicked.transform.position, pickUpPoint.transform.position, Time.deltaTime * 10.0f);
            //objectPicked.GetComponent<Rigidbody>().transform.localRotation = Quaternion.Lerp(objectPicked.transform.localRotation, pickUpPoint.transform.localRotation, Time.deltaTime * 10.0f);
            if (!objectPicked.GetComponent<PickUpObjectBehaviour>().isObjective)
            {
                objectPicked = null;
            }
        }
    }

    void MovementUpdate()
    {
        if (!isPushed)
        {
            Vector3 last_location = transform.position;
            Vector3 actual_direction = GetInput();
            Vector3 new_direction = Vector3.Lerp(lastDirection, actual_direction, Time.deltaTime * 20.0f);
            Vector3 new_position = last_location + new_direction;

            if (actual_direction.magnitude > 0.0f)
            {
                lastDirection = new_direction;
                direction = new_direction.normalized;
            }
            else
            {
                direction = Vector3.zero;
            }

            if (new_position != last_location)
            {
                Quaternion rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new_position - last_location), Time.deltaTime * 10.0f);
                GetComponent<Rigidbody>().MoveRotation(rotation);
            }
        }
    }

    void PushUpdate()
    {
        if (Input.GetButtonDown("Push_" + playerNumber) && canPushAgain && !objectPicked)
        {
            animator.SetBool("Punch", true);
            pushChild.Push();
            canPushAgain = false;
            punchSound.Play();
        }
    }

    //PICK UP FUNCTIONS

    void PickUpUpdate()
    {
        if (pickUpInRange)
        {
            pickUpCanvas.enabled = true;
            if (pickUpInRangeScript.CanBePicked())
            {
                //PICK UP PROGRESS ON BUTTON DOWN
                bool pickUpButtonPressed = Input.GetButtonDown("PickUp_" + playerNumber);
                if (pickUpButtonPressed)
                {
                    resistanceObjectLeft -= 0.1f;
                    if (resistanceObjectLeft <= 0.0f)
                    {
                        pickUpInRangeScript.PickUp();
                        objectPicked = pickUpInRange;
                    }
                }

                UpdateProgressBar();
            }
            else
            {
                pickUpInRange = null;
                resistanceObjectLeft = 0.0f;
                UpdateProgressBar();
                pickUpCanvas.enabled = false;
            }
        }
    }

    //Function to unlink the object when we are goint to pay for it.
    public GameObject CheckoutItem()
    {
        if (objectPicked)
        {
            GameObject tmp = objectPicked;
            pickUpInRangeScript.CheckOut();
            ClearReferencesObject();
            return tmp;
        }

        return null;
    }

    void ClearReferencesObject()
    {
        objectPicked = null;
        pickUpInRange = null;
        pickUpInRangeScript = null;
        resistanceObjectLeft = 0.0f;
        pickUpCanvas.enabled = false;
    }

    //Drops the object when we punch this player.
    public void DropObject()
    {
        if (objectPicked)
        {
            Debug.Log("Push player drop object");
            pickUpInRangeScript.Drop();
            ClearReferencesObject();
        }
    }

    //Function to update the progress bar to pick up an item.
    void UpdateProgressBar()
    {
        if (resistanceObject > 0.0f)
        {
            Vector3 scale = pickUpProgressBar.rectTransform.localScale;
            float new_scale = 1.0f - (resistanceObjectLeft / resistanceObject);
            scale.x = Mathf.Lerp(scale.x, new_scale, Time.deltaTime * 10.0f);
            pickUpProgressBar.rectTransform.localScale = scale;
        }
    }

    //PICK UP FUNCTIONS

    public void PushPlayer(Vector3 push_direction)
    {
        direction = Vector3.zero;
        direction.Normalize();
        isPushed = true;
        DropObject();
        animator.SetBool("IsHit", true);
        EndPunch();
        pickUpInRange = null;
        resistanceObjectLeft = 0.0f;
        UpdateProgressBar();
        pickUpCanvas.enabled = false;
    }

    public void GetUp()
    {
        animator.SetBool("IsHit", false);
        isPushed = false;
    }

    public void EndPunch()
    {
        animator.SetBool("Punch", false);
        canPushAgain = true;
    }

    public string GetPlayerNumber()
    {
        return playerNumber;
    }

    public void SetPlayerNumber(string number)
    {
        playerNumber = number;
    }

    public void ActivatePlayer(bool activate)
    {
        activated = activate;
    }

    public void ResetMovement()
    {
        direction = Vector3.zero;
    }

    public void SetAsWinner(bool is_winner)
    {
        animator.SetBool("IsWinner", is_winner);
        animator.SetBool("IsGameDone", true);
    }

    public void AsignateCamera(Camera camera)
    {
        pickUpProgressBar.transform.GetComponentInParent<PickUpBillboard>().cameraUsing = camera;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickUp") && !objectPicked)
        {
            pickUpInRange = other.gameObject;
            pickUpInRangeScript = pickUpInRange.GetComponent<PickUpObjectBehaviour>();
            resistanceObject = pickUpInRangeScript.getResitanceToPick();
            resistanceObjectLeft = resistanceObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PickUp") && !objectPicked && pickUpInRange == other.gameObject)
        {
            pickUpInRange = null;
            resistanceObjectLeft = 0.0f;
            pickUpCanvas.enabled = false;
            Debug.Log("TRIGGER EXIT" + other.gameObject.name);
        }
    }
}
