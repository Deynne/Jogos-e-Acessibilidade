using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Fabrica genérica que será base para as outras fábricas.
public class GenericFactory<T> : MonoBehaviour where T : MonoBehaviour
{
    // prefab do objeto a ser fabricado
    public T prefab;


    public T make(Vector3 position) {
        if(prefab == null) throw new NullReferenceException("o prefab para " + typeof(T).ToString() + " não foi definido.");
        return Instantiate(prefab,position, new Quaternion(0,0,0,0));
    }

    public T make(Vector3 position, Transform parent) {
        if(prefab == null) throw new NullReferenceException("o prefab para " + typeof(T).ToString() + " não foi definido.");
        return Instantiate(prefab,position, new Quaternion(0,0,0,0),parent);
    }

    public T make(Vector3 position,Quaternion rotation) {
        if(prefab == null) throw new NullReferenceException("o prefab para " + typeof(T).ToString() + " não foi definido.");
        return Instantiate(prefab,position,rotation);
    }   

    public T make(Vector3 position,Quaternion rotation, Transform parent) {
        if(prefab == null) throw new NullReferenceException("o prefab para " + typeof(T).ToString() + " não foi definido.");
        return Instantiate(prefab,position,rotation,parent);
    }
}
