using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPickUpScript : MonoBehaviour {

    GameObject oject_pick_;
    PickUpObjectBehaviour object_script_;
    PlayerBehaviour player;

    public GameObject pick_up_point_;
    public Image pick_up_progress_;
    public Canvas pick_up_parent_;

    public float resitance_left_ = 0.0f;

    public GameObject object_picked_;

	// Use this for initialization
	void Start () {
        player = GetComponent<PlayerBehaviour>();
        pick_up_parent_.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (oject_pick_)
        {
            pick_up_parent_.enabled = true;
            if (!object_script_.is_picked_up_)
            {
                bool pick_up_button = Input.GetButtonDown("PickUp_" + player.getPlayerNumber());
                if (pick_up_button)
                {
                    resitance_left_ -= 0.1f;
                    if (resitance_left_ <= 0.0f)
                    {
                        object_script_.PickUp();
                        object_picked_ = oject_pick_;
                    }
                }
                Vector3 scale = pick_up_progress_.rectTransform.localScale;
                float new_scale = 1.0f - (resitance_left_ / object_script_.getResitanceToPick());
                scale.x = Mathf.Lerp(scale.x, new_scale, Time.deltaTime * 10.0f);
                pick_up_progress_.rectTransform.localScale = scale;
            }
            else
            {
                oject_pick_ = null;
                pick_up_parent_.enabled = false;
            }
        }

        if (object_picked_)
        {
            object_picked_.transform.position = Vector3.Lerp(object_picked_.transform.position, pick_up_point_.transform.position, Time.deltaTime * 10.0f);
            object_picked_.transform.localRotation = Quaternion.Lerp(object_picked_.transform.localRotation, pick_up_point_.transform.localRotation, Time.deltaTime * 10.0f);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickUp"))
        {
            oject_pick_ = other.gameObject;
            object_script_ = oject_pick_.GetComponent<PickUpObjectBehaviour>();
            resitance_left_ = object_script_.getResitanceToPick();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PickUp"))
        {
            oject_pick_ = null;
            object_script_ = null;
            resitance_left_ = 0.0f;
            pick_up_parent_.enabled = false;
        }
    }
}
