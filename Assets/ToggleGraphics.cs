using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGraphics : MonoBehaviour
{

    public Toggle toggle;
    public UIViewControl viewControl;
    public AudioClip ativar;
    public AudioClip desativar;
    public DescriptionPlayer descriptionPlayer;

    // Start is called before the first frame update
    void Start()
    {
        toggle.isOn = PlayerPrefs.GetInt("showInterface",1) == 1;
        if(toggle.isOn) {
            descriptionPlayer.audioClip = desativar;
        }
        else {
            descriptionPlayer.audioClip = ativar;
        }
        descriptionPlayer.OnDescriptorPress(null);
    }

    public void ToggleAllGraphics() {
        if(toggle.isOn) {
            descriptionPlayer.audioClip = desativar;
        }
        else {
            descriptionPlayer.audioClip = ativar;
        }
        PlayerPrefs.SetInt("showInterface",toggle.isOn?1:0);
        viewControl.CheckGraphics();
    }
}
