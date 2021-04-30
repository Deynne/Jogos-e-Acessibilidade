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
        string json = null;
        Debug.Log(path);
        if (Application.platform == RuntimePlatform.Android || true) {
            UnityWebRequest reader = UnityWebRequest.Get(path);
            
            reader.SendWebRequest();

            while (!reader.isDone) {
                if (reader.isNetworkError || reader.isHttpError) {
                    Debug.Log("net" + reader.isNetworkError);
                    Debug.Log("http" + reader.isHttpError);
                    break;
                }
             }
            Debug.Log(reader.downloadHandler);
            if(!(reader.isNetworkError && reader.isHttpError)) {
                Debug.Log("Entrou no jsonReader");
                json = reader.downloadHandler.text;
            }
            else {
                Debug.Log("net" + reader.isNetworkError);
                Debug.Log("http" + reader.isHttpError);
            }
        }
        else{
            using(StreamReader sr = new StreamReader(path)) {
                json = sr.ReadToEnd();
            }
        }
        return JsonUtility.FromJson<SceneDataList>(json) as SceneDataList;
    }
}
