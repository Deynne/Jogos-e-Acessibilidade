using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.IO;

// Função para criação da lista de jogos e descrições de seus nomes.
// Devido a limitações tecnológicas do Unity só é possível fazer o carregamento dinâmico da lista de jogos
// Desta forma.
// A lista é criada através de uma Scene de carregamento que deve ser aberta sempre que um jogo é adcionado.
// A scene em sí não deve ser incluída no jogo.
// Uma vez montada a lista, ela será salva num arquivo na pasta de streamingAssets do projeto.
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

    // A partir da lista de scenas no build do jogo, cria uma lista e recupera o caminho da descrição de audio
    // que corresponde ao nome do jogo.
    // Alguns padrões devem ser seguidos para que isto seja feito corretamente.
    // Primeiramente, a primeira scene do jogo deve ser iniciada com "_"
    // A descrição de audio deve esta na pasta definida na variável audioDescriptionFolder e deve ter o mesmo nome
    // que o scene principal do jogo, mas sem o "_"
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

    // Uma vez criado o json com os dados, salva ele no arquivo em StreamingAssets com o nome presentem em jsonName
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
    // Remove os caracteres "_" e "." do nome da palavra e insere um espaço em branco
    private string NameFilter(string name) {
        string s = name.Substring(2,name.Length-3);
        return Regex.Replace(s, "(\\B[A-Z])", " $1");
    }
}
