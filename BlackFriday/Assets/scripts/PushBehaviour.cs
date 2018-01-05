using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBehaviour : MonoBehaviour {

    BoxCollider box;
    bool is_pushing_;

    GameObject player;

    // Use this for initialization
    void Start () {
        box = GetComponent<BoxCollider>();
        box.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void EnablePush()
    {
        box.enabled = true;
    }

    public void DisablePush()
    {
        box.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 pos = (other.gameObject.transform.position - gameObject.transform.parent.position).normalized;
            other.gameObject.GetComponent<PlayerBehaviour>().PushPlayer(pos);
            box.enabled = false;
        }
    }
}
