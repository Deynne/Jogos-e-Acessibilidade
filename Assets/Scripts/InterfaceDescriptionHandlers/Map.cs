using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
public class Map : MonoBehaviour {

    public List<GameObject> interactables;

    public void AddInteractable(GameObject go) {
        if(interactables == null)
            interactables = new List<GameObject>();
        interactables.Add(go);
    }

    void OnEnable() {
        Map m = GetComponent(typeof(Map)) as Map;
        ListSingleton.instance.UpdateMap(m);
    }
}