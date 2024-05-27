using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    /// <summary>
    /// Start play to load into the game
    /// </summary>
    public void Play()
    {
        //Scenos veikia pagal build index 0-main menu,  1 - main game scene,
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Quit the game 
    /// </summary>
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player has Quit the game");
    }
}
