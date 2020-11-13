using UnityEngine;

// Singleton genérico utilizado para gerar outros tipos de Singleton
public class Singleton<T> : MonoBehaviour  where T : MonoBehaviour {

    // A instância do singleton que será utilizado. A instância não deve ser alterada por
    // elementos externos
    protected static T _instance;

    // Controla o acesso em situações onde o objeto está em processo de ser destruído ou ja foi destruído
    private static bool shuttingDown;

    public static T instance { 
        get {
            // Verifica se o obejto foi destruído e avisa caso tenha acontecido
            if(shuttingDown) {
                Debug.LogWarning("A instância do objeto " + typeof(T) + " já foi destruída");
                return null;
            }
            // Verifica se já há, em algum lugar, uma instância do singleton
            if(_instance == null) {
                _instance = (T) FindObjectOfType(typeof(T));
                // Caso não exista, cria um objeto e liga-a a singleton a ele
                // O objeto não deve ser destruído 
                if(_instance == null) {
                    // Para melhor identificação o nome do objeto é definido como o da
                    // Singleton
                    GameObject go = new GameObject(typeof(T).ToString() + " (Singleton)");
                    
                    _instance = (T) go.AddComponent(typeof(T));
                    DontDestroyOnLoad(go);    
                }
                
            }
            return _instance;
            }
    }
    
    protected Singleton() {
        shuttingDown = false;
    }

    private void OnApplicationQuit() {
        shuttingDown = true;
    }

    private void OnDestroy() {
        shuttingDown = true;
    }
}