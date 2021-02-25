using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.IO;

[ExecuteInEditMode]
public class SceneListWriter : MonoBehaviour
{
    // Nome do arquivo com as referencias
    public const string jsonName = "sceneReference.json";

    // Regex utilizado para filtrar, dentro dos caminhos, o nome a scene principal de um jogo.
    // Os nomes das scenes principais devem seguir o padrão <caminho>/_<nome>.unity
    private const string regex = "/_.*\\.";
    public const string audioDescriptionFolder = "Sound/AudioDescription/GamesDescription/";

    // Start is called before the first frame update
    void Update()
    {
        if(Application.isEditor) {
            SaveScenesReference(CreateSceneReference());
        }
    }

    private void Awake() {
        if(Application.isEditor) {
            SaveScenesReference(CreateSceneReference());
        }
    }

    private string CreateSceneReference() {
        
        SceneDataList sceneDataList = new SceneDataList();
        string path;
        string name;
        for(int i = 0; i < SceneManager.sceneCountInBuildSettings;i++) {
            path = SceneUtility.GetScenePathByBuildIndex(i);

            if (Regex.IsMatch(path,regex)) {

                name = Regex.Match(path,regex).Value;
                path = path.Substring(7,path.Length-7-6);
                sceneDataList.list.Add(new SceneData(path, NameFilter(name), audioDescriptionFolder + name.Substring(1,name.Length-2)));
            }
        }
        return JsonUtility.ToJson(sceneDataList);
    }

    private void SaveScenesReference(string json) {
        if(Directory.Exists(Application.streamingAssetsPath)) {
            using(StreamWriter sw = new StreamWriter(Application.streamingAssetsPath + "/" + jsonName)) {
                sw.Write(json);
                Debug.LogWarning("Arquivo com referência de cenas foi atualizado.");
            }
        }
        else {
            Debug.LogWarning("A pasta StreamingAssetsPath não existe e o arquivo com a lista de cenas não pôde ser criado.");
        }
    }

    private string NameFilter(string name) {
        string s = name.Substring(2,name.Length-3);
        return Regex.Replace(s, "(\\B[A-Z])", " $1");
    }
}
