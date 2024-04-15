using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneInitialization : MonoBehaviour
{
    public MenuEvents menuEvents;
    public PlayerManager playerManager;

    public Text p1DamageDisplay;
    public Text p2DamageDisplay;
    public Text p3DamageDisplay;
    public Text p4DamageDisplay;

    public Text p1StockDisplay;
    public Text p2StockDisplay;
    public Text p3StockDisplay;
    public Text p4StockDisplay;

    public static bool P1Set = false;
    public static bool P2Set = false;
    public static bool P3Set = false;
    public static bool P4Set = false;

    public void Init()
    {
        SceneManager.activeSceneChanged += SceneBeginCheck;
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += SceneBeginCheck;
    }


    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= SceneBeginCheck;
    }


    private void SceneBeginCheck(Scene fromScene, Scene toScene)
    {

        //foreach (var item in PlayerObjectHandler.playerControlSchemes)
        //{
        //    UnityEngine.Debug.Log(item);
        //}


        if (PlayerObjectHandler.shouldSpawnSelectedPlayers)
        {
            SpawnSelectedPlayers();
            PlayerObjectHandler.shouldSpawnSelectedPlayers = false;
        }
    }



    private void SpawnSelectedPlayers()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        p1DamageDisplay = GameObject.Find("DmgDisplayP1").GetComponent<Text>();
        p2DamageDisplay = GameObject.Find("DmgDisplayP2").GetComponent<Text>();
        p3DamageDisplay = GameObject.Find("DmgDisplayP3").GetComponent<Text>();
        p1StockDisplay = GameObject.Find("P1StockCount").GetComponent<Text>();
        p2StockDisplay = GameObject.Find("P2StockCount").GetComponent<Text>();
        p3StockDisplay = GameObject.Find("P3StockCount").GetComponent<Text>();

        foreach (var player in PlayerObjectHandler.playerControllers)
        {
            var playerController = PlayerObjectHandler.playerControllers[player.Key];
            var playerObjectName = PlayerObjectHandler.playerSelectionNames[player.Key];
            var playerControlScheme = PlayerObjectHandler.playerControlSchemes[player.Key];

            GameObject parentPlayerObject = new GameObject();

            for (int i = 0; i < playerObjectName.Count; i++)
            {
                var currentObject = Resources.Load<GameObject>(playerObjectName[i]);
                if (MenuEvents.gameModeSelect == 2)
                {
                    MenuEvents.audioManager.PlaySound("Scrap");
                    if (player.Key == 0)
                    {
                        currentObject.transform.position = new Vector3(-5f, 1.5f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p1StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p1DamageDisplay;
                        playerManager.m_conqueror = currentObject;
                        playerManager.P1isPlayer = true;
                        playerManager.P1Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Blue";
                        
                    }
                    if (player.Key == 1)
                    {
                        currentObject.transform.position = new Vector3(1f, 1.5f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p2StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p2DamageDisplay;
                        playerManager.m_conqueror2 = currentObject;
                        playerManager.P2isPlayer = true;
                        playerManager.P2Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Red";
                        //p3DamageDisplay.transform.parent.gameObject.SetActive(true);
                    }
                    if (player.Key == 2)
                    {
                        currentObject.transform.position = new Vector3(-2f, 6f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p3StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p3DamageDisplay;
                        p3DamageDisplay.transform.parent.gameObject.SetActive(true);
                        playerManager.m_conqueror3 = currentObject;
                        playerManager.P3isPlayer = true;
                        playerManager.P3Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Green";
                    }
                }

                if (MenuEvents.gameModeSelect == 3)
                {
                    MenuEvents.audioManager.PlaySound("Joust");
                    if (player.Key == 0)
                    {
                        currentObject.transform.position = new Vector3(-31f, 1.5f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p1StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p1DamageDisplay;
                        playerManager.m_conqueror = currentObject;
                        playerManager.P1isPlayer = true;
                        playerManager.P1Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Blue";

                    }
                    if (player.Key == 1)
                    {
                        currentObject.transform.position = new Vector3(-29.5f, 1.5f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p2StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p2DamageDisplay;
                        playerManager.m_conqueror2 = currentObject;
                        playerManager.P2isPlayer = true;
                        playerManager.P2Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Blue";
                        //p3DamageDisplay.transform.parent.gameObject.SetActive(true);
                    }
                    if (player.Key == 2)
                    {
                        currentObject.transform.position = new Vector3(31f, 6f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p3StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p3DamageDisplay;
                        p3DamageDisplay.transform.parent.gameObject.SetActive(true);
                        playerManager.m_conqueror3 = currentObject;
                        playerManager.P3isPlayer = true;
                        playerManager.P3Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Red";
                    }
                    if (player.Key == 3)
                    {
                        currentObject.transform.position = new Vector3(29.5f, 6f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p3StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p3DamageDisplay;
                        p4DamageDisplay.transform.parent.gameObject.SetActive(true);
                        playerManager.m_conqueror4 = currentObject;
                        playerManager.P4isPlayer = true;
                        playerManager.P4Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Red";
                    }
                }

                // Only activate PlayerInput component on the first object (it defines the "player"
                if (i == 0)
                {
                    parentPlayerObject = currentObject;
                    PlayerInput playerInput = PlayerInput.Instantiate(currentObject, player.Key, playerControlScheme, -1, playerController);
                    
                    // Activates the player input component on the prefab we just instantiated
                    // We have the component disabled by default, otherwise it could not be a "selectable object" independent of the PlayerInput component on the cursor
                    // in the selection screen
                    currentObject.GetComponent<PlayerControls>().SetPlayerInputActive(true, playerInput);

                    //  *** It seems...that the above Instantiation doesn't exactly work... I'm assuming, because the PlayerInput component on the prefab is starting off
                    // disabled, that it...doesn't work.  This code here will force it to keep the device/scheme/etc... that we tried to assign the wretch above!
                    var inputUser = playerInput.user;
                    playerInput.SwitchCurrentControlScheme(playerControlScheme);
                    InputUser.PerformPairingWithDevice(playerController, inputUser, InputUserPairingOptions.UnpairCurrentDevicesFromUser);
                }

                // If not the first object (sword/vehicle/etc...) just instantiate, don't associate a PlayerInput
                else
                {
                    Instantiate(currentObject, parentPlayerObject.transform);
                }



            }
        }
    }


}
