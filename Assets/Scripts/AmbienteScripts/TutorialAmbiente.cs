using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// Realiza a reprodução inicial do tutorial;
public class TutorialAmbiente : MonoBehaviour
{
    private static AudioSource left,right;
    public AudioClip tutorial;
    // Ao iniciar a Scene, caso não tenha tocado inicialmente, deve tocar o Tutorial do ambiente.
    void Start()
    {
        if(PlayerPrefs.GetInt("playedAmbienteTutorial",0) < 1) {
            PlayTutorial();
        }
    }

    public void PlayTutorial() {
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
            DescriptionPlayer.playingTutorial = true;
            left.clip = tutorial;
            right.clip = tutorial;
            left.Play();
            right.Play();
            
            DescriptionPlayer dp = ListSingleton.instance.focusedGo.GetComponent(typeof(DescriptionPlayer)) as DescriptionPlayer;
            
            if(dp == null)
                throw new NullReferenceException("A descrição de audio para " + ListSingleton.instance.focusedGo + " não foi atribuída.");
           
            DescriptionPlayer.UpdateCurrentClip(dp.audioClip);
            
            // descriptionPlayer.PlayTutorial(Resources.Load<AudioClip>("Sound/AudioDescription/Tutorials/Ambiente"));
            PlayerPrefs.SetInt("playedAmbienteTutorial",1);
    }
}
