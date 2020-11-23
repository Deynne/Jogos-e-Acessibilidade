using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneManagerTest : MonoBehaviour
{    
    public string file = "scene.json";

    // TODO GENERALIZAR ISSO
    public void Save()
    {
        // string json = JsonUtility.ToJson(data);
        // WriteToFile(file, json);
    }

    public void Load()
    {
        string json = ReadFromFile(file);
        SceneDataList scenesInJson = JsonUtility.FromJson<SceneDataList>(json);
        Debug.Log(scenesInJson.sceneList.Length);
        foreach (SceneData scene in scenesInJson.sceneList)
        {
            Debug.Log(scene.scenePath);
        }      

        // data = new SceneData();
        // JsonUtility.FromJsonOverwrite(json, data);
        // SceneManager.LoadScene(data.scenePath);
    }

    // GENERALIZAR ISSO
    private void WriteToFile(string fileName, string json)
    {
        // string path = GetFilePath(fileName);
        // FileStream fileStream = new FileStream(path, FileMode.Create);

        // using(StreamWriter writer = new StreamWriter(fileStream))
        // {
        //     writer.Write(json);
        // }
    }

    private string ReadFromFile(string fileName)
    {
        string path = GetFilePath(fileName);
        if(File.Exists(path))
        {
            string json_contents = File.ReadAllText(path);
            return json_contents;
        }
        else
        {
            Debug.LogWarning("File not found!");
            return "";
        }
    }

    private string GetFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    private void OnApplicationPause(bool pauseStatus) {
        // for mobile devices
        if (pauseStatus)
            Save();
    }
}