using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChanger : MonoBehaviour
{
    // Carrega a scene do ambiente
    // public void LoadHub()
    // {
    //     SceneManager.LoadScene("Ambiente");
    // }

    // // Carrega a scene do StreetFighter
    // public void LoadGame()
    // {
    //     SceneManager.LoadScene("Jogo");
    // }

    // Carrega a scene do StreetFighter
    public void LoadGame(string path)
    {
        if(path.Equals("") || path == null)
            return;
        SceneManager.LoadScene(path);
    }

    // Carrega a scene do Tutorial
    // public void LoadTutorial()
    // {
    //     SceneManager.LoadScene("TutorialJogo");
    // }

    // // Carrega a scene do Lobby
    // public void LoadLobby()
    // {
    //     SceneManager.LoadScene("Lobby");
    // }
}