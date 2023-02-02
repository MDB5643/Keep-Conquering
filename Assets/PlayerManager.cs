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

    public Text p1DamageDisplay;
    public Text p2DamageDisplay;
    public Text p3DamageDisplay;
    public Text p4DamageDisplay;

    public Text infoText;

    private void Awake()
    {
        try
        {
            characterIndex = 0;
            var player1 = Instantiate(playerPrefabs[MenuEvents.P1Select], new Vector3(78, 5, 0),
                Quaternion.identity);

            var cam = mainCam.GetComponent<Camera2DFollow>();

            if (player1.GetComponent<PrototypeHero>() != null)
            {
                player1.GetComponent<PrototypeHero>().m_DamageDisplay = p1DamageDisplay;
                player1.GetComponent<PrototypeHero>().infoText = infoText;
            }
            else if (player1.GetComponent<TheChampion>() != null)
            {
                player1.GetComponent<TheChampion>().m_DamageDisplay = p1DamageDisplay;
                player1.GetComponent<TheChampion>().infoText = infoText;
            }

            mainCam.GetComponent<Camera2DFollow>().target = player1.transform;
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
        
    }
}
