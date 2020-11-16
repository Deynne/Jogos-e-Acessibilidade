using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
public class InteractableList  {

    // Lista que armazena referência para todos os elementos interativos da interface.
    private List<GameObject> interactableElements;

    // Indice do item atualmente em foco. A ideia é que não seja permitida uma alteração direta do valor do índice.
    private int _currentIndex;

    public int currentIndex {get => _currentIndex;}

    // Retorna o objeto atualmente focado
    public GameObject focusedGo {get => interactableElements[_currentIndex];}

    public InteractableList() {
        _currentIndex = 0;
        interactableElements = new List<GameObject>();
    }

    

    // Movimenta-se para o próximo elemento da lista. Caso se encontre no ultimo elemento, volta para o primeiro
    public GameObject Next() {
        _currentIndex = ++_currentIndex % interactableElements.Count;

        return interactableElements[_currentIndex];
    }

    // Movimenta-se para o elemento na lista. Caso se encontre no primeiro elemento, move-se para o ultimo
    public GameObject Previous() {
        _currentIndex = --_currentIndex < 0?interactableElements.Count-1:_currentIndex;

        return interactableElements[_currentIndex];
    }

    // Retorna um elemento específico da lista
    public GameObject Get(int index) {
        // Se o indice indicado não existir na lista ocasiona em erro
        if(index < 0 || index > interactableElements.Count) {
            throw new IndexOutOfRangeException("Não há elementos em " + typeof(InteractableList) + " correspondendo ao índice " + index + ".");
        }
        
        _currentIndex = index;

        return interactableElements[_currentIndex];
    }


    // Busca pela Scene todos os objetos com a tag Interactables e adiciona na lista
    public void FindInteractables() {
        GameObject [] interactableGoList = GameObject.FindGameObjectsWithTag("Interactable");

        foreach (GameObject go in interactableGoList) {
            interactableElements.Add(go);
        }

    }

    // Permite que a lista seja limpa
    // Pode ser utilizado para permitir elimitação de lixo de memória e repopulação da
    // Lista.
    public void ClearList() {
        interactableElements.Clear();
    }
}