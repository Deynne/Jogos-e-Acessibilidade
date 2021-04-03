using UnityEngine;
using System;
using System.Collections;

public class TextPlayer : MonoBehaviour {

    private AudioSource left,right;

    public static string _DESCRIPTION_PATH = "Sound/AudioDescription/";

    public static string _SONS_GENERICOS = DESCRIPTION_PATH + "GenericText/";

    public static string _SONS_NUMEROS = DESCRIPTION_PATH + "Numbers/";

    public static string _SONS_GAMES = DESCRIPTION_PATH + "GamesDescription/";

    public static string DESCRIPTION_PATH {get => _DESCRIPTION_PATH;}

    public static string SONS_GENERICOS {get => _SONS_GENERICOS;}

    public static string SONS_NUMEROS  {get => _SONS_NUMEROS;}

    public static string SONS_GAMES  {get => _SONS_GAMES;}

    private void Start() {        
        GameObject g = GameObject.Find("SoundHandler");
        Component [] audioDescriptions = g.GetComponentsInParent(typeof(AudioSource)) as Component[];
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

    public void playInSequence(params AudioClip [] clips)
    {
        StartCoroutine(playAudioSequentially(clips));
    }

    IEnumerator playAudioSequentially(params AudioClip [] clips)
    {
        yield return null;

        //1.Loop through each AudioClip
        for (int i = 0; i < clips.Length; i++)
        {
            Debug.Log(clips[i]);
            //2.Assign current AudioClip to audiosource
            left.clip = clips[i];
            right.clip = clips[i];

            //3.Play Audio
            left.Play();
            right.Play();

            //4.Wait for it to finish playing
            while (left.isPlaying)
            {
                if(left.clip != clips[i])
                    break;
                yield return null;
                
            }

            yield return new WaitForSeconds(0.1f);   
            //5. Go back to #2 and play the next audio in the adClips array
        }
    }    
}