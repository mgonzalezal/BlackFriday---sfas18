using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script to update the money saved by each player.
/// </summary>
public class UpdateMoneySaved : MonoBehaviour {

    public int PlayerToUpdate;
    Text moneySaved;
    GameController gameController;
    int moneyShowing;
    int moneySavedReal;

    float timeIncrementLeft;
    

	// Use this for initialization
	void Start () {
		
	}

    void Awake()
    {
        moneySaved = GetComponentInChildren<Text>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }
	
	// Update is called once per frame
	void Update () {
        timeIncrementLeft -= Time.deltaTime;
        if (timeIncrementLeft <= 0.0f)
        {
            moneySavedReal = gameController.GetMoneySaved(PlayerToUpdate);
            if (moneySavedReal > moneyShowing)
            {
                moneyShowing += 10;
                moneySaved.color = Color.green;
                if (moneyShowing > moneySavedReal)
                {
                    moneyShowing = moneySavedReal;
                }
            }
            else
            {
                moneySaved.color = Color.white;
            }
            timeIncrementLeft = 0.1f;
        }
        
        moneySaved.text = moneyShowing.ToString();
    }
}
