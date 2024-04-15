using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.InputSystem;

public class MenuEvents : MonoBehaviour
{
    public static int P1Select;
    public static int P2Select;
    public static int P3Select;
    public static int P4Select;

    public static bool P1Set = false;
    public static bool P2Set = false;
    public static bool P3Set = false;
    public static bool P4Set = false;

    public static bool ReturningToMenu = false;

    public GameObject P1Preview;
    public GameObject P2Preview;
    public GameObject P3Preview;
    public GameObject P4Preview;

    public GameObject StartButton;

    public Text P1Text;
    public Text P2Text;
    public Text P3Text;
    public Text P4Text;
     
    public Sprite BBSprite;
    public Sprite DPSprite;
    public Sprite LKSprite;
    public Sprite RRSprite;

    public Conqueror DPPrefab;
    public Conqueror BBPrefab;
    public Conqueror RRPrefab;
    public Conqueror ROPrefab;

    public Conqueror P1Active;
    public Conqueror P2Active;
    public Conqueror P3Active;
    public Conqueror P4Active;

    public Button FFAButton;
    public Button JoustButton;

    public bool P3Enabled;
    public bool P4Enabled;

    public static bool levelLoaded = false;

    public string currentSelector = "";

    public AudioManager_PrototypeHero audioManagerPrefab;
    public static AudioManager_PrototypeHero audioManager;
    public Sound[] audioManagerSounds;
    bool audioStarted = false;
    float audioCounter = 0f;

    public static int gameModeSelect;

    public PrototypeHero proto;
    public Vector3 Player1Spawn = new Vector3(0, 1, 0);

    private int readyPlayers = 0;
    private GameObject menu_events;
   
    private void Update()
    {
        if (readyPlayers >= 2)
        {
            StartButton.GetComponent<Button>().interactable = true;
        }
        if (audioStarted == false)
        {
            audioCounter++;
            if (audioCounter == 10)
            {
                audioStarted = true;
                audioManager.PlaySound("Practice");
            }
        }
    }

    public static void LoadLevel()
    {
        if (!levelLoaded)
        {
            ReturningToMenu = false;
            audioManager.StopSound("Practice");
            SceneManager.LoadScene(gameModeSelect);
            levelLoaded = true;
        }
        
    }

    public void SetMode(int selectedMode)
    {
        gameModeSelect = selectedMode;
        if (selectedMode == 2)
        {
            FFAButton.Select();
        }
        if (selectedMode == 3)
        {
            JoustButton.Select();
        }
    }

    public void SetConqueror(int conq, string whichPlayer)
    {
        readyPlayers++;
        if (whichPlayer == "P1")
        {
            P1Select = conq;
            P1Set = true;
            //P1Active.preview = false;
        }
        if (whichPlayer == "P2")
        {
            P2Select = conq;
            P2Set = true;
        }
        if (whichPlayer == "P3")
        {
            P3Select = conq;
            P3Set = true;
        }
        if (whichPlayer == "P4")
        {
            P4Select = conq;
            P4Set = true;
        }
    }

    public void SetThisConqueror(int conq)
    {
        readyPlayers++;
        if (currentSelector == "P1")
        {
            P1Select = conq;
            P1Set = true;
            //P1Active.preview = false;
        }
        if (currentSelector == "P2")
        {
            P2Select = conq;
            P2Set = true;
        }
        if (currentSelector == "P3")
        {
            P3Select = conq;
            P3Set = true;
        }
        if (currentSelector == "P4")
        {
            P4Select = conq;
            P4Set = true;
        }
    }

    public static void DestroyAudio()
    {
        GameObject.Destroy(audioManager);
    }

    public void Awake()
    {
        audioManager = GameObject.Instantiate(audioManagerPrefab);
        audioManager.SetSoundArray(audioManagerSounds);
        StartButton.GetComponent<Button>().interactable = false;
    }

    private void Start()
    {
        Thread.Sleep(1000);
        //audioManager.PlaySound("Practice");
    }

    public void changeSelector (string selector)
    {
        currentSelector = selector;
    }

    public void PreviewConq(string conqName, string whichPlayer)
    {
        if (conqName == "TheChampion")
        {
            if (whichPlayer == "P1" && P1Set == false)
            {

                if (P1Active)
                {
                    GameObject.Destroy(P1Active.gameObject);
                }
                //P3Preview.GetComponent<Image>().sprite = DPSprite;
                P1Active = GameObject.Instantiate(BBPrefab, new Vector3(P1Preview.transform.position.x, P1Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P1Active.preview = true;
                P1Active.GetComponent<PlayerInput>().DeactivateInput();
                P1Text.text = "Burning Belligerent";
                P1Text.gameObject.SetActive(true);
            }
            if (whichPlayer == "P2" && P2Set == false)
            {
                if (P2Active)
                {
                    GameObject.Destroy(P2Active.gameObject);
                }
                //P2Preview.GetComponent<Image>().sprite = DPSprite;
                P2Active = GameObject.Instantiate(BBPrefab, new Vector3(P2Preview.transform.position.x, P2Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P2Active.preview = true;
                P2Active.GetComponent<PlayerInput>().DeactivateInput();
                P2Text.text = "Burning Belligerent";
                P2Text.gameObject.SetActive(true);
            }
            if (whichPlayer == "P3" && P3Set == false)
            {
                if (P3Active)
                {
                    GameObject.Destroy(P3Active.gameObject);
                }
                //P3Preview.GetComponent<Image>().sprite = DPSprite;
                P3Active = GameObject.Instantiate(BBPrefab, new Vector3(P3Preview.transform.position.x, P3Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P3Active.preview = true;
                P3Active.GetComponent<PlayerInput>().DeactivateInput();
                P3Text.text = "Burning Belligerent";
                P3Text.gameObject.SetActive(true);
            }
            if (whichPlayer == "P4" && P4Set == false)
            {
                if (P4Active)
                {
                    GameObject.Destroy(P4Active.gameObject);
                }
                //P3Preview.GetComponent<Image>().sprite = DPSprite;
                P4Active = GameObject.Instantiate(BBPrefab, new Vector3(P4Preview.transform.position.x, P4Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P4Active.preview = true;
                P4Active.GetComponent<PlayerInput>().DeactivateInput();
                P4Text.text = "Burning Belligerent";
                P4Text.gameObject.SetActive(true);
            }
            
        }
        if (conqName == "PrototypeHero")
        {
            if (whichPlayer == "P1" && P1Set == false)
            {
                if (P1Active)
                {
                    GameObject.Destroy(P1Active.gameObject);
                }
                ////P3Preview.GetComponent<Image>().sprite = DPSprite;
                P1Active = GameObject.Instantiate(DPPrefab, new Vector3(P1Preview.transform.position.x, P1Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P1Active.preview = true;
                P1Active.GetComponent<PlayerInput>().DeactivateInput();
                P1Text.text = "Death's Proxy";
                //P1Preview.GetComponent<Image>().gameObject.SetActive(true);
                P1Text.gameObject.SetActive(true);
            }
            if (whichPlayer == "P2" && P2Set == false)
            {
                if (P2Active)
                {
                    GameObject.Destroy(P2Active.gameObject);
                }
                //P2Preview.GetComponent<Image>().sprite = RRSprite;
                P2Active = GameObject.Instantiate(DPPrefab, new Vector3(P2Preview.transform.position.x, P2Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P2Active.preview = true;
                P2Active.GetComponent<PlayerInput>().DeactivateInput();
                P2Text.gameObject.SetActive(true);
                P2Text.text = "Death's Proxy";
            }
            if (whichPlayer == "P3" && P3Set == false)
            {
                if (P3Active)
                {
                    GameObject.Destroy(P3Active.gameObject);
                }
                //P2Preview.GetComponent<Image>().sprite = RRSprite;
                P3Active = GameObject.Instantiate(DPPrefab, new Vector3(P3Preview.transform.position.x, P3Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P3Active.preview = true;
                P3Active.GetComponent<PlayerInput>().DeactivateInput();
                P3Text.gameObject.SetActive(true);
                P3Text.text = "Death's Proxy";
            }
            if (whichPlayer == "P4" && P4Set == false)
            {
                if (P4Active)
                {
                    GameObject.Destroy(P4Active.gameObject);
                }
                //P2Preview.GetComponent<Image>().sprite = RRSprite;
                P4Active = GameObject.Instantiate(DPPrefab, new Vector3(P4Preview.transform.position.x, P4Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P4Active.preview = true;
                P4Active.GetComponent<PlayerInput>().DeactivateInput();
                P4Text.gameObject.SetActive(true);
                P4Text.text = "Death's Proxy";
            }
        }
        if (conqName == "Lumiknight")
        {
            if (whichPlayer == "P1" && P1Set == false)
            {
                if (P1Active)
                {
                    GameObject.Destroy(P1Active);
                }
                //P3Preview.GetComponent<Image>().sprite = DPSprite;
                P1Text.text = "Fighter/Support";
                P1Preview.GetComponent<Image>().gameObject.SetActive(true);
                P1Text.gameObject.SetActive(true);
            }
            if (whichPlayer == "P2" && P2Set == false)
            {
                P2Preview.GetComponent<Image>().gameObject.SetActive(true);
                P2Text.gameObject.SetActive(true);
                P2Preview.GetComponent<Image>().sprite = LKSprite;
                P2Text.text = "Fighter/Support";
            }
            if (whichPlayer == "P3" && P3Set == false)
            {
                P3Preview.GetComponent<Image>().sprite = LKSprite;
                P3Text.text = "Fighter/Support";
                P3Preview.GetComponent<Image>().gameObject.SetActive(true);
                P3Text.gameObject.SetActive(true);
            }
            if (whichPlayer == "P4" && P4Set == false)
            {
                P4Preview.GetComponent<Image>().sprite = LKSprite;
                P4Text.text = "Fighter/Support";
                P4Preview.GetComponent<Image>().gameObject.SetActive(true);
                P4Text.gameObject.SetActive(true);
            }
        }
        if (conqName == "RunebornRanger")
        {
            if (whichPlayer == "P1" && P1Set == false)
            {
                if (P1Active)
                {
                    GameObject.Destroy(P1Active.gameObject);
                }
                //P2Preview.GetComponent<Image>().sprite = RRSprite;
                P1Active = GameObject.Instantiate(RRPrefab, new Vector3(P1Preview.transform.position.x, P1Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P1Active.preview = true;
                P1Active.GetComponent<PlayerInput>().DeactivateInput();
                P1Text.gameObject.SetActive(true);
                P1Text.text = "Runeborn Ranger";
            }
            if (whichPlayer == "P2" && P2Set == false)
            {
                if (P2Active)
                {
                    GameObject.Destroy(P2Active.gameObject);
                }
                //P2Preview.GetComponent<Image>().sprite = RRSprite;
                P2Active = GameObject.Instantiate(RRPrefab, new Vector3(P2Preview.transform.position.x, P2Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P2Active.preview = true;
                P2Active.GetComponent<PlayerInput>().DeactivateInput();
                P2Text.gameObject.SetActive(true);
                P2Text.text = "Runeborn Ranger";
            }
            if (whichPlayer == "P3" && P3Set == false)
            {
                if (P3Active)
                {
                    GameObject.Destroy(P3Active.gameObject);
                }
                //P2Preview.GetComponent<Image>().sprite = RRSprite;
                P3Active = GameObject.Instantiate(RRPrefab, new Vector3(P3Preview.transform.position.x, P3Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P3Active.preview = true;
                P3Active.GetComponent<PlayerInput>().DeactivateInput();
                P3Text.gameObject.SetActive(true);
                P3Text.text = "Runeborn Ranger";
            }
            if (whichPlayer == "P4" && P4Set == false)
            {
                if (P4Active)
                {
                    GameObject.Destroy(P4Active.gameObject);
                }
                //P2Preview.GetComponent<Image>().sprite = RRSprite;
                P4Active = GameObject.Instantiate(RRPrefab, new Vector3(P4Preview.transform.position.x, P4Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P4Active.preview = true;
                P4Active.GetComponent<PlayerInput>().DeactivateInput();
                P4Text.gameObject.SetActive(true);
                P4Text.text = "Runeborn Ranger";
            }
        }
        if (conqName == "RagingOrc")
        {
            if (whichPlayer == "P1" && P1Set == false)
            {

                if (P1Active)
                {
                    GameObject.Destroy(P1Active.gameObject);
                }
                P1Active = GameObject.Instantiate(ROPrefab, new Vector3(P1Preview.transform.position.x, P1Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P1Active.preview = true;
                P1Active.GetComponent<PlayerInput>().DeactivateInput();
                P1Text.text = "Raging Orc";
                P1Text.gameObject.SetActive(true);
            }
            if (whichPlayer == "P2" && P2Set == false)
            {
                if (P2Active)
                {
                    GameObject.Destroy(P2Active.gameObject);
                }
                P2Active = GameObject.Instantiate(ROPrefab, new Vector3(P2Preview.transform.position.x, P2Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P2Active.preview = true;
                P2Active.GetComponent<PlayerInput>().DeactivateInput();
                P2Text.text = "Raging Orc";
                P2Text.gameObject.SetActive(true);
            }
            if (whichPlayer == "P3" && P3Set == false)
            {
                if (P3Active)
                {
                    GameObject.Destroy(P3Active.gameObject);
                }
                P3Active = GameObject.Instantiate(ROPrefab, new Vector3(P3Preview.transform.position.x, P3Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P3Active.preview = true;
                P3Active.GetComponent<PlayerInput>().DeactivateInput();
                P3Text.text = "Raging Orc";
                P3Text.gameObject.SetActive(true);
            }
            if (whichPlayer == "P4" && P4Set == false)
            {
                if (P4Active)
                {
                    GameObject.Destroy(P4Active.gameObject);
                }
                P4Active = GameObject.Instantiate(ROPrefab, new Vector3(P4Preview.transform.position.x, P4Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P4Active.preview = true;
                P4Active.GetComponent<PlayerInput>().DeactivateInput();
                P4Text.text = "Raging Orc";
                P4Text.gameObject.SetActive(true);
            }

        }
    }

    public void PreviewThisConq(string conqName)
    {
        if (conqName == "BurningBelligerent")
        {
            if (currentSelector == "P1" && P1Set == false)
            {

                if (P1Active)
                {
                    GameObject.Destroy(P1Active.gameObject);
                }
                P1Active = GameObject.Instantiate(BBPrefab, new Vector3(P1Preview.transform.position.x, P1Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P1Active.preview = true;
                P1Text.text = "Heavy";
                P1Text.gameObject.SetActive(true);
            }
            if (currentSelector == "P2" && P2Set == false)
            {
                if (P2Active)
                {
                    GameObject.Destroy(P2Active.gameObject);
                }
                P2Active = GameObject.Instantiate(BBPrefab, new Vector3(P2Preview.transform.position.x, P2Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P2Active.preview = true;
                P2Text.text = "Heavy";
                P2Text.gameObject.SetActive(true);
            }
            if (currentSelector == "P3" && P3Set == false)
            {
                if (P3Active)
                {
                    GameObject.Destroy(P3Active.gameObject);
                }
                //P3Preview.GetComponent<Image>().sprite = DPSprite;
                P3Active = GameObject.Instantiate(BBPrefab, new Vector3(P3Preview.transform.position.x, P3Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P3Active.preview = true;
                P3Text.text = "Heavy";
                P3Text.gameObject.SetActive(true);
            }
            if (currentSelector == "P4" && P4Set == false)
            {
                if (P4Active)
                {
                    GameObject.Destroy(P4Active.gameObject);
                }
                //P3Preview.GetComponent<Image>().sprite = DPSprite;
                P4Active = GameObject.Instantiate(BBPrefab, new Vector3(P4Preview.transform.position.x, P4Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P4Active.preview = true;
                P4Text.text = "Heavy";
                P4Text.gameObject.SetActive(true);
            }

        }
        if (conqName == "DeathsProxy")
        {
            if (currentSelector == "P1" && P1Set == false)
            {
                if (P1Active)
                {
                    GameObject.Destroy(P1Active.gameObject);
                }
                //P3Preview.GetComponent<Image>().sprite = DPSprite;
                P1Active = GameObject.Instantiate(DPPrefab, new Vector3(P1Preview.transform.position.x, P1Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P1Active.preview = true;
                P1Text.text = "Acrobat";
                //P1Preview.GetComponent<Image>().gameObject.SetActive(true);
                P1Text.gameObject.SetActive(true);
            }
            if (currentSelector == "P2" && P2Set == false)
            {
                if (P2Active)
                {
                    GameObject.Destroy(P2Active.gameObject);
                }
                //P2Preview.GetComponent<Image>().sprite = RRSprite;
                P2Active = GameObject.Instantiate(DPPrefab, new Vector3(P2Preview.transform.position.x, P2Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P2Active.preview = true;
                P2Text.gameObject.SetActive(true);
                P2Text.text = "Acrobat";
            }
            if (currentSelector == "P3" && P3Set == false)
            {
                if (P3Active)
                {
                    GameObject.Destroy(P3Active.gameObject);
                }
                //P2Preview.GetComponent<Image>().sprite = RRSprite;
                P3Active = GameObject.Instantiate(DPPrefab, new Vector3(P3Preview.transform.position.x, P3Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P3Active.preview = true;
                P3Text.gameObject.SetActive(true);
                P3Text.text = "Acrobat";
            }
            if (currentSelector == "P4" && P4Set == false)
            {
                if (P4Active)
                {
                    GameObject.Destroy(P4Active.gameObject);
                }
                //P2Preview.GetComponent<Image>().sprite = RRSprite;
                P4Active = GameObject.Instantiate(DPPrefab, new Vector3(P4Preview.transform.position.x, P4Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                P4Active.preview = true;
                P4Text.gameObject.SetActive(true);
                P4Text.text = "Acrobat";
            }
        }
        if (conqName == "Lumiknight")
        {
            if (currentSelector == "P1" && P1Set == false)
            {
                if (P1Active)
                {
                    GameObject.Destroy(P1Active);
                }
                //P3Preview.GetComponent<Image>().sprite = DPSprite;
                P1Text.text = "Fighter/Support";
                P1Preview.GetComponent<Image>().gameObject.SetActive(true);
                P1Text.gameObject.SetActive(true);
            }
            if (currentSelector == "P2" && P2Set == false)
            {
                P2Preview.GetComponent<Image>().gameObject.SetActive(true);
                P2Text.gameObject.SetActive(true);
                P2Preview.GetComponent<Image>().sprite = LKSprite;
                P2Text.text = "Fighter/Support";
            }
            if (currentSelector == "P3" && P3Set == false)
            {
                P3Preview.GetComponent<Image>().sprite = LKSprite;
                P3Text.text = "Fighter/Support";
                P3Preview.GetComponent<Image>().gameObject.SetActive(true);
                P3Text.gameObject.SetActive(true);
            }
            if (currentSelector == "P4" && P4Set == false)
            {
                P4Preview.GetComponent<Image>().sprite = LKSprite;
                P4Text.text = "Fighter/Support";
                P4Preview.GetComponent<Image>().gameObject.SetActive(true);
                P4Text.gameObject.SetActive(true);
            }
        }
        if (conqName == "RunebornRanger")
        {
            if (currentSelector == "P1" && P1Set == false)
            {
                if (P1Active)
                {
                    GameObject.Destroy(P1Active.gameObject);
                }
                //P2Preview.GetComponent<Image>().sprite = RRSprite;
                P1Active = GameObject.Instantiate(RRPrefab, new Vector3(P1Preview.transform.position.x, P1Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                //P1Active.preview = true;
                P1Text.gameObject.SetActive(true);
                P1Text.text = "Sharpshooter/Caster";
            }
            if (currentSelector == "P2" && P2Set == false)
            {
                if (P2Active)
                {
                    GameObject.Destroy(P2Active.gameObject);
                }
                //P2Preview.GetComponent<Image>().sprite = RRSprite;
                P2Active = GameObject.Instantiate(RRPrefab, new Vector3(P2Preview.transform.position.x, P2Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                //P2Active.preview = true;
                P2Text.gameObject.SetActive(true);
                P2Text.text = "Sharpshooter/Caster";
            }
            if (currentSelector == "P3" && P3Set == false)
            {
                if (P3Active)
                {
                    GameObject.Destroy(P3Active.gameObject);
                }
                //P2Preview.GetComponent<Image>().sprite = RRSprite;
                P3Active = GameObject.Instantiate(RRPrefab, new Vector3(P3Preview.transform.position.x, P3Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                //P3Active.preview = true;
                P3Text.gameObject.SetActive(true);
                P3Text.text = "Sharpshooter/Caster";
            }
            if (currentSelector == "P4" && P4Set == false)
            {
                if (P4Active)
                {
                    GameObject.Destroy(P4Active.gameObject);
                }
                //P2Preview.GetComponent<Image>().sprite = RRSprite;
                P4Active = GameObject.Instantiate(RRPrefab, new Vector3(P4Preview.transform.position.x, P4Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                //P4Active.preview = true;
                P4Text.gameObject.SetActive(true);
                P4Text.text = "Sharpshooter/Caster";
            }
        }
        if (conqName == "RagingOrc")
        {
            if (currentSelector == "P1" && P1Set == false)
            {

                if (P1Active)
                {
                    GameObject.Destroy(P1Active.gameObject);
                }
                //P3Preview.GetComponent<Image>().sprite = DPSprite;
                //P1Active = GameObject.Instantiate(BBPrefab, new Vector3(P1Preview.transform.position.x, P1Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                //P1Active.preview = true;
                P1Text.text = "Brawler";
                P1Text.gameObject.SetActive(true);
            }
            if (currentSelector == "P2" && P2Set == false)
            {
                if (P2Active)
                {
                    GameObject.Destroy(P2Active.gameObject);
                }
                P2Preview.GetComponent<Image>().sprite = DPSprite;
                //P2Active = GameObject.Instantiate(BBPrefab, new Vector3(P2Preview.transform.position.x, P2Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                //P2Active.preview = true;
                P2Text.text = "Brawler";
                P2Text.gameObject.SetActive(true);
            }
            if (currentSelector == "P3" && P3Set == false)
            {
                if (P3Active)
                {
                    GameObject.Destroy(P3Active.gameObject);
                }
                //P3Preview.GetComponent<Image>().sprite = DPSprite;
                //P3Active = GameObject.Instantiate(BBPrefab, new Vector3(P3Preview.transform.position.x, P3Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                //P3Active.preview = true;
                P3Text.text = "Brawler";
                P3Text.gameObject.SetActive(true);
            }
            if (currentSelector == "P4" && P4Set == false)
            {
                if (P4Active)
                {
                    GameObject.Destroy(P4Active.gameObject);
                }
                //P3Preview.GetComponent<Image>().sprite = DPSprite;
                //P4Active = GameObject.Instantiate(BBPrefab, new Vector3(P4Preview.transform.position.x, P4Preview.transform.position.y, 0f), new Quaternion(0, 0, 0, 0));
                //P4Active.preview = true;
                P4Text.text = "Brawler";
                P4Text.gameObject.SetActive(true);
            }

        }
    }

    public void ClearConq(string whichPlayer)
    {
            if (whichPlayer == "P1" && P1Active)
            {
            GameObject.Destroy(P1Active.gameObject);
            P1Text.text = "";
        }
            if (whichPlayer == "P2" && P2Active)
            {
            GameObject.Destroy(P2Active.gameObject);
            P2Text.text = "";
        }
            if (whichPlayer == "P3" && P3Active)
            {
            GameObject.Destroy(P3Active.gameObject);
            P3Text.text = "";
        }
            if (whichPlayer == "P4" && P4Active)
            {
            GameObject.Destroy(P4Active.gameObject);
            P4Text.text = "";
        }

    }

}
