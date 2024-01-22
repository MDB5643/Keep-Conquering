using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MenuEventsExtra : MonoBehaviour
{
    public MenuEvents PrimaryCanvas;
    string currentSelector = "P2";

    private void Update()
    {

    }

    public void LoadLevel()
    {

    }

    public void SetMode(int selectedMode)
    {
        
    }

    public void SetConqueror(int conq)
    {
        PrimaryCanvas.SetConqueror(conq, currentSelector);
    }

    public void Awake()
    {
        //StartButton.GetComponent<Button>().interactable = false;
    }

    public void changeSelector (string selector)
    {
        currentSelector = selector;
    }

    public void PreviewConq(string conqName)
    {
        PrimaryCanvas.PreviewConq(conqName, currentSelector);
    }


}
