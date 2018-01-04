using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public string player_number;
    public float speed;

    bool is_pushed_;
    Vector3 pushed_direction_;
    public float time_pushed_;
    float time_pushed_left_;

    public Vector3 last_direction;

    // Use this for initialization
    void Start()
    {
        is_pushed_ = false;
        pushed_direction_ = new Vector3(0.0f, 0.0f, 0.0f);
        last_direction = new Vector3(0.0f, 0.0f, 0.0f);       
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
            GetComponent<Rigidbody>().MovePosition(transform.position + pushed_direction_ * speed * Time.deltaTime);
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
            Vector3 new_direction = Vector3.Lerp(last_direction, actual_direction, Time.deltaTime * 5.0f);

            if (actual_direction.magnitude > 0.0f)
            {
                last_direction = new_direction;
                transform.position += new_direction.normalized * speed * Time.deltaTime;
            }
            if (transform.position != last_location)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.position - last_location), Time.deltaTime * 10.0f);
            }
            //GetComponent<Rigidbody>().MovePosition(transform.position + getInput() * speed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Push_" + player_number))
        {
            Ray ray = new Ray(transform.position, last_direction.normalized);
            Debug.DrawRay(transform.position, last_direction * 10.0f, Color.red);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10.0f))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    hit.collider.gameObject.GetComponent<PlayerBehaviour>().PushPlayer(last_direction);
                }
            }
        }
    }

    public void PushPlayer(Vector3 push_direction)
    {
        pushed_direction_ = push_direction;
        pushed_direction_.Normalize();
        time_pushed_left_ = time_pushed_;
        is_pushed_ = true;
    }

    public string getPlayerNumber()
    {
        return player_number;
    }
}
