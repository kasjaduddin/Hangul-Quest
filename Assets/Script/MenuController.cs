using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void playButton()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void quitButton()
    {
        Application.Quit();
    }
}
