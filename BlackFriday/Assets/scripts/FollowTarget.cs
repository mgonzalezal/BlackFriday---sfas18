using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {

    public GameObject target_follow_;
    public Vector3 offset_;

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
	void Update () {
        
	}

    private void FixedUpdate()
    {
        if (target_follow_)
        {
            transform.position = Vector3.Lerp(transform.position, target_follow_.transform.position + offset_, Time.deltaTime * 5.0f);
            Quaternion rotation_new = Quaternion.LookRotation((target_follow_.transform.position) - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation_new, Time.deltaTime * 2.0f);
            //transform.LookAt(target_follow_.transform);
        }
    }

    private void LateUpdate()
    {
        
    }
}
