// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class SceneLoader : MonoBehaviour
// {
//     private int actualScene;
//     private int prevScene;
//     private int nextScene;

//     void Start()
//     {
//         actualScene = SceneManager.GetActiveScene().buildIndex;
//         prevScene = actualScene - 1;
//         nextScene = actualScene + 1;
//         Debug.Log("Scene loading: " + actualScene);
//     }

//     public void LoadPrevScene()
//     {
//         Debug.Log("Scene loading: " + prevScene);
//         SceneManager.LoadScene(prevScene);
//     } 

//     public void LoadNextScene()
//     {
//         Debug.Log("Scene loading: " + nextScene);
//         SceneManager.LoadScene(nextScene);
//     } 
// }