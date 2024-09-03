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

    public Slider p1ManaBar;
    public Slider p2ManaBar;
    public Slider p3ManaBar;
    public Slider p4ManaBar;

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



    public void SpawnSelectedPlayers()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        p1DamageDisplay = GameObject.Find("DmgDisplayP1").GetComponent<Text>();
        p2DamageDisplay = GameObject.Find("DmgDisplayP2").GetComponent<Text>();
        p3DamageDisplay = GameObject.Find("DmgDisplayP3").GetComponent<Text>();
        p4DamageDisplay = GameObject.Find("DmgDisplayP4").GetComponent<Text>();
        p1StockDisplay = GameObject.Find("P1StockCount").GetComponent<Text>();
        p2StockDisplay = GameObject.Find("P2StockCount").GetComponent<Text>();
        p3StockDisplay = GameObject.Find("P3StockCount").GetComponent<Text>();
        p4StockDisplay = GameObject.Find("P4StockCount").GetComponent<Text>();
        p1ManaBar = GameObject.Find("P1Mana").GetComponent<Slider>();
        p2ManaBar = GameObject.Find("P2Mana").GetComponent<Slider>();
        p3ManaBar = GameObject.Find("P3Mana").GetComponent<Slider>();
        p4ManaBar = GameObject.Find("P4Mana").GetComponent<Slider>();

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
                        currentObject.GetComponent<Conqueror>().m_manaBar = p1ManaBar;
                        playerManager.m_conqueror = currentObject;
                        playerManager.P1isPlayer = true;
                        playerManager.P1Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Blue";
                        currentObject.GetComponent<Conqueror>().isPlayer = true;


                    }
                    if (player.Key == 1)
                    {
                        currentObject.transform.position = new Vector3(1f, 1.5f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p2StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p2DamageDisplay;
                        currentObject.GetComponent<Conqueror>().m_manaBar = p2ManaBar;
                        playerManager.m_conqueror2 = currentObject;
                        playerManager.P2isPlayer = true;
                        playerManager.P2Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Red";
                        currentObject.GetComponent<Conqueror>().isPlayer = true;
                    }
                    if (player.Key == 2)
                    {
                        currentObject.transform.position = new Vector3(-2f, 6f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p3StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p3DamageDisplay;
                        currentObject.GetComponent<Conqueror>().m_manaBar = p3ManaBar;
                        p3DamageDisplay.transform.parent.gameObject.SetActive(true);
                        playerManager.m_conqueror3 = currentObject;
                        playerManager.P3isPlayer = true;
                        playerManager.P3Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Green";
                        currentObject.GetComponent<Conqueror>().isPlayer = true;
                    }
                }

                if (MenuEvents.gameModeSelect == 3)
                {
                    MenuEvents.audioManager.PlaySound("Joust");
                    if (player.Key == 0)
                    {
                        currentObject.transform.position = new Vector3(-26f, 1.5f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p1StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p1DamageDisplay;
                        currentObject.GetComponent<Conqueror>().m_manaBar = p1ManaBar;
                        playerManager.m_conqueror = currentObject;
                        playerManager.P1isPlayer = true;
                        playerManager.P1Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Blue";
                        currentObject.GetComponent<Conqueror>().isPlayer = true;

                    }
                    if (player.Key == 1)
                    {
                        currentObject.transform.position = new Vector3(-24.5f, 1.5f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p2StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p2DamageDisplay;
                        currentObject.GetComponent<Conqueror>().m_manaBar = p2ManaBar;
                        playerManager.m_conqueror2 = currentObject;
                        playerManager.P2isPlayer = true;
                        playerManager.P2Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Blue";
                        currentObject.GetComponent<Conqueror>().isPlayer = true;
                    }
                    if (player.Key == 2)
                    {
                        currentObject.transform.position = new Vector3(26f, 6f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p3StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p3DamageDisplay;
                        currentObject.GetComponent<Conqueror>().m_manaBar = p3ManaBar;
                        p3DamageDisplay.transform.parent.gameObject.SetActive(true);
                        playerManager.m_conqueror3 = currentObject;
                        playerManager.P3isPlayer = true;
                        playerManager.P3Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Red";
                        currentObject.GetComponent<Conqueror>().isPlayer = true;
                    }
                    if (player.Key == 3)
                    {
                        currentObject.transform.position = new Vector3(24.5f, 6f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p3StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p3DamageDisplay;
                        p4DamageDisplay.transform.parent.gameObject.SetActive(true);
                        playerManager.m_conqueror4 = currentObject;
                        playerManager.P4isPlayer = true;
                        playerManager.P4Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Red";
                        currentObject.GetComponent<Conqueror>().isPlayer = true;
                    }
                }

                if (MenuEvents.gameModeSelect == 4)
                {
                    MenuEvents.audioManager.PlaySound("Joust");
                    if (player.Key == 0)
                    {
                        currentObject.transform.position = new Vector3(0f, 1f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p1StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p1DamageDisplay;
                        currentObject.GetComponent<Conqueror>().m_manaBar = p1ManaBar;
                        playerManager.m_conqueror = currentObject;
                        playerManager.P1isPlayer = true;
                        playerManager.P1Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Blue";
                        currentObject.GetComponent<Conqueror>().isPlayer = true;

                    }
                    if (player.Key == 1)
                    {
                        currentObject.transform.position = new Vector3(17f, 9.8f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p2StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p2DamageDisplay;
                        currentObject.GetComponent<Conqueror>().m_manaBar = p2ManaBar;
                        playerManager.m_conqueror2 = currentObject;
                        playerManager.P2isPlayer = true;
                        playerManager.P2Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Red";
                        currentObject.GetComponent<Conqueror>().isPlayer = true;
                    }
                    if (player.Key == 2)
                    {
                        currentObject.transform.position = new Vector3(-2f, 1f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p2StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p2DamageDisplay;
                        currentObject.GetComponent<Conqueror>().m_manaBar = p2ManaBar;
                        playerManager.m_conqueror2 = currentObject;
                        playerManager.P2isPlayer = true;
                        playerManager.P2Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Red";
                        currentObject.GetComponent<Conqueror>().isPlayer = true;
                    }
                }

                if (MenuEvents.gameModeSelect == 6)
                {
                    MenuEvents.audioManager.PlaySound("Scrap");
                    if (player.Key == 0)
                    {
                        currentObject.transform.position = new Vector3(-144f, 0f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p1StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p1DamageDisplay;
                        currentObject.GetComponent<Conqueror>().m_manaBar = p1ManaBar;
                        playerManager.m_conqueror = currentObject;
                        playerManager.P1isPlayer = true;
                        playerManager.P1Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Blue";
                        currentObject.GetComponent<Conqueror>().isPlayer = true;

                    }
                    if (player.Key == 1)
                    {
                        currentObject.transform.position = new Vector3(17f, 9.8f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p2StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p2DamageDisplay;
                        currentObject.GetComponent<Conqueror>().m_manaBar = p2ManaBar;
                        playerManager.m_conqueror2 = currentObject;
                        playerManager.P2isPlayer = true;
                        playerManager.P2Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Red";
                        currentObject.GetComponent<Conqueror>().isPlayer = true;
                    }
                    if (player.Key == 2)
                    {
                        currentObject.transform.position = new Vector3(-2f, 1f, 0);
                        currentObject.GetComponent<Conqueror>().m_StockDisplay = p2StockDisplay;
                        currentObject.GetComponent<Conqueror>().m_DamageDisplay = p2DamageDisplay;
                        currentObject.GetComponent<Conqueror>().m_manaBar = p2ManaBar;
                        playerManager.m_conqueror2 = currentObject;
                        playerManager.P2isPlayer = true;
                        playerManager.P2Defeated = false;
                        currentObject.GetComponent<Conqueror>().teamColor = "Red";
                        currentObject.GetComponent<Conqueror>().isPlayer = true;
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

        if (MenuEvents.CPUsActive)
        {
            if (MenuEvents.gameModeSelect == 2)
            {
                var CPU1 = Resources.Load<GameObject>("PrototypeHero");
                CPU1.transform.position = new Vector3(2f, 1.5f, 0);
                CPU1.GetComponent<Conqueror>().m_StockDisplay = p2StockDisplay;
                CPU1.GetComponent<Conqueror>().m_DamageDisplay = p2DamageDisplay;
                playerManager.m_conqueror2 = CPU1;
                playerManager.P2Defeated = false;
                CPU1.GetComponent<Conqueror>().teamColor = "Red";
                CPU1.GetComponent<Conqueror>().isPlayer = false;
                CPU1.GetComponent<PlayerInput>().DeactivateInput();

                Instantiate(CPU1);
                CPU1.GetComponent<CPUBehavior>().routine = "HoldGround";
            }
            if (MenuEvents.gameModeSelect == 3)
            {
                var CPU1 = Resources.Load<GameObject>("PrototypeHero");
                CPU1.transform.position = new Vector3(26f, 6f, 0);
                CPU1.GetComponent<Conqueror>().m_StockDisplay = p3StockDisplay;
                CPU1.GetComponent<Conqueror>().m_DamageDisplay = p3DamageDisplay;
                playerManager.m_conqueror3 = CPU1;
                playerManager.P3Defeated = false;
                CPU1.GetComponent<Conqueror>().teamColor = "Red";
                CPU1.GetComponent<Conqueror>().isPlayer = false;
                CPU1.GetComponent<CPUBehavior>().routine = "Lane";
                CPU1.GetComponent<PlayerInput>().DeactivateInput();

                var CPU2 = Resources.Load<GameObject>("TheChampion");
                CPU2.transform.position = new Vector3(26f, 6f, 0);
                CPU2.GetComponent<Conqueror>().m_StockDisplay = p4StockDisplay;
                CPU2.GetComponent<Conqueror>().m_DamageDisplay = p4DamageDisplay;
                playerManager.m_conqueror4 = CPU2;
                playerManager.P4Defeated = false;
                CPU2.GetComponent<Conqueror>().teamColor = "Red";
                CPU2.GetComponent<Conqueror>().isPlayer = false;
                CPU2.GetComponent<CPUBehavior>().routine = "HoldGround";
                CPU2.GetComponent<PlayerInput>().DeactivateInput();

                Instantiate(CPU1);
                Instantiate(CPU2);
            }
            if (MenuEvents.gameModeSelect == 4)
            {
                var CPU1 = Resources.Load<GameObject>("PrototypeHero");
                CPU1.transform.position = new Vector3(20f, 10f, 0);
                CPU1.GetComponent<Conqueror>().m_StockDisplay = p2StockDisplay;
                CPU1.GetComponent<Conqueror>().m_DamageDisplay = p2DamageDisplay;
                playerManager.m_conqueror2 = CPU1;
                playerManager.P2Defeated = false;
                CPU1.GetComponent<Conqueror>().teamColor = "Red";
                CPU1.GetComponent<Conqueror>().isPlayer = false;
                CPU1.GetComponent<PlayerInput>().DeactivateInput();

                Instantiate(CPU1);
            }
            if (MenuEvents.gameModeSelect == 4)
            {
                var CPU1 = Resources.Load<GameObject>("PrototypeHero");
                CPU1.transform.position = new Vector3(20f, 10f, 0);
                CPU1.GetComponent<Conqueror>().m_StockDisplay = p2StockDisplay;
                CPU1.GetComponent<Conqueror>().m_DamageDisplay = p2DamageDisplay;
                playerManager.m_conqueror2 = CPU1;
                playerManager.P2Defeated = false;
                CPU1.GetComponent<Conqueror>().teamColor = "Red";
                CPU1.GetComponent<Conqueror>().isPlayer = false;
                CPU1.GetComponent<PlayerInput>().DeactivateInput();

                Instantiate(CPU1);
            }
            
        }
        if (MenuEvents.gameModeSelect == 6)
        {
            var CPU1 = Resources.Load<GameObject>("PrototypeHero");
            CPU1.transform.position = new Vector3(-93.5f, 1f, 0);
            CPU1.GetComponent<Conqueror>().m_StockDisplay = p2StockDisplay;
            CPU1.GetComponent<Conqueror>().m_DamageDisplay = p2DamageDisplay;
            playerManager.m_conqueror2 = CPU1;
            playerManager.P2Defeated = false;
            CPU1.GetComponent<Conqueror>().teamColor = "Red";
            CPU1.GetComponent<Conqueror>().isPlayer = false;
            CPU1.GetComponent<PlayerInput>().DeactivateInput();

            Instantiate(CPU1);
        }
    }


}
