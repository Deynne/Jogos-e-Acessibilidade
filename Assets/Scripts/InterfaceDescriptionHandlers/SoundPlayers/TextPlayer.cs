using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TextPlayer : Singleton<TextPlayer> {

    private AudioSource left,right;

    public static string _DESCRIPTION_PATH = "Sound/AudioDescription/";

    public static string _SONS_GENERICOS = DESCRIPTION_PATH + "GenericText/";

    public static string _SONS_NUMEROS = DESCRIPTION_PATH + "Numbers/";

    public static string _SONS_GAMES = DESCRIPTION_PATH + "GamesDescription/";

    public static string DESCRIPTION_PATH {get => _DESCRIPTION_PATH;}

    public static string SONS_GENERICOS {get => _SONS_GENERICOS;}

    public static string SONS_NUMEROS  {get => _SONS_NUMEROS;}

    public static string SONS_GAMES  {get => _SONS_GAMES;}

    private Coroutine corrotina;

    private List<AudioClip> clipsToPlay;

    private bool tocandoSequencia = false;

    private bool _forcedToStop = false;
    public bool ForcedToStop {get =>_forcedToStop;}
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

        clipsToPlay = new List<AudioClip>();

    }

    public bool SourcesPlaying() {
        return left.isPlaying || right.isPlaying || tocandoSequencia;
    }

    public void StopAudio() {
        _forcedToStop = true;
        if(SourcesPlaying()) {
            left.Stop();
            right.Stop();
        }
        if(corrotina != null) {
            Debug.Log("Entrou aqui");
            StopCoroutine(corrotina);
            corrotina = null;
            clipsToPlay.Clear();
            tocandoSequencia = false;
        }
    }
    // public void PlayOnce(AudioClip audio) {
    //     if(!tocandoSequencia) return;
    //     if (left.isPlaying || right.isPlaying) {
    //         left.Stop();
    //         right.Stop();
    //     }
    //     left.PlayOneShot(audio);
    //     right.PlayOneShot(audio);
    // }
    public void playInSequence(params AudioClip [] clips)
    {
        StopAudio();
        Debug.Log("passou por aqui");
        clipsToPlay.AddRange(clips);
        tocandoSequencia = true;
        _forcedToStop = false;;
        corrotina = StartCoroutine(playAudioSequentially());
    }

    IEnumerator playAudioSequentially()
    {
        if (left.isPlaying || right.isPlaying) {
            left.Stop();
            right.Stop();
        }
        // yield return null;
        
        //1.Loop through each AudioClip
        while (clipsToPlay.Count != 0)
        {
            // Debug.Log(clipsToPlay[i]);
            //2.Assign current AudioClip to audiosource
            left.clip = clipsToPlay[0];
            right.clip = clipsToPlay[0];

            //3.Play Audio
            left.Play();
            right.Play();

            //4.Wait for it to finish playing
            while (left.isPlaying)
            {
                // Se tocar algum audio fora do que está programado. Para toda a corrotina.
                if(left.clip != clipsToPlay[0])
                    yield break;
                yield return null;
                
            }
            clipsToPlay.RemoveAt(0);

            yield return new WaitForSeconds(0.1f);   
            //5. Go back to #2 and play the next audio in the adClips array
        }
        tocandoSequencia = false;
        yield break;
    }


    public void addToEndOfSequence(params AudioClip [] clips) {
        if(SourcesPlaying()){
            clipsToPlay.AddRange(clips);
        }
        else
            playInSequence(clips);

    }
}