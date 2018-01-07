using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObjectBehaviour : MonoBehaviour {

    public float resitance_to_pick_ = 0.0f;
    public bool is_picked_up_ = false;
    public bool is_being_checkout = false;
    SphereCollider collider_pick;
    BoxCollider collider_object;
    Rigidbody rigid_body;
    public bool is_objective_ = true;

    Vector3 start_scale;

	// Use this for initialization
	void Start () {
        is_picked_up_ = false;
        collider_pick = GetComponent<SphereCollider>();
        collider_object = GetComponent<BoxCollider>();
        rigid_body = GetComponent<Rigidbody>();
        collider_pick.enabled = is_objective_;
        start_scale = transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {
        if (!is_picked_up_ && !is_being_checkout)
        {
            transform.localScale = Vector3.Lerp(transform.lossyScale, start_scale, Time.deltaTime * 5.0f);
        }
	}

    public float getResitanceToPick()
    {
        return resitance_to_pick_;
    }

    public void CheckOut()
    {
        is_being_checkout = true;
        collider_pick = GetComponent<SphereCollider>();
        collider_pick.enabled = false;
        collider_object.enabled = false;
        rigid_body.useGravity = false;
        rigid_body.velocity = Vector3.zero;
        rigid_body.angularVelocity = Vector3.zero;
    }

    public void PickUp()
    {
        is_picked_up_ = true;
        collider_pick = GetComponent<SphereCollider>();
        collider_pick.enabled = false;
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        collider_object.enabled = false;
        rigid_body.useGravity = false;
        rigid_body.velocity = Vector3.zero;
        rigid_body.angularVelocity = Vector3.zero;
    }

    public void Drop()
    {
        is_picked_up_ = false;
        collider_pick = GetComponent<SphereCollider>();
        collider_pick.enabled = true;
        collider_object.enabled = true;
        rigid_body.useGravity = true;
        rigid_body.velocity = Vector3.zero;
        rigid_body.angularVelocity = Vector3.zero;
    }

    public Vector3 GetStartScale()
    {
        return start_scale;
    }
}
