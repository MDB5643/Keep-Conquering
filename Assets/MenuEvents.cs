using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuEvents : MonoBehaviour
{
    public static int P1Select = 0;
    public static int P2Select;
    public static int P3Select;
    public static int P4Select;

    public PrototypeHero proto;
    public Vector3 Player1Spawn = new Vector3(0, 1, 0);
    public void LoadLevel(int selectedConqueror)
    {
        P1Select = selectedConqueror;

        SceneManager.LoadScene(1);
    }
}
