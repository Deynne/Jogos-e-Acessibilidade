using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SceneDataList {

    [SerializeField] private List<SceneData> sceneList;

    public List<SceneData> list {get => sceneList;}
    public SceneDataList() {
        sceneList = new List<SceneData>();
    }
    
}
