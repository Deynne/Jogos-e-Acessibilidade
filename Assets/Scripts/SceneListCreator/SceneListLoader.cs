using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SceneListLoader : MonoBehaviour
{
    // Uma função para carregar a lista
    public static SceneDataList LoadSceneList() {
        using(StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/" + SceneListWriter.jsonName)) {
            return JsonUtility.FromJson<SceneDataList>(sr.ReadToEnd()) as SceneDataList;
        }
    }
}
