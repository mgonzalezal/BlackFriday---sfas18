using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Game canvas controller it is just to control the canvas of the game.
/// </summary>
public class GameCanvasController : MonoBehaviour {

    Animator animator;
    public Image saleImage;
    public Image counterImage;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        DeactivateSale();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //Allows the animatior load the predifined layout for the number of players specified.
    public void SetNumberPlayers(int numberPlayers)
    {
        animator.SetInteger("NumberPlayers", numberPlayers);
    }

    //Activates a sale
    public void ActivateSale()
    {
        saleImage.enabled = true;
        counterImage.enabled = true;
    }

    //Deactivates a sale
    public void DeactivateSale()
    {
        saleImage.enabled = false;
        counterImage.enabled = false;
    }
}
