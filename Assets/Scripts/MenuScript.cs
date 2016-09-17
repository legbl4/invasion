using Assets.W.Scripts;
using Assets.W.Types;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    public void OnExit()
    {
        Application.Quit();
    }

    public void OnSinglePlayer()
    {
        SceneManager.LoadScene("SPMenu");
    }

    public void OnBack()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnStartSingle(int difficult)
    {
        Difficult gameDifficult = Difficult.Easy;
        switch (difficult)
        {
            case 1:
                gameDifficult = Difficult.Medium;
                break;
            case 2: 
                gameDifficult = Difficult.Hard;
                break;
            default:
                gameDifficult = Difficult.Easy;
                break;
        }

        GameManager.Difficult = gameDifficult;
        GameManager.GameMode = GameMode.SinglePlayer;
        GameManager.IsHost = true;
        GameManager.IsInitialized = false; 
        SceneManager.LoadScene("Game");
    }

    public void OnMultiplayer()
    {
        SceneManager.LoadScene("MPMenu");
    }

    public void OnBackToScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void OnSingleRestart()
    {
        OnStartSingle((int)GameManager.Difficult);
    }

    
}
