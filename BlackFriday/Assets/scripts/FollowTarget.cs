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
        if (target_follow_)
        {
            transform.position = Vector3.Lerp(transform.position, target_follow_.transform.position + offset_, Time.deltaTime * 5.0f);
            transform.LookAt(target_follow_.transform);
        }
	}
}
