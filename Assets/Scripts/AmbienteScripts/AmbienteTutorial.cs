using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Realiza a reprodução inicial do tutorial;
public class AmbienteTutorial : MonoBehaviour
{

    public DescriptionPlayer descriptionPlayer;
    // Ao iniciar a Scene, caso não tenha tocado inicialmente, deve tocar o Tutorial do ambiente.
    void Start()
    {
        if(PlayerPrefs.GetInt("playedAmbienteTutorial",0) < 1) {
            descriptionPlayer.PlayTutorial(Resources.Load<AudioClip>("Sound/AudioDescription/Tutorials/Ambiente"));
            PlayerPrefs.SetInt("playedAmbienteTutorial",1);
        }
    }
}
