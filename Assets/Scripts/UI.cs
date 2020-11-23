using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public SceneManagerTest sceneManagerTest;

    // Start is called before the first frame update
    void Start()
    {
        sceneManagerTest.Load();
        // SceneManager.LoadScene(prevScene);
        // myPrefab.
        // Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
