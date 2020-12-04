using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class DescriptionPlayer : MonoBehaviour, DescriptionEventHandler
{
    private static AudioSource audioDescription;
    public AudioClip audioClip;
    private static AudioClip currentClip;


    private void OnEnable() {
        SetVariables();

    }

    private void SetVariables() {
        if(audioClip == null) 
            throw new NullReferenceException("A descrição de audio para " + this + " não foi atribuída.");
        audioDescription = gameObject.GetComponentInParent(typeof(AudioSource)) as AudioSource;
        if(audioDescription == null)
            throw new NullReferenceException("O componente de reprodução de audio não foi encontrado no objeto " + gameObject.transform.parent + ".");
    }
    public void SetVariables(AudioClip ac) {
        if(audioClip == null) 
            audioClip = ac;

        this.SetVariables();
    }

    public void OnDescriptorPress(PointerEventData pointerEventData) {
        if(audioDescription == null)
            throw new NullReferenceException("O componente de reprodução de audio não foi encontrado no objeto " + gameObject.transform.parent + ".");
        
        if(audioClip == null) 
            throw new NullReferenceException("A descrição de audio para " + this + " não foi atribuída.");

        
        
        if(!audioDescription.isPlaying) {
            audioDescription.clip = audioClip;
            audioDescription.Play();
        }
        else if (audioDescription.clip != audioClip) {
            audioDescription.clip = audioClip;
            audioDescription.Play();
        }
        
    }

    public void StopDescription() {
        if(audioDescription == null)
            throw new NullReferenceException("O componente de reprodução de audio não foi encontrado no objeto " + gameObject.transform.parent + ".");
        audioDescription.Stop();
    }

}
