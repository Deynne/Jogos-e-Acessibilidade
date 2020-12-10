using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienteTutorial : MonoBehaviour
{

    public DescriptionPlayer descriptionPlayer;
    // Start is called before the first frame update
    void Start()
    {
        descriptionPlayer.PlayTutorial(Resources.Load<AudioClip>("Sound/AudioDescription/Tutorials/Ambiente"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
