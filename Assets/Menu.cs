using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject menu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            menu.SetActive(!menu.activeInHierarchy);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }
    }

    public void CCDArm()
    {
        SceneManager.LoadScene("SampleScene 1");
    }

    public void CCDSpider()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void FABRIKArm()
    {
        SceneManager.LoadScene("SampleScene 2");
    }
}
