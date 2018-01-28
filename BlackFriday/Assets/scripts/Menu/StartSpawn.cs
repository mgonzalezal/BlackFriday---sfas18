using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSpawn : MonoBehaviour {

    public GameObject StartButton;
    public GameObject StartToSelect;
    public GameObject Ready;
    Animator animator;

    // Use this for initialization
    void Start () {
        
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void ActivateStartToSelect()
    {
        animator.SetBool("IsSelectingCharacter", true);
    }

    public void ActivateReady()
    {
        animator.SetBool("IsPlayerReady", true);
    }
}
