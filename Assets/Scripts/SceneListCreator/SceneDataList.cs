using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Uma lista contendo os dados de todos os jogos e as descrições de audio de seus nomes.
[Serializable]
public class SceneDataList {

    [SerializeField] private List<SceneData> sceneList;

    public List<SceneData> list {get => sceneList;}
    public SceneDataList() {
        sceneList = new List<SceneData>();
    }
    
}
