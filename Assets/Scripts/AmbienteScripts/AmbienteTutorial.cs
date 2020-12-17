using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienteTutorial : MonoBehaviour
{

    public DescriptionPlayer descriptionPlayer;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("playedAmbienteTutorial",0) < 1) {
            descriptionPlayer.PlayTutorial(Resources.Load<AudioClip>("Sound/AudioDescription/Tutorials/Ambiente"));
            PlayerPrefs.SetInt("playedAmbienteTutorial",1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
