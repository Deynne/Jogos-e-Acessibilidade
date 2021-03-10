using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        Component [] audioDescriptions = this.gameObject.GetComponentsInParent(typeof(AudioSource)) as Component[];
        if(audioDescriptions == null)
            throw new NullReferenceException("O componente de reprodução de audio não foi encontrado no objeto " + gameObject.transform.parent + ".");
        AudioSource a;
        for (int i = 0; i < audioDescriptions.Length; i++) {
            a = audioDescriptions[i] as AudioSource;
            if(a.panStereo < 0) {
                a.volume = PlayerPrefs.GetFloat("leftVolume",1);
            } else if(a.panStereo > 0) {
                a.volume = PlayerPrefs.GetFloat("rightVolume",1);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
