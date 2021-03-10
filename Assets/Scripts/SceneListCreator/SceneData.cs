using UnityEngine;
using System;
// Armazena os dados relativos as scenes principais de cada jogo e a descrição de audio do nome destes jogos
[Serializable]
public class SceneData {
    [SerializeField]
    private string _scenePath;
    // [SerializeField] private int score;
    // [SerializeField] private bool _hasScore;
    [SerializeField]
    private string _clipPath;
    [SerializeField]
    private string _sceneName;

    // public int highestScore {get => score; set {_hasScore = true; score = value;}}
    public string clipPath {get => _clipPath;}
    public string scenePath {get => _scenePath;}
    public string sceneName {get => _sceneName;}
    // public bool hasScore {get => _hasScore;}

    public SceneData(string scenePath, string name, string clipPath) {
        _scenePath = scenePath;
        // _hasScore = true;
        // this.score = score;
        _clipPath = clipPath;
        _sceneName = name;
    }

}

