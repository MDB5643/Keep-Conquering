using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets._2D;

public class PlayerManager : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    int characterIndex;
    public GameObject mainCam;

    public GameObject redMinion;
    public GameObject blueMinion;

    public int redMinionCount = 0;
    public int blueMinionCount = 0;

    public Text p1DamageDisplay;
    public Text p2DamageDisplay;
    public Text p3DamageDisplay;
    public Text p4DamageDisplay;

    public AudioManager_PrototypeHero m_audioManager;

    public Text infoText;
    public GameObject m_conqueror;
    public GameObject m_conqueror2;
    public GameObject m_CPU;
    public GameObject m_BB_CPU;
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
            
            characterIndex = 0;
            if (MenuEvents.gameModeSelect == 1)
            {
                m_conqueror = Instantiate(playerPrefabs[MenuEvents.P1Select], new Vector3(-45f, 6f, 0),  //Vector3(76f, 6f, 0),
                                Quaternion.identity);
                
                if (m_CPU != null && MenuEvents.P2Select == 0)
                {
                    m_conqueror2 = Instantiate(m_CPU, new Vector3(90f, 6f, 0),  //Vector3(76f, 6f, 0),
                                Quaternion.identity);
                }
                else if (m_BB_CPU != null && MenuEvents.P2Select == 1)
                {
                    m_conqueror2 = Instantiate(m_BB_CPU, new Vector3(90f, 6f, 0),  //Vector3(76f, 6f, 0),
                                Quaternion.identity);
                }
            }
            else if (MenuEvents.gameModeSelect == 2)
            {
                m_conqueror = Instantiate(playerPrefabs[MenuEvents.P1Select], new Vector3(-4.72f, 1.5f, 0),  //Vector3(76f, 6f, 0),
                                Quaternion.identity);
                if (m_CPU != null && MenuEvents.P2Select == 0)
                {
                    m_conqueror2 = Instantiate(m_CPU, new Vector3(1f, 6f, 0),  //Vector3(76f, 6f, 0),
                                Quaternion.identity);
                }
                else if (m_BB_CPU != null && MenuEvents.P2Select == 1)
                {
                    m_conqueror2 = Instantiate(m_BB_CPU, new Vector3(1f, 6f, 0),  //Vector3(76f, 6f, 0),
                                Quaternion.identity);
                }
            }

            var cam = mainCam.GetComponent<Camera2DFollow>();

            if (m_conqueror.GetComponent<PrototypeHero>() != null)
            {
                m_conqueror.GetComponent<PrototypeHero>().m_DamageDisplay = p1DamageDisplay;
                m_conqueror.GetComponent<PrototypeHero>().infoText = infoText;
            }
            else if (m_conqueror.GetComponent<TheChampion>() != null)
            {
                m_conqueror.GetComponent<TheChampion>().m_DamageDisplay = p1DamageDisplay;
                m_conqueror.GetComponent<TheChampion>().infoText = infoText;
            }
            else if (m_conqueror.GetComponent<Conqueror>() != null)
            {
                m_conqueror.GetComponent<Conqueror>().m_DamageDisplay = p1DamageDisplay;
                m_conqueror.GetComponent<Conqueror>().infoText = infoText;
            }
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
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skyBoxSpeed);
        if (m_conqueror.transform.position.z > 10 && !Input.GetKey("m"))
        {
            var cam = mainCam.GetComponent<Camera2DFollow>();
            cam.maxValue.z = 1;
            cam.GetComponentInParent<Camera>().fieldOfView = 30;
        }
        else if (!Input.GetKey("m"))
        {
            var cam = mainCam.GetComponent<Camera2DFollow>();
            cam.maxValue.z = -24;
            cam.GetComponentInParent<Camera>().fieldOfView = 27;
        }
        if (Input.GetKey("m"))
        {
            var cam = mainCam.GetComponent<Camera2DFollow>();
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
            var cam = mainCam.GetComponent<Camera2DFollow>();
            cam.GetComponentInParent<Camera>().fieldOfView = 27;
            cam.maxValue.x = 185;
            cam.maxValue.y = 10;
            cam.minValues.z = -24;
            cam.minValues.x = -90;
            cam.minValues.y = -10;
        }

        minionSpawnElapsedTime += Time.deltaTime;

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
