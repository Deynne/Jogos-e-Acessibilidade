using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialStreetFighter : MonoBehaviour
{
    private AudioSource audioDescription;
    public SceneChanger sceneChanger;
    void Start() {
        audioDescription = gameObject.GetComponentInParent(typeof(AudioSource)) as AudioSource;
        if(audioDescription == null)
            throw new NullReferenceException("O componente de reprodução de audio não foi encontrado no objeto " + gameObject.transform.parent + ".");
    }
    // Start is called before the first frame update
    public void PlayAndLoad(AudioClip tutorial) {
        if(PlayerPrefs.GetInt("playedStreetFighterTutorial",0) < 1) {
            StartCoroutine(Play(tutorial));
        }else {
            sceneChanger.LoadGame("StreetFighter_game");
        }
    }

    private IEnumerator Play(AudioClip tutorial) {
        
        audioDescription.PlayOneShot(tutorial);
        PlayerPrefs.SetInt("playedStreetFighterTutorial",1);
        while(audioDescription.isPlaying) yield return null;       
        sceneChanger.LoadGame("StreetFighter_game");
    }
}
