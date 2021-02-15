using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
public class InteractableList  : MonoBehaviour {

    // Lista que armazena referência para todos os elementos interativos da interface.
    [SerializeField]
    private Map _map;

    // Indice do item atualmente em foco. A ideia é que não seja permitida uma alteração direta do valor do índice.
    private int _currentIndex;

    public int currentIndex {get => _currentIndex;}

    // Retorna o objeto atualmente focado
    public GameObject focusedGo {get {
                                        if(_map.interactables.Count == 0){
                                            Debug.LogWarning("Não há elementos na lista de interativos. Você marcou os elementos interativos com a tag interactables?");
                                            return null;
                                        }
                                        return _map.interactables[_currentIndex];}
                                    }

    public bool isEmpty {get => _map == null || _map.interactables.Count == 0?true:false;}

    public InteractableList() {
        _currentIndex = 0;
    }

    

    // Movimenta-se para o próximo elemento da lista. Caso se encontre no ultimo elemento, volta para o primeiro
    public GameObject Next() {
        if(_map.interactables.Count == 0){
            Debug.LogWarning("Não há elementos na lista de interativos. Você marcou os elementos interativos com a tag interactables?");
            return null;
        }
        _currentIndex = ++_currentIndex % _map.interactables.Count;

        return _map.interactables[_currentIndex];
    }

    // Movimenta-se para o elemento na lista. Caso se encontre no primeiro elemento, move-se para o ultimo
    public GameObject Previous() {
        if(_map.interactables.Count == 0){
            Debug.LogWarning("Não há elementos na lista de interativos. Você marcou os elementos interativos com a tag interactables?");
            return null;
        }
        _currentIndex = --_currentIndex < 0?_map.interactables.Count-1:_currentIndex;

        return _map.interactables[_currentIndex];
    }

    // Retorna um elemento específico da lista
    public GameObject Get(int index) {
        if(_map.interactables.Count == 0){
            Debug.LogWarning("Não há elementos na lista de interativos. Você marcou os elementos interativos com a tag interactables?");
            return null;
        }

        // Se o indice indicado não existir na lista ocasiona em erro
        if(index < 0 || index > _map.interactables.Count) {
            throw new IndexOutOfRangeException("Não há elementos em " + typeof(InteractableList) + " correspondendo ao índice " + index + ".");
        }
        
        _currentIndex = index;

        return _map.interactables[_currentIndex];
    }

    public int Find(GameObject go) {
        if(go == null) return -1;
        return _map.interactables.FindIndex(g => {
                                                    if(g == go) return true;
                                                    else {
                                                        Component[] t;
                                                        t = go.GetComponentsInChildren(typeof(Transform));
                                                        
                                                        foreach(Transform tr in t) {
                                                            if(g == tr.gameObject) return true;
                                                        }
                                                    }
                                                    return false;
                                                 });
    }


    // Busca pela Scene todos os objetos com a tag Interactables e adiciona na lista
    // public void FindInteractables() {
    //     GameObject [] interactableGoList = GameObject.FindGameObjectsWithTag("Interactable");

    //     // TODO verificar se é possível definir uma ordem para os objetos
    //     foreach (GameObject go in interactableGoList) {
    //         _map.interactables.Add(go);
    //     }

    // }

    // Permite que a lista seja limpa
    // Pode ser utilizado para permitir elimitação de lixo de memória e repopulação da
    // Lista.
    public void ClearList() {
        if(isEmpty)
            return;
        _map = null;
    }

    public void UpdateMap(Map newInteractablesList) {
        _map = newInteractablesList;
    }

    public void AppendToMap(Map interactableListToApend) {
        Map tempMap = new Map();
        List<GameObject> temp = new List<GameObject>(_map.interactables);
        
        foreach (GameObject go in interactableListToApend.interactables) {
            temp.Add(go);
        }
        
        tempMap.interactables = temp;
        _map = tempMap;
    }
}