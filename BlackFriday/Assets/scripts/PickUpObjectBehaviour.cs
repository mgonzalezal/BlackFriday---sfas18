using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObjectBehaviour : MonoBehaviour {

    public float resitance_to_pick_ = 0.0f;
    public bool is_picked_up_ = false;
    SphereCollider collider_pick;
    BoxCollider collider_object;
    Rigidbody rigid_body;
    public bool is_objective_ = true;

	// Use this for initialization
	void Start () {
        is_picked_up_ = false;
        collider_pick = GetComponent<SphereCollider>();
        collider_object = GetComponent<BoxCollider>();
        rigid_body = GetComponent<Rigidbody>();
        collider_pick.enabled = is_objective_;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public float getResitanceToPick()
    {
        return resitance_to_pick_;
    }
    public void PickUp()
    {
        is_picked_up_ = true;
        collider_pick = GetComponent<SphereCollider>();
        collider_pick.enabled = false;
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        collider_object.enabled = false;
        rigid_body.useGravity = false;
    }
}
