using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public string player_number;
    public float speed;

    bool is_pushed_;
    
    Vector3 direction_;
    public float time_pushed_;
    float time_pushed_left_;

    public Vector3 last_direction;

    PushBehaviour push_child_;
    PlayerPickUpScript pick_up_;

    // Use this for initialization
    void Start()
    {
        is_pushed_ = false;
        direction_ = new Vector3(0.0f, 0.0f, 0.0f);
        last_direction = new Vector3(0.0f, 0.0f, 0.0f);
        push_child_ = GetComponentInChildren<PushBehaviour>();
        pick_up_ = GetComponent<PlayerPickUpScript>();
        push_child_.DisablePush();
    }

    Vector3 getInput()
    {
        float x_axis = Input.GetAxis("horizontal_" + player_number);
        float z_axis = Input.GetAxis("vertical_" + player_number);
        Vector3 direction = new Vector3(x_axis, 0.0f, -z_axis);
        direction.Normalize();
        return direction;
    }

    // Update is called once per frame
    void Update()
    {
        if (is_pushed_)
        {
            time_pushed_left_ -= Time.deltaTime;
            if (time_pushed_left_ <= 0.0f)
            {
                is_pushed_ = false;
            }
        }
        else
        {
            Vector3 last_location = transform.position;
            Vector3 actual_direction = getInput();
            Vector3 new_direction = Vector3.Lerp(last_direction, actual_direction, Time.deltaTime * 20.0f);
            Vector3 new_position = last_location + new_direction;

            if (actual_direction.magnitude > 0.0f)
            {
                last_direction = new_direction;
                direction_ = new_direction.normalized;
            }
            else
            {
                direction_ = Vector3.zero;
            }
            if (new_position != last_location)
            {
                Quaternion rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new_position - last_location), Time.deltaTime * 10.0f);
                GetComponent<Rigidbody>().MoveRotation(rotation);
            }
        }

        if (Input.GetButtonDown("Push_" + player_number))
        {
            push_child_.EnablePush();
        }
        else
        {
            push_child_.DisablePush();
        }
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().MovePosition(transform.position + direction_ * speed * Time.deltaTime);
    }

    public void PushPlayer(Vector3 push_direction)
    {
        direction_ = push_direction;
        direction_.Normalize();
        time_pushed_left_ = time_pushed_;
        is_pushed_ = true;
        pick_up_.DropObject();
    }

    public string getPlayerNumber()
    {
        return player_number;
    }
}
