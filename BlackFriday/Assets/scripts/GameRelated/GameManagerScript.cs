using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public string id;
    public GameObject character;
    public bool isReady;
};

/// <summary>
/// Script to share information about the players between scenes.
/// </summary>
public class GameManagerScript : MonoBehaviour {

    public Player[] players;
    int numberPlayers;

    private static GameManagerScript instance;

    public static GameManagerScript Instance()
    {
        if (instance)
        {
            return instance;
        }else
        {
            GameManagerScript script = new GameObject("GameManager").AddComponent<GameManagerScript>();
            script.Init();
            return instance = script;
        }
    }

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ClearPlayers()
    {
        numberPlayers = 0;
        players = new Player[] { new Player(), new Player(), new Player(), new Player() };
    }

    void Init()
    {
        numberPlayers = 0;
        players = new Player[] { new Player(), new Player(), new Player(), new Player() };
        DontDestroyOnLoad(this);
    }

    public void RegisterPlayer(int index, Player player)
    {
        players[index] = player;
        numberPlayers++;
    }

    public Player[] GetPlayers()
    {
        return players;
    }

    public int GetNumberPlayers()
    {
        return numberPlayers;
    }
}
