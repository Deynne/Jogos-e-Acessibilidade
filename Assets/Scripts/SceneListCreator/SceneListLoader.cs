using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class SceneListLoader : MonoBehaviour
{
    // Uma função para carregar a lista
    public static SceneDataList LoadSceneList() {
        string path = Application.streamingAssetsPath + "/" + SceneListWriter.jsonName;
        string json;
        if (Application.platform == RuntimePlatform.Android || true) {
            UnityWebRequest reader = new UnityWebRequest(path);
            
            reader.SendWebRequest();

            while (!reader.isDone) { }
     
            json = reader.downloadHandler.text;
        }
        else{
            using(StreamReader sr = new StreamReader(path)) {
                json = sr.ReadToEnd();
            }
        }
        return JsonUtility.FromJson<SceneDataList>(json) as SceneDataList;
    }
}
