using UnityEngine;

// Um singleton que armazena a referência para os objetos com o qual se pode interagir
// Na UI
public class ListSingleton : Singleton<ListSingleton> {

    // Lista que armazena e transita por elementos da interaface. A referência da lista
    // Não deve ser alterada.
    private InteractableList _interactableList;

    public InteractableList interactableList {get=>_interactableList;}

    protected ListSingleton() : base() {
        _interactableList = new InteractableList();
    }

    
}