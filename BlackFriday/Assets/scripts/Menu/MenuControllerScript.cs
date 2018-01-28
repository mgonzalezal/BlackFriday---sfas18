using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PlayerState
{
    kPlayerState_WaitingForOwner,
    kPlayerState_SelectingCharacter,
    kPlayerState_Ready,
}

/// <summary>
/// Menu controller script, handles the behaviour of the menu.
/// </summary>
public class MenuControllerScript : MonoBehaviour
{
    public GameObject[] charactersArray;
    public GameObject[] carsPlayersArray;
    public Player[] playersArray;
    public bool[] playersReady;
    public Canvas LoadingScreen;

    public GameObject playersNeeded;

    int numberOfPlayers;
    int numberPlayersEnteredSupermarket;

    public Text[] uiTextJoin;

    // Use this for initialization
    void Start()
    {
        numberOfPlayers = 0;
        numberPlayersEnteredSupermarket = 0;
        playersArray = new Player[] { new Player(), new Player(), new Player(), new Player() };
        playersReady = new bool[] { false, false, false, false };
        Time.timeScale = 1.0f;
        GameManagerScript.Instance().ClearPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        //In case we do not have a controller connected or available, allow the user to press escape key to exit the game.
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            GameObject.Find("LoadingScreen").GetComponent<Animator>().SetBool("Loading", true);
            StartCoroutine(LoadExitScene());
        }

        //When the player press start on a controller activate a car to bring the character to the game.
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetButtonUp("StartPlayer_" + (i+1)) && !playersReady[i])
            {
                carsPlayersArray[numberOfPlayers].GetComponent<CarBehaviour>().PlayHorn();
                carsPlayersArray[numberOfPlayers].GetComponent<CarBehaviour>().PlayerEnter(i, numberOfPlayers);
                playersReady[i] = true;
                playersArray[i].id = (i + 1).ToString();
                playersArray[i].character = charactersArray[numberOfPlayers];
                playersArray[i].isReady = true;
                uiTextJoin[numberOfPlayers].text = "JOINED!";
                numberOfPlayers++;
                GameManagerScript.Instance().RegisterPlayer(i, playersArray[i]);
            }
        }
    }

    //Spawns a player with the index in the player array and cars array.
    public void SpawnPlayer(int player, int car)
    {
        GameObject character = playersArray[player].character;
        GameObject characterSpawned = Instantiate(character, carsPlayersArray[car].transform.position, carsPlayersArray[car].transform.rotation);
        PlayerBehaviour playerScript = characterSpawned.GetComponent<PlayerBehaviour>();
        playerScript.ActivatePlayer(true);
        playerScript.SetPlayerNumber(playersArray[player].id);
    }

    //When a player enters the supermarket update the number of players needed to play the game, and if we meet the conditions to play the game
    //load the game.
    public void PlayerEnterSupermarket()
    {
        numberPlayersEnteredSupermarket++;
        if (numberPlayersEnteredSupermarket == numberOfPlayers && numberOfPlayers > 1)
        {
            LoadingScreen.GetComponent<Animator>().SetBool("Loading", true);
            Invoke("LoadGame", 1.0f);
        }

        if (numberOfPlayers > 1)
        {
            playersNeeded.GetComponent<TextMesh>().text = numberPlayersEnteredSupermarket + "/" + numberOfPlayers + " Players needed.";
        }
        else
        {
            playersNeeded.GetComponent<TextMesh>().text = numberPlayersEnteredSupermarket + "/2 Players needed.";
        }
    }

    void LoadGame()
    {
        StartCoroutine(LoadGameScene());
    }
    
    IEnumerator LoadGameScene()
    {
        // The Application loads the Scene in the background at the same time as the current Scene.
        //This is particularly good for creating loading screens. You could also load the Scene by build //number.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");

        //Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    IEnumerator LoadExitScene()
    {
        // The Application loads the Scene in the background at the same time as the current Scene.
        //This is particularly good for creating loading screens. You could also load the Scene by build //number.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Exit");

        //Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
