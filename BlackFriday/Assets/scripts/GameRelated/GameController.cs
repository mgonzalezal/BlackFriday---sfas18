using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum GameState
{
    kGameState_PreparingGame,
    kGameState_PresentingGame,
    kGameState_TimeFreeRoaming,
    kGameState_PresentNextItem,
    kGameState_TimeGetItem,
    kGameState_CheckingItem,
    kGameState_PresentResults,
    kGameState_EndGame
}

public enum PresentingGame
{
    kPresentingGame_ShowPlayers,
    kPresentingGame_ShowRegister,
    kPresentingGame_ShowWholeSupermarket,
    kPresentingGame_ShowAllPlayers
}

/// <summary>
/// The game controller is the main class that holds the main game loop
/// </summary>
public class GameController : MonoBehaviour
{

    public class PlayerInGame
    {
        public string id;
        public int moneySaved;
        public GameObject player;
    }

    List<PlayerInGame> playersList;


    [Header("Game Config")]
    public GameObject[] spawnLocations;
    public GameObject cameraPrefab;
    public GameObject cameraGlobal;
    public AudioControler audioControler;


    [Header("UI Game Config")]
    public Text timeLeftItemText;
    public GameObject salesCurtain;
    public GameObject clock;
    public GameCanvasController canvasController;
    public Canvas canvasGameOver;
    public GameObject moneySpawner;
    public Text priceOriginalSale;
    public Text discountSale;
    public Text newPriceSale;
    public GameObject salesGameobjectUi;
    public Canvas loadingScreen;
    public Canvas pauseCanvas;

    int playerPresentingIndex;
    PlayerInGame winner;

    int numberPlayers;
    GameState gameState;
    PresentingGame presentingGame;
    bool checkingItemDone;
    bool isPlaying;

    public List<GameObject> objectsToSell;

    GameObject currentSale;

    [Header("Present Game Config - Present players")]
    public GameObject allPlayersLocation;
    public float timePresentEachPlayer;
    float timePresentEachPlayerLeft;

    [Header("Present Game Config - Present check out location")]
    public GameObject checkOutLocation;
    public float timePresentCheckout;
    float timePresentCheckoutLeft;

    [Header("Present Game Config - Aerial view")]
    public GameObject allSupermarketView;
    public float timeAllSupermarketView;
    float timeAllSupermarketViewLeft;

    [Header("Game config - Time of the game")]
    public float totalTimeOfGame;
    float totalTimeOfGameLeft;

    [Header("Game config - Time present item")]
    public float timeToPresentItem;
    float timeToPresentItemLeft;

    [Header("Game config - Free roaming")]
    public float timeFreeRoam;
    float timeFreeRoamLeft;

    bool gamePaused;

    // Use this for initialization
    void Start()
    {
        gamePaused = false;
        Time.timeScale = 1.0f;
    }

    private void Awake()
    {
        isPlaying = false;
        gameState = GameState.kGameState_PreparingGame;
        playersList = new List<PlayerInGame>();
        Player[] playerArray = GameManagerScript.Instance().GetPlayers();
        numberPlayers = GameManagerScript.Instance().GetNumberPlayers();
        int i = 0;

        //CONFIGURE AND SPAWN THE PLAYERS WITH RESPECTIVE CAMERAS
        foreach (Player player in playerArray)
        {
            if (player.isReady)
            {
                playersList.Add(new PlayerInGame());
                PlayerInGame playerNew = playersList[playersList.Count - 1];
                playerNew.id = player.id;
                playerNew.moneySaved = 0;
                GameObject cameraPlayer = Instantiate(cameraPrefab, spawnLocations[i].transform.position, spawnLocations[i].transform.rotation);

                //PLAYER CONFIGURATION
                GameObject playerInst = Instantiate(player.character, spawnLocations[i].transform.position, spawnLocations[i].transform.rotation);
                playerInst.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                PlayerBehaviour behaviour = playerInst.GetComponent<PlayerBehaviour>();
                behaviour.SetPlayerNumber(player.id);
                behaviour.ActivatePlayer(false);
                behaviour.AsignateCamera(cameraPlayer.GetComponent<Camera>());
                behaviour.speed = 10.0f;

                //CAMERA CONFIGURATION
                cameraPlayer.GetComponent<FollowTarget>().SetTarget(playerInst);
                cameraPlayer.GetComponent<FollowTarget>().ConfigureCamera(numberPlayers, i);
                cameraPlayer.GetComponent<FollowTarget>().SetOffset(new Vector3(0.0f, 15.0f, -3.0f));
                cameraPlayer.GetComponent<FollowTarget>().SetSpeedCamera(7.0f);

                playerNew.player = playerInst;

                i++;
            }
        }

        //SEARCH FOR ALL ITEMS IN THE GAME
        objectsToSell.AddRange(GameObject.FindGameObjectsWithTag("PickUp"));


        //CHECK THE GAME AS READY TO PRESENT
        gameState = GameState.kGameState_PresentingGame;
        presentingGame = PresentingGame.kPresentingGame_ShowPlayers;

        playerPresentingIndex = -1;
        timePresentEachPlayerLeft = timePresentEachPlayer;
        cameraGlobal.GetComponent<FollowTarget>().SetSpeedCamera(4.0f);


        audioControler.PlayWithoutFade(2);
    }

    // Update is called once per frame
    void Update()
    {
        //Pause the game when we press start on the controller.
        if (Input.GetButtonUp("PauseGame"))
        {
            if (!gamePaused)
            {
                GameObject continueButton = GameObject.Find("ContinueButton");
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(continueButton);
                Time.timeScale = 0.0f;
                gamePaused = true;
                pauseCanvas.GetComponent<Animator>().SetBool("Pause", true);
            }
        }
        //If there is a game running run the timer to finish the game, and update the timer on the hud.
        if (isPlaying)
        {
            totalTimeOfGameLeft -= Time.deltaTime;
            if (totalTimeOfGameLeft<=0.0f)
            {
                totalTimeOfGameLeft = 0.0f;
                gameState = GameState.kGameState_PresentResults;
            }
            UpdateTimeText();
        }

        //Switch with the game state.
        switch (gameState)
        {
            //This is just an init case, once the awake function is done changes to the next case.
            case GameState.kGameState_PreparingGame:
                {
                    break;
                }
            //Presenting game is to do some camera transitions in the scene to present the game.
            case GameState.kGameState_PresentingGame:
                {
                    PresentingGameState();
                    break;
                }
            //Time that nothing happens allowing the players to explore the map.
            case GameState.kGameState_TimeFreeRoaming:
                {
                    TimeFreeRoamingState();
                    break;
                }
            //Time that the item is presented to the players.
            case GameState.kGameState_PresentNextItem:
                {
                    PresentNextItemState();
                    break;
                }
            //Function waiting for the player to check out an item.
            case GameState.kGameState_TimeGetItem:
                {
                    TimeGetItemState();
                    break;
                }
            //Waiting for the object to be checked out.
            case GameState.kGameState_CheckingItem:
                {
                    CheckingItemState();
                    break;
                }
            //When the game is done, set up the game over screen.
            case GameState.kGameState_PresentResults:
                {
                    PresentResultsState();
                    break;
                }
            //Gameover screen waiting for a button to be pressed.
            case GameState.kGameState_EndGame:
                {
                    break;
                }
        }
    }

    void PresentingGameState()
    {
        switch (presentingGame)
        {
            //Show each player in the game focusing them.
            case PresentingGame.kPresentingGame_ShowPlayers:
                {
                    timePresentEachPlayerLeft -= Time.deltaTime;
                    if (timePresentEachPlayerLeft <= 0.0f)
                    {
                        playerPresentingIndex++;
                        if (playerPresentingIndex >= numberPlayers)
                        {
                            presentingGame = PresentingGame.kPresentingGame_ShowRegister;
                            timePresentCheckoutLeft = timePresentCheckout;
                            cameraGlobal.GetComponent<FollowTarget>().SetTarget(checkOutLocation);
                            cameraGlobal.GetComponent<FollowTarget>().SetOffset(new Vector3(0.0f, 7.0f, -7.0f));
                        }
                        else
                        {
                            cameraGlobal.GetComponent<FollowTarget>().SetTarget(spawnLocations[playerPresentingIndex]);
                            cameraGlobal.GetComponent<FollowTarget>().SetOffset(new Vector3(0.0f, 5.0f, -5.0f));
                            timePresentEachPlayerLeft = timePresentEachPlayer;
                        }
                    }
                    break;
                }
            //Show the position of the checkout focusing it.
            case PresentingGame.kPresentingGame_ShowRegister:
                {
                    timePresentCheckoutLeft -= Time.deltaTime;
                    if (timePresentCheckoutLeft <= 0.0f)
                    {
                        presentingGame = PresentingGame.kPresentingGame_ShowWholeSupermarket;
                        timeAllSupermarketViewLeft = timeAllSupermarketView;
                        cameraGlobal.GetComponent<FollowTarget>().SetPositionLook(allSupermarketView.transform.position);
                        cameraGlobal.GetComponent<FollowTarget>().SetOffset((allSupermarketView.transform.position) - allSupermarketView.transform.forward * 2.0f);
                    }
                    break;
                }
            //Aerial view of the supermarket.
            case PresentingGame.kPresentingGame_ShowWholeSupermarket:
                {
                    timeAllSupermarketViewLeft -= Time.deltaTime;
                    if (timeAllSupermarketViewLeft <= 0.0f)
                    {
                        presentingGame = PresentingGame.kPresentingGame_ShowAllPlayers;
                        audioControler.PlayWithFade(3.0f, 0);
                    }
                    break;
                }
            //Transition state to the game, that sets the hud and activates the players to allow the control.
            case PresentingGame.kPresentingGame_ShowAllPlayers:
                {
                    canvasController.SetNumberPlayers(numberPlayers);
                    totalTimeOfGameLeft = totalTimeOfGame;
                    timeFreeRoamLeft = timeFreeRoam;
                    gameState = GameState.kGameState_TimeFreeRoaming;
                    clock.GetComponent<Animator>().SetBool("IsShowingClock", true);
                    isPlaying = true;
                    foreach(PlayerInGame player in playersList)
                    {
                        player.player.GetComponent<PlayerBehaviour>().ActivatePlayer(true);
                    }
                    break;
                }
        }
    }

    void TimeFreeRoamingState()
    {
        timeFreeRoamLeft -= Time.deltaTime;
        if (timeFreeRoamLeft <= 0.0f)
        {
            gameState = GameState.kGameState_PresentNextItem;
            salesCurtain.GetComponent<Animator>().SetBool("IsShowingItem", true);
            timeToPresentItemLeft = timeToPresentItem;
        }
    }

    void PresentNextItemState()
    {
        timeToPresentItemLeft -= Time.deltaTime;
        if (timeToPresentItemLeft <= 0.0f)
        {
            gameState = GameState.kGameState_TimeGetItem;
            GetNextItem();
            salesCurtain.GetComponent<Animator>().SetBool("IsShowingItem", false);
            salesGameobjectUi.GetComponent<Animator>().SetBool("IsDealShowing", true);
        }
            
    }

    void TimeGetItemState()
    {
        //JUST WAIT FOR THE OBJECT TO BE CHECKOUT
        //Next code is just for debug
        /*if (Input.GetKeyUp(KeyCode.W))
        {
            gameState = GameState.kGameState_PresentNextItem;
            salesCurtain.GetComponent<Animator>().SetBool("IsShowingItem", true);
            salesGameobjectUi.GetComponent<Animator>().SetBool("IsDealShowing", false);
            timeToPresentItemLeft = timeToPresentItem;
        }*/
    }

    void CheckingItemState()
    {
        //When the item is checked present next item.
        if (checkingItemDone)
        {
            gameState = GameState.kGameState_PresentNextItem;
            salesCurtain.GetComponent<Animator>().SetBool("IsShowingItem", true);
            salesGameobjectUi.GetComponent<Animator>().SetBool("IsDealShowing", false);
            timeToPresentItemLeft = timeToPresentItem;
        }
    }

    void PresentResultsState()
    {
        canvasGameOver.GetComponent<Animator>().SetBool("IsGameOver", true);
        winner = playersList[0];
        //Deactivate the players and search for the winner.
        foreach (PlayerInGame player in playersList)
        {
            player.player.GetComponent<PlayerBehaviour>().ActivatePlayer(false);
            player.player.GetComponent<PlayerBehaviour>().ResetMovement();
            if (winner.moneySaved < player.moneySaved)
            {
                winner = player;
            }
        }

        gameState = GameState.kGameState_EndGame;
        Invoke("FocusWinner", 2.0f);
        isPlaying = false;
        if (currentSale)
        {
            currentSale.GetComponent<PickUpObjectBehaviour>().DeactivateSale();
        }
    }

    void FocusWinner()
    {
        //Animate the players to the victory or defeat pose.
        foreach (PlayerInGame player in playersList)
        {
            if (player == winner)
            {
                player.player.GetComponent<PlayerBehaviour>().SetAsWinner(true);
            }
            else
            {
                player.player.GetComponent<PlayerBehaviour>().SetAsWinner(false);
            }
        }

        //Focus the camera to the winner.
        cameraGlobal.GetComponent<FollowTarget>().SetTarget(winner.player);
        cameraGlobal.GetComponent<FollowTarget>().SetOffset(new Vector3(0.0f, 10.0f, -10.0f));

        //Play some coin particles at the head of the player.
        Instantiate(moneySpawner, winner.player.transform.position + new Vector3(0.0f, 10.0f, 0.0f), Quaternion.identity);
        winner.player.transform.LookAt(winner.player.transform.position + new Vector3(0.0f, 0.0f, -10.0f));

        //Play victory song.
        audioControler.PlayWithFade(1.0f, 1);

        //Focus the Play again button so we can control it with the controller.
        GameObject playAgainButton = GameObject.Find("PlayAgain");
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(playAgainButton);
    }

    void GetNextItem()
    {
        int numberItems = objectsToSell.Count;
        if (numberItems > 0)
        {
            //Get a random discount in the item.
            int discountApply = RandomDiscount();

            //Get a random item from the list.
            int index = Random.Range(0, numberItems);
            GameObject nextSale = objectsToSell[index];
            objectsToSell.RemoveAt(index);

            //Activate the sale and focus the camera on the object.
            nextSale.GetComponent<PickUpObjectBehaviour>().SetDiscount(discountApply);
            nextSale.GetComponent<PickUpObjectBehaviour>().ActivateSale();
            Vector3 position = nextSale.GetComponent<PickUpObjectBehaviour>().getPositionLook();
            Vector3 offset = nextSale.GetComponent<PickUpObjectBehaviour>().getViewOffset();
            cameraGlobal.GetComponent<FollowTarget>().SetPositionLook(position);
            cameraGlobal.GetComponent<FollowTarget>().SetOffset(offset);
            cameraGlobal.GetComponent<FollowTarget>().SetSpeedCamera(100.0f);
            currentSale = nextSale;

            //Set up the hud to display the info of the object.
            int originalPrice = nextSale.GetComponent<PickUpObjectBehaviour>().GetOriginalMoney();
            priceOriginalSale.text = originalPrice.ToString();
            discountSale.text = discountApply.ToString() + "%";
            int moneySaved = nextSale.GetComponent<PickUpObjectBehaviour>().GetMoneySaved();
            newPriceSale.text = (originalPrice - moneySaved).ToString();
        }
        else
        {
            gameState = GameState.kGameState_PresentResults;
        }
    }

    //Set up the camera when the object is picked to an "aerial" view of the object (like a minimap view).
    public void ObjectPickedUp()
    {
        cameraGlobal.GetComponent<FollowTarget>().SetTarget(currentSale);
        cameraGlobal.GetComponent<FollowTarget>().SetOffset(new Vector3(0.0f, 20.0f, 0.0f));
    }

    //Function called when we are going to check out an item.
    public void CheckoutItem()
    {
        checkingItemDone = false;
        gameState = GameState.kGameState_CheckingItem;
        cameraGlobal.GetComponent<FollowTarget>().SetOffset(new Vector3(0.0f, 5.0f, -5.0f));
    }

    //When the item is checked out call this function to jump to the get next item state.
    public void DoneCheckout()
    {
        checkingItemDone = true;
    }

    //Function to update the hud text of the time.
    void UpdateTimeText()
    {
        int minutes = Mathf.FloorToInt(totalTimeOfGameLeft / 60);
        int seconds = Mathf.FloorToInt(totalTimeOfGameLeft % 60);
        timeLeftItemText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }

    //Add money to the player that has paid an item.
    public void AddMoneyTo(string playerId, int moneySaved)
    {
        foreach(PlayerInGame playerEach in playersList)
        {
            if (playerEach.id == playerId)
            {
                playerEach.moneySaved += moneySaved;
            }
        }
    }

    //Get a random discount with some percentages.
    public int RandomDiscount()
    {
        int random = Random.Range(0, 10);
        if (random < 7)
        {
            //APPLY PERCENTAGE FROM 10 - 40%
            return Random.Range(10, 40);
        }else if (random < 9)
        {
            //APPLY PERCENTAGE FROM 40 - 70%
            return Random.Range(40, 70);
        }
        else
        {
            //APPLY PERCENTAGE FROM 70 - 100%
            return Random.Range(70, 100);
        }
    }

    //Get the money saved by the player specified.
    public int GetMoneySaved(int player)
    {
        return playersList[player-1].moneySaved;
    }

    //Function to call when play again button is pressed.
    public void PlayAgain()
    {
        loadingScreen.GetComponent<Animator>().SetBool("Loading", true);
        StartCoroutine(LoadGameScene());
    }

    //Function to load the game again in a corutine.
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

    //Function to load the menu in a corutine.
    IEnumerator LoadMenuScene()
    {
        // The Application loads the Scene in the background at the same time as the current Scene.
        //This is particularly good for creating loading screens. You could also load the Scene by build //number.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainMenu");

        //Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    //Function to load the exit scene in a corutine.
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

    //Function to call when main menu button is pressed.
    public void MainMenu()
    {
        loadingScreen.GetComponent<Animator>().SetBool("Loading", true);
        StartCoroutine(LoadMenuScene());
    }

    //Function to call when exit game button is pressed.
    public void ExitGame()
    {
        loadingScreen.GetComponent<Animator>().SetBool("Loading", true);
        StartCoroutine(LoadExitScene());
    } 

    //Returns the actual object in sale.
    public GameObject GetActualItem()
    {
        return currentSale;
    }

    //Funtion to call when we press continue.
    public void UnpauseGame()
    {
        gamePaused = false;
        pauseCanvas.GetComponent<Animator>().SetBool("Pause", false);
        Time.timeScale = 1.0f;
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
    }
}
