using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* A class to work with UI */
public class UIHandler : MonoBehaviour
{
    public Text ScoresCount;
    public Text JumpsCount;

    public GameObject ScoresPanel;
    public GameObject JumpsPanel;

    public GameObject GameOverText;

    static bool isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    /* Methods to refresh UI numbers */
    public void RefreshScoreCount(int value)
    {
        ScoresCount.text = value.ToString();
    }
    public void RefreshJumpsCount(int value) 
    {
        JumpsCount.text = value.ToString();
    }
    public void ToggleGameOverScreen()
    {
        isGameOver = !isGameOver;
        RefreshJumpsCount(3);
        RefreshScoreCount(0);

        ScoresPanel.SetActive(!isGameOver);
        JumpsPanel.SetActive(!isGameOver);
        GameOverText.SetActive(isGameOver);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
