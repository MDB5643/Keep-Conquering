using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets._2D;

public class PlayerManager : MonoBehaviour
{
    public SceneInitialization initializer;

    public GameObject[] playerPrefabs;
    public GameObject[] cpuPrefabs;
    public GameObject mainCam;

    public GameObject redMinion;
    public GameObject blueMinion;

    public Text p1StockDisplay;
    public Text p2StockDisplay;
    public Text p3StockDisplay;
    public Text p4StockDisplay;

    public int redMinionCount = 0;
    public int blueMinionCount = 0;

    public Text EndGameText;
    public bool GameOver = false;
    public float EndScreenLinger = 0.0f;

    public bool P1isPlayer = true;
    public bool P2isPlayer = false;
    public bool P3isPlayer = false;
    public bool P4isPlayer = false;

    public bool P1Defeated = false;
    public bool P2Defeated = false;
    public bool P3Defeated = true;
    public bool P4Defeated = true;

    public AudioManager_PrototypeHero m_audioManager;

    public Text infoText;
    public GameObject m_conqueror;
    public GameObject m_conqueror2;
    public GameObject m_conqueror3;
    public GameObject m_conqueror4;
    public GameObject m_CPU;
    public GameObject m_BB_CPU;
    public GameObject m_RR_CPU;
    public float minionSpawnElapsedTime;
    public float secondsBetweenSpawn = 30;

    public float skyBoxSpeed;

    public bool isBlueChallenging = false;
    public bool isRedChallenging = false;

    public bool challengeStarted = false;

    private void Awake()
    {
        try
        {
            initializer = GameObject.Find("SelectionScreenInputManager").GetComponent<SceneInitialization>();
            initializer.Init();
            //m_audioManager.PlaySound("MainMenu");

            //characterIndex = 0;
            if (MenuEvents.gameModeSelect == 1)
            {
                m_conqueror = Instantiate(playerPrefabs[MenuEvents.P1Select], new Vector3(-45f, 6f, 0),  //Vector3(76f, 6f, 0),
                                Quaternion.identity);
                P1Defeated = false;
                m_conqueror.GetComponent<Conqueror>().m_StockCount = -1;
                P1isPlayer = true;

                //if (MenuEvents.P2Set && !P2isPlayer)
                //{
                //    m_conqueror2 = Instantiate(cpuPrefabs[MenuEvents.P2Select], new Vector3(90f, 6f, 0),  //Vector3(76f, 6f, 0),
                //                Quaternion.identity);
                //    P2Defeated = false;
                //    m_conqueror2.GetComponent<Conqueror>().m_StockCount = -1;
                //}
                //if (MenuEvents.P3Set && !P3isPlayer)
                //{
                //    m_conqueror3 = Instantiate(cpuPrefabs[MenuEvents.P3Select], new Vector3(88f, 6f, 0),  //Vector3(76f, 6f, 0),
                //                Quaternion.identity);
                //    p3DamageDisplay.transform.parent.gameObject.SetActive(true);
                //    P3Defeated = false;
                //    m_conqueror3.GetComponent<Conqueror>().m_StockCount = -1;
                //}
                //if (MenuEvents.P4Set && !P4isPlayer)
                //{
                //    m_conqueror4 = Instantiate(cpuPrefabs[MenuEvents.P4Select], new Vector3(86f, 6f, 0),  //Vector3(76f, 6f, 0),
                //                Quaternion.identity);
                //    p4DamageDisplay.transform.parent.gameObject.SetActive(true);
                //    P4Defeated = false;
                //    m_conqueror4.GetComponent<Conqueror>().m_StockCount = -1;
                //}
            }
            else if (MenuEvents.gameModeSelect == 2)
            {
                
                //m_conqueror = Instantiate(playerPrefabs[MenuEvents.P1Select], new Vector3(-5f, 1.5f, 0),  //Vector3(76f, 6f, 0),
                //                Quaternion.identity);
                //P1Defeated = false;
                //m_conqueror.GetComponent<Conqueror>().m_StockDisplay = p1StockDisplay;
                //P1isPlayer = true;
                if (MenuEvents.P2Set && !P2isPlayer)
                {
                    //m_conqueror2 = Instantiate(cpuPrefabs[MenuEvents.P2Select], new Vector3(1f, 1.5f, 0),  //Vector3(76f, 6f, 0),
                    //            Quaternion.identity);
                    //m_conqueror2.GetComponent<Conqueror>().m_StockDisplay = p2StockDisplay;
                    //P2Defeated = false;
                }
                if (MenuEvents.P3Set && !P3isPlayer)
                {
                    //m_conqueror3 = Instantiate(cpuPrefabs[MenuEvents.P3Select], new Vector3(-2f, 6f, 0),  //Vector3(76f, 6f, 0),
                    //            Quaternion.identity);
                    //p3DamageDisplay.transform.parent.gameObject.SetActive(true);
                    //m_conqueror3.GetComponent<Conqueror>().m_StockDisplay = p3StockDisplay;
                    //P3Defeated = false;
                }
                if (MenuEvents.P4Set && !P4isPlayer)
                {
                    //m_conqueror4 = Instantiate(cpuPrefabs[MenuEvents.P4Select], new Vector3(0f, 6f, 0),  //Vector3(76f, 6f, 0),
                    //            Quaternion.identity);
                    //p3DamageDisplay.transform.parent.gameObject.SetActive(true);
                    //m_conqueror4.GetComponent<Conqueror>().m_StockDisplay = p4StockDisplay;
                    //P4Defeated = false;
                }
            }

            var cam = mainCam.GetComponent<Camera2DFollow>();

            //if (m_conqueror)
            //{
            //    m_conqueror.GetComponent<Conqueror>().m_DamageDisplay = p1DamageDisplay;
            //    m_conqueror.GetComponent<Conqueror>().infoText = infoText;
            //}
            //if (m_conqueror2)
            //{
            //    m_conqueror2.GetComponent<Conqueror>().m_DamageDisplay = p2DamageDisplay;
            //}
            //if (m_conqueror3)
            //{
            //    m_conqueror3.GetComponent<Conqueror>().m_DamageDisplay = p3DamageDisplay;
            //}
            //if (m_conqueror4)
            //{
            //    m_conqueror4.GetComponent<Conqueror>().m_DamageDisplay = p4DamageDisplay;
            //}

            //CHAGE FOLLOW TARGET FOR TESTING
            mainCam.GetComponent<Camera2DFollow>().target = m_conqueror.transform;

            cam.transform.parent = null;
        }
        catch(System.Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }

    }

    // Update is called once per frame
    void Update()
    {
        var cam = mainCam.GetComponent<Camera2DFollow>();
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skyBoxSpeed);
        //if (m_conqueror.transform.position.z > 10 && !Input.GetKey("m"))
        //{
        //    var cam = mainCam.GetComponent<Camera2DFollow>();
        //    cam.maxValue.z = 1;
        //    cam.GetComponentInParent<Camera>().fieldOfView = 30;
        //}
        //else if (!Input.GetKey("m"))
        //{
        //    var cam = mainCam.GetComponent<Camera2DFollow>();
        //    cam.maxValue.z = -24;
        //    cam.GetComponentInParent<Camera>().fieldOfView = 27;
        //}
        if (Input.GetKey("m"))
        {
            cam.GetComponentInParent<Camera>().fieldOfView = 100;
            cam.maxValue.z = -28;
            cam.maxValue.x = 40;
            cam.maxValue.y = 0;
            cam.minValues.z = -47;
            cam.minValues.x = 40;
            cam.minValues.y = 0;
        }
        else if (Input.GetKeyUp("m"))
        {
            cam.GetComponentInParent<Camera>().fieldOfView = 27;
            cam.maxValue.x = 185;
            cam.maxValue.y = 10;
            cam.minValues.z = -24;
            cam.minValues.x = -90;
            cam.minValues.y = -10;
        }

        minionSpawnElapsedTime += Time.deltaTime;

        var foundPlayersObjects = FindObjectsOfType<Conqueror>();
        foreach (Conqueror c in foundPlayersObjects)
        {
            if (c.m_cameraShake)
            {
                StartCoroutine(cam.Shaking(c.m_shakeIntensity));
                c.m_cameraShake = false;
            }
        }

        if (m_conqueror)
        {
            if (m_conqueror.GetComponent<Conqueror>().m_cameraShake)
            {
                m_conqueror.GetComponent<Conqueror>().m_cameraShake = false;
                StartCoroutine(cam.Shaking(m_conqueror.GetComponent<Conqueror>().m_shakeIntensity));
            }
            P1Defeated = p1StockDisplay.text.Substring(1, 1) == "0";
        }
        if (m_conqueror2)
        {
            if (m_conqueror2.GetComponent<Conqueror>().m_cameraShake)
            {
                m_conqueror2.GetComponent<Conqueror>().m_cameraShake = false;
                StartCoroutine(cam.Shaking(m_conqueror2.GetComponent<Conqueror>().m_shakeIntensity));
            }
            P2Defeated = p2StockDisplay.text.Substring(1, 1) == "0";
        }
        else
        {
            P2Defeated = true;
        }
        if (m_conqueror3)
        {
            if (m_conqueror3.GetComponent<Conqueror>().m_cameraShake)
            {
                m_conqueror3.GetComponent<Conqueror>().m_cameraShake = false;
                StartCoroutine(cam.Shaking(m_conqueror3.GetComponent<Conqueror>().m_shakeIntensity));
            }
            P3Defeated = p3StockDisplay.text.Substring(1, 1) == "0";
        }
        else
        {
            P3Defeated = true;
        }
        if (m_conqueror4)
        {
            if (m_conqueror4.GetComponent<Conqueror>().m_cameraShake)
            {
                m_conqueror4.GetComponent<Conqueror>().m_cameraShake = false;
                StartCoroutine(cam.Shaking(m_conqueror4.GetComponent<Conqueror>().m_shakeIntensity));
            }
            P4Defeated = p4StockDisplay.text.Substring(1, 1) == "0";
        }
        else
        {
            P4Defeated = true;
        }

        if (GameOver)
        {
            EndScreenLinger += Time.deltaTime;
            if (EndScreenLinger > 7.0f)
            {
                MenuEvents.audioManager.StopSound("Scrap");
                MenuEvents.audioManager.StopSound("Joust");
                MenuEvents.DestroyAudio();
                MenuEvents.ReturningToMenu = true;
                MenuEvents.gameModeSelect = 0;
                SceneManager.LoadScene(0);
                MenuEvents.P1Set = false;
                MenuEvents.P2Set = false;
                MenuEvents.P3Set = false;
                MenuEvents.P4Set = false;
            }
        }

        if (P1Defeated)
        {
            if (P1isPlayer && !P2isPlayer && !P3isPlayer && !P4isPlayer)
            {
                EndGameText.gameObject.SetActive(true);
                EndGameText.color = Color.red;
                EndGameText.text = "DEFEAT";
                GameOver = true;
            }
        }
        if (P2Defeated && P3Defeated && P4Defeated && !P1Defeated && MenuEvents.gameModeSelect == 2)
        {
            EndGameText.gameObject.SetActive(true);
            EndGameText.color = Color.blue;
            EndGameText.text = "P1 WINS";
            GameOver = true;
        }
        if (!P2Defeated && P3Defeated && P4Defeated && P1Defeated && MenuEvents.gameModeSelect == 2)
        {
            EndGameText.gameObject.SetActive(true);
            EndGameText.color = Color.blue;
            EndGameText.text = "P2 WINS";
            GameOver = true;
        }
        if (P2Defeated && !P3Defeated && P4Defeated && P1Defeated && MenuEvents.gameModeSelect == 2)
        {
            EndGameText.gameObject.SetActive(true);
            EndGameText.color = Color.blue;
            EndGameText.text = "P3 WINS";
            GameOver = true;
        }
        if (P2Defeated && P3Defeated && !P4Defeated && P1Defeated && MenuEvents.gameModeSelect == 2)
        {
            EndGameText.gameObject.SetActive(true);
            EndGameText.color = Color.blue;
            EndGameText.text = "P4 WINS";
            GameOver = true;
        }


        if (minionSpawnElapsedTime > secondsBetweenSpawn && MenuEvents.gameModeSelect == 1)
        {
            minionSpawnElapsedTime = 0;
            Debug.Log(true);
            if (redMinionCount <= 9)
            {
                Vector3 spawnPosition = new Vector3(131f, 4f, 0);
                Instantiate(redMinion, spawnPosition, Quaternion.identity).SetActive(true);
                spawnPosition = new Vector3(133f, 4f, 0);
                Instantiate(redMinion, spawnPosition, Quaternion.identity).SetActive(true);
                spawnPosition = new Vector3(129f, 4f, 0);
                Instantiate(redMinion, spawnPosition, Quaternion.identity).SetActive(true);
                redMinionCount += 3;
            }
            if (blueMinionCount <= 9)
            {
                Vector3 spawnPosition = new Vector3(-46f, 4f, 0);
                Instantiate(blueMinion, spawnPosition, Quaternion.identity).SetActive(true);
                spawnPosition = new Vector3(-48f, 4f, 0);
                Instantiate(blueMinion, spawnPosition, Quaternion.identity).SetActive(true);
                spawnPosition = new Vector3(-50f, 4f, 0);
                Instantiate(blueMinion, spawnPosition, Quaternion.identity).SetActive(true);
                blueMinionCount += 3;
            }
        }
        if (minionSpawnElapsedTime > secondsBetweenSpawn && MenuEvents.gameModeSelect == 3)
        {
            minionSpawnElapsedTime = 0;
            Debug.Log(true);
            if (redMinionCount <= 9)
            {
                Vector3 spawnPosition = new Vector3(30.5f, 3f, 0);
                Instantiate(redMinion, spawnPosition, Quaternion.identity).SetActive(true);
                spawnPosition = new Vector3(32f, 3f, 0);
                Instantiate(redMinion, spawnPosition, Quaternion.identity).SetActive(true);
                spawnPosition = new Vector3(29f, 3f, 0);
                Instantiate(redMinion, spawnPosition, Quaternion.identity).SetActive(true);
                redMinionCount += 3;
            }
            if (blueMinionCount <= 9)
            {
                Vector3 spawnPosition = new Vector3(-30.5f, 3f, 0);
                Instantiate(blueMinion, spawnPosition, Quaternion.identity).SetActive(true);
                spawnPosition = new Vector3(-32f, 3f, 0);
                Instantiate(blueMinion, spawnPosition, Quaternion.identity).SetActive(true);
                spawnPosition = new Vector3(-29f, 3f, 0);
                Instantiate(blueMinion, spawnPosition, Quaternion.identity).SetActive(true);
                blueMinionCount += 3;
            }
        }

    }

    public void InitiateFinalChallenge(string teamColor)
    {
        challengeStarted = true;
        if (teamColor == "Blue" && challengeStarted == true)
        {
            m_conqueror.transform.position = new Vector3(-57, 6, 0);
        }
        if (teamColor == "Red" && challengeStarted == true)
        {
            m_CPU.transform.position = new Vector3(140, 6, 0);
        }
        isRedChallenging = false;
    }
}
