using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour {

	public void PlayerVsAI()
    {
        SceneManager.LoadScene(1);
    }

    public void AIvsAI()
    {
        SceneManager.LoadScene(2);
    }

    public void Walking()
    {
        SceneManager.LoadScene(3);
    }

    public void Jumping()
    {
        SceneManager.LoadScene(4);
    }

    public void WalkingAndJumping()
    {
        SceneManager.LoadScene(5);
    }
}
