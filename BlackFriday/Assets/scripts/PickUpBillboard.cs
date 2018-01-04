using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpBillboard : MonoBehaviour {

    public Camera camera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(camera.transform);
    }

    private void OnPreCull()
    {

    }
}
