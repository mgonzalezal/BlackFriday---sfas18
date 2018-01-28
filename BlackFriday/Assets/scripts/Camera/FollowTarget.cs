using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script done to make the camera follow a target.
/// </summary>
public class FollowTarget : MonoBehaviour
{

    public GameObject target_follow_;
    public Vector3 position_;
    public Vector3 offset_;
    public float speedCamera;

    bool manualFollow;

    // Use this for initialization
    void Start()
    {
        if (target_follow_)
        {
            transform.rotation = target_follow_.transform.rotation;
            transform.position = target_follow_.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        //If we have set a target to follow and it is not a defined position, update like this
        if (target_follow_ && !manualFollow)
        {
            transform.position = Vector3.Lerp(transform.position, target_follow_.transform.position + offset_, Time.deltaTime * speedCamera);
            Quaternion rotation_new = Quaternion.LookRotation((target_follow_.transform.position) - (target_follow_.transform.position + offset_));
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation_new, Time.deltaTime * speedCamera);
        }

        //If we have set a position to focus, update like this
        if (manualFollow)
        {
            transform.position = Vector3.Lerp(transform.position, offset_, Time.deltaTime * speedCamera);
            Quaternion rotation_new = Quaternion.LookRotation((position_) - offset_);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation_new, Time.deltaTime * speedCamera);
        }
    }

    //Set the target to follow
    public void SetTarget(GameObject target)
    {
        target_follow_ = target;
        manualFollow = false;
    }

    //Set a position to look at.
    public void SetPositionLook(Vector3 positionLook)
    {
        position_ = positionLook;
        manualFollow = true;
    }

    //Set the offset of the camera from the view point.
    public void SetOffset(Vector3 offset)
    {
        offset_ = offset;
    }

    //Set the speed camera, used by the lerp functions.
    public void SetSpeedCamera(float speed)
    {
        speedCamera = speed;
    }

    //Function to configure the cameras depending of the number of players in the game.
    public void ConfigureCamera(int numberPlayers, int index)
    {
        if (numberPlayers == 2)
        {
            GetComponent<Camera>().rect = new Rect(new Vector2(index * 0.5f, 0.0f), new Vector2(0.5f, 1.0f));
        }else
        {
            if (index < 2)
            {
                GetComponent<Camera>().rect = new Rect(new Vector2((index % 2) * 0.5f, 0.5f), new Vector2(0.5f, 1.0f));
            }else
            {
                GetComponent<Camera>().rect = new Rect(new Vector2((index % 2) * 0.5f, 0.0f), new Vector2(0.5f, 0.5f));
            }
        }
    }
}
