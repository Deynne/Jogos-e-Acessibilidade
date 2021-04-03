using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialGame : MonoBehaviour
{
    private static AudioSource left,right;
    public SceneChanger sceneChanger;
    public AudioClip tutorial;
    public string nomeCena;
    void Start() {
        GameObject g = GameObject.Find("SoundHandler");
        Component [] audioDescriptions = g.GetComponents(typeof(AudioSource)) as Component[];
        if(audioDescriptions == null)
            throw new NullReferenceException("O componente de reprodução de audio não foi encontrado no objeto " + gameObject.transform.parent + ".");
        AudioSource a;
        for (int i = 0; i < audioDescriptions.Length; i++) {
            a = audioDescriptions[i] as AudioSource;
            if(a.panStereo < 0) {
                left = a;
            } else if(a.panStereo > 0) {
                right = a;
            }
        }
    }
    // Start is called before the first frame update
     /// <param name="deveTocar">Se o tutorial deve tocar obrigatóriamente.</param>
    public void PlayAndLoad(bool deveTocar) {
        if(PlayerPrefs.GetInt("playedStreetFighterTutorial",0) < 1 || deveTocar) {
            StartCoroutine(Play());
        }else {
            sceneChanger.LoadGame(nomeCena);
        }
    }

    private IEnumerator Play() {
        
        left.PlayOneShot(tutorial);
        right.PlayOneShot(tutorial);
        PlayerPrefs.SetInt("playedStreetFighterTutorial",1);
        while(left.isPlaying) yield return null;       
        sceneChanger.LoadGame(nomeCena);
    }
}
