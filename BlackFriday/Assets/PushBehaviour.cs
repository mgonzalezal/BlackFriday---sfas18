using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBehaviour : MonoBehaviour {

    BoxCollider box;

    // Use this for initialization
    void Start () {
        box = GetComponent<BoxCollider>();

	}
	
	// Update is called once per frame
	void Update () {

    }

    private void OnTriggerStay(Collider other)
    {
        
    }
}
