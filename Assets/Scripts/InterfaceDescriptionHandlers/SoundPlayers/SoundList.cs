using UnityEngine;

[CreateAssetMenu(fileName = "SoundList", menuName = "desenvolvimento/SoundList", order = 0)]
public class SoundList : ScriptableObject {
    public AudioClip[] punchSounds = new AudioClip[4];
    public AudioClip playerDamageSound;
    public AudioClip enemyDamageSound;
    public AudioClip victoryDescription;
    public AudioClip defeatDescription;
    public AudioClip danoRecebido;
    public AudioClip facaUmMovimentoNovo;
    public AudioClip ganhouARodada;
    public AudioClip inicieABatalha;
    public AudioClip inicieNovaRodada;
    public AudioClip perdeuARodada;
    public AudioClip pontoDeVida;
    public AudioClip pontosDeVida;
    public AudioClip rodada;
    public AudioClip seuAdvNovaRodada;
    public AudioClip seuAdvTem;
    public AudioClip seuInimEncerrouTurno;
    public AudioClip seuTurnoAcabou;
    public AudioClip tentarNovamente;
    public AudioClip voce;
    public AudioClip voceAcertou;
    public AudioClip voceTem;
    public AudioClip voltarPMenu;
}
