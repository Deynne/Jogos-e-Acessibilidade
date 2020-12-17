using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
public class Map : MonoBehaviour {

    public List<GameObject> interactables;


    void OnEnable() {
        Map m = GetComponent(typeof(Map)) as Map;
        Debug.Log(m.interactables[0]);
        ListSingleton.instance.UpdateMap(m);
    }
}