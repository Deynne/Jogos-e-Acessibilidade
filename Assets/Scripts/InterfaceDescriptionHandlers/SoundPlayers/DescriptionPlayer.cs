using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class DescriptionPlayer : MonoBehaviour, DescriptionEventHandler
{
    private static AudioSource left,right;
    public AudioClip audioClip;
    private static AudioClip currentClip;
    private static bool _playingTutorial;

    public static bool playingTutorial {get => _playingTutorial;}


    private void OnEnable() {
        SetVariables();
    }

    private void SetVariables() {
        if(audioClip == null) 
            throw new NullReferenceException("A descrição de audio para " + this + " não foi atribuída.");
        Component [] audioDescriptions = gameObject.GetComponentsInParent(typeof(AudioSource)) as Component[];
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
    public void SetVariables(AudioClip ac) {
        if(audioClip == null) 
            audioClip = ac;

        this.SetVariables();
    }

    public void OnDescriptorPress(PointerEventData pointerEventData) {
        if(_playingTutorial)
            return;
        if(left == null || right == null)
            throw new NullReferenceException("O componente de reprodução de audio não foi encontrado no objeto " + gameObject.transform.parent + ".");
        
        if(audioClip == null) 
            throw new NullReferenceException("A descrição de audio para " + this + " não foi atribuída.");
        
        if(CanPlay()) {//!left.isPlaying) {
            left.clip = audioClip;
            right.clip = audioClip;
            left.Play();
            right.Play();
        }
        // else if (left.clip != audioClip) {
        //     left.clip = audioClip;
        //     right.clip = audioClip;
        //     left.Play();
        //     right.Play();
        // }
        
    }

    public bool CanPlay() {
        return !left.isPlaying || left.clip != audioClip;
    }

    public void StopDescription() {
        if(left == null || right == null)
            throw new NullReferenceException("O componente de reprodução de audio não foi encontrado no objeto " + gameObject.transform.parent + ".");
        if(!_playingTutorial && left.isPlaying) {
            left.Stop();
            right.Stop();
        }
            
    }

    public void PlayTutorial(AudioClip tutorial) {
        if(!_playingTutorial && tutorial != null) {
            left.clip = tutorial;
            right.clip = tutorial;
            DescriptionPlayer dp = ListSingleton.instance.focusedGo.GetComponent(typeof(DescriptionPlayer)) as DescriptionPlayer;
            currentClip = dp.audioClip;
            left.Play();
            right.Play();
            _playingTutorial = true;
        }
    }

    // Tocar o audio do primeiro botão após o fim do tutorial
    void Update() {
        if(_playingTutorial && !left.isPlaying) {
            left.clip = currentClip;
            right.clip = currentClip;
            _playingTutorial = false;
            left.Play();
            right.Play();
        }
    }

    public void ChangeVolumeLeft(float volume) {
        left.volume = volume;
        left.Play();
    }
    public void ChangeVolumeRight(float volume) {
        right.volume = volume;
        right.Play();
    }

}
