using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
// Mapa contendo rota prevista de interações com a tela. Todo canvas deve ter um mapa.
// Cada canvas deve conter apenas 1 mapa.
public class Map : MonoBehaviour {
    // Lista de objetos interagiveis.
    public List<GameObject> interactables;

    // Permite adicionar interagíveis.
    public void AddInteractable(GameObject go) {
        if(interactables == null)
            interactables = new List<GameObject>();
        interactables.Add(go);
    }

    // Sempre que for habilitado, atualiza a lista de interagíveis com os seus dados.
    void OnEnable() {
        Map m = GetComponent(typeof(Map)) as Map;
        ListSingleton.instance.UpdateMap(m);
    }
}