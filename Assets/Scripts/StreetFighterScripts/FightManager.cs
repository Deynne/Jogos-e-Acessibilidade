using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum Move {UP, DOWN, LEFT, RIGHT}
public class Fighter {
    #pragma warning disable CS0649
    public bool player;
    public int games = 0;
    public int hP = 0;

    public Fighter(bool _player, int _hP) {
        player = _player;
        hP = _hP;
    }
}


public class FightManager : MonoBehaviour
{
    //Esta classe inicializa duas listas, uma para os movimentos recorrentes da luta, e outra que abriga os temporários do jogador atual
    public int hpInicial = 3;
    public int gamesToWin = 2;
    int rodadaAtual = 1;
    public List<Move> fightMoves = new List<Move>();
    public List<Move> tempMoves = new List<Move>();
    #pragma warning disable CS0649
    [SerializeField] private ShiftManagementScript shiftManagementScript;
    [SerializeField] private Text playerHP;
    [SerializeField] private Text playerRounds;
    [SerializeField] private Text enemyRounds;
    [SerializeField] private Text enemyHP;
    [SerializeField] private Text lastMove;
    [SerializeField] private Text inputText;
    //Os áudios desejados devem ser atribuídos no editor
    //[SerializeField] private AudioClip[] punchSounds = new AudioClip[4];
    public SceneChanger sceneChanger;
    //[SerializeField] private AudioClip playerDamageSound;
    //[SerializeField] private AudioClip enemyDamageSound;
    //[SerializeField] private AudioClip victoryDescription;
    //[SerializeField] private AudioClip defeatDescription;
    #pragma warning restore CS0649
    public SoundList listaSons;
    public Fighter playerFighter;
    public Fighter enemyFighter;
    public bool suspendMoveCalculation = false;

    private AudioClip audioToPlay = null;

    //No início, os HP dos lutadores são determinados
    private void Start() {        
        GameObject g = GameObject.Find("SoundHandler");
        Component [] audioDescriptions = g.GetComponentsInParent(typeof(AudioSource)) as Component[];
        if(audioDescriptions == null)
            throw new NullReferenceException("O componente de reprodução de audio não foi encontrado no objeto " + gameObject.transform.parent + ".");
        /*AudioSource a;
        for (int i = 0; i < audioDescriptions.Length; i++) {
            a = audioDescriptions[i] as AudioSource;
            if(a.panStereo < 0) {
                left = a;
            } else if(a.panStereo > 0) {
                right = a;
            }
        }*/

        playerFighter = new Fighter(true,hpInicial);
        enemyFighter = new Fighter(false,hpInicial);
        
        UpdateUI();
    }
    //O método PerformMove adiciona golpes à respectiva lista dos golpes
    public IEnumerator PerformMove (Move newMove) {
        
        if(ShiftManagementScript.state == BattleState.PLAYERTURN) {
            inputText.text = "Seu turno.";
        } else if (ShiftManagementScript.state == BattleState.ENEMYTURN) {
            inputText.text = "Turno do inimigo";
        }

        
        
        //Caso a lista temporária seja menor do que a final, adiciona o próximo golpe à temporária
        //e é checado se a sequência atual é correta.
        
        PopUp(lastMove.gameObject, 0.3f);

        if(tempMoves.Count < fightMoves.Count) {
            tempMoves.Add(newMove);
            //O áudio do golpe é tocado uma vez
            
            
            lastMove.text = newMove.ToString();

            //Caso a sequência não esteja correta, o personagem leva dano
            if(!RightSequence()) {
                lastMove.text = ShiftManagementScript.state == BattleState.PLAYERTURN?"Dano recebido!":"Você Acertou!";
                StartCoroutine(TakeDamage());
            }
            else if (tempMoves.Count == fightMoves.Count && ShiftManagementScript.state== BattleState.PLAYERTURN) {
                audioToPlay = listaSons.punchSounds[(int)newMove];
                TextPlayer.instance.playInSequence(audioToPlay);
                while(TextPlayer.instance.SourcesPlaying()) yield return null;
                
                inputText.text = "Faça um movimento novo!";
                audioToPlay = listaSons.facaUmMovimentoNovo;
                TextPlayer.instance.playInSequence(audioToPlay);
                while(TextPlayer.instance.SourcesPlaying()) yield return null;                
            }
            else {
                TextPlayer.instance.playInSequence(listaSons.punchSounds[(int)newMove]);
            }
            
            yield break;
        }
        suspendMoveCalculation = true;
        
        //Se a sequência estiver correta, o golpe é adicionado à lista principal e a lista temporária é zerada
        fightMoves.Add(newMove);
        
        lastMove.text = newMove.ToString();
        //O áudio do golpe é tocado uma vez
        audioToPlay = listaSons.punchSounds[(int)newMove];
        TextPlayer.instance.playInSequence(audioToPlay);
        while(TextPlayer.instance.SourcesPlaying()) yield return null;
        lastMove.text = "";
        tempMoves = new List<Move>();

        
        if(ShiftManagementScript.state == BattleState.ENEMYTURN) {
            
            shiftManagementScript.enemyButton.onClick.Invoke();
            audioToPlay = listaSons.seuInimEncerrouTurno;
            inputText.text = "Seu inimigo encerrou seu turno. Agora é a sua vez!";
            TextPlayer.instance.playInSequence(audioToPlay);
            while(TextPlayer.instance.SourcesPlaying()) yield return null;

            //Caso o turno seja do jogador, invoca-se o botão de mudança de turno para o inimigo
            

        } else if (ShiftManagementScript.state == BattleState.PLAYERTURN) {
            //Caso contrário, invoca-se o botão de mudança de turno para o jogador
            
            shiftManagementScript.playerButton.onClick.Invoke();
            audioToPlay = listaSons.seuTurnoAcabou;
            inputText.text = "Seu turno acabou. Agora é a vez do inimigo.";
            TextPlayer.instance.playInSequence(audioToPlay);
            while(TextPlayer.instance.SourcesPlaying()) yield return null;

        }
        suspendMoveCalculation = false;
        
    }

    //Este método checa as sequências principal e temporária
    public bool RightSequence() {
        
        for(int i = 0; i < tempMoves.Count; i++) {
            if(tempMoves[i] != fightMoves[i]) {
                return false;
            }
        }
        return true;
    }

    //Esta função checa de quem é o turno e remove um de HP do lutador respectivo
    public IEnumerator TakeDamage() {
        suspendMoveCalculation = true;
        //Audio de dano toca.
        if(ShiftManagementScript.state == BattleState.PLAYERTURN) {
            playerFighter.hP--;
            TextPlayer.instance.addToEndOfSequence  (listaSons.playerDamageSound,
                                                     listaSons.danoRecebido);
        } else if(ShiftManagementScript.state == BattleState.ENEMYTURN) {
            enemyFighter.hP--;
            TextPlayer.instance.addToEndOfSequence  (listaSons.enemyDamageSound,
                                                     listaSons.voceAcertou);
        }
        UpdateUI();
        // while(TextPlayer.instance.SourcesPlaying()) yield return null;
        PlayHPCounter(playerFighter);
        // while(TextPlayer.instance.SourcesPlaying()) yield return null;
        PlayHPCounter(enemyFighter);
        while(TextPlayer.instance.SourcesPlaying()) yield return null;

        //As listas são reinicializadas
        fightMoves = new List<Move>();
        tempMoves = new List<Move>();

        FighterAI ai = GameObject.FindObjectOfType<FighterAI>();
        ai.RefreshEnemyHitChance();

        //Caso alguém chegue a 0 de vida, o jogo se encerra
        if(playerFighter.hP == 0 || enemyFighter.hP == 0) {
            StartCoroutine(EndRound());
            yield break;
        }

        if(ShiftManagementScript.state == BattleState.PLAYERTURN) {
            TextPlayer.instance.addToEndOfSequence(listaSons.facaUmMovimentoNovo);
            // while(TextPlayer.instance.SourcesPlaying()) yield return null;
        }else if(ShiftManagementScript.state == BattleState.ENEMYTURN) {
            TextPlayer.instance.addToEndOfSequence(listaSons.seuAdvNovaRodada);
            // while(TextPlayer.instance.SourcesPlaying()) yield return null;
        }
        while(TextPlayer.instance.SourcesPlaying()) yield return null;
        suspendMoveCalculation = false;
    }

    public void PlayHPCounter(Fighter fghtr) {
        if(fghtr.player) {
            if(fghtr.hP != 1) {
                TextPlayer.instance.addToEndOfSequence  (listaSons.voceTem,
                                                        Resources.Load<AudioClip>(TextPlayer.SONS_NUMEROS + fghtr.hP),
                                                        listaSons.pontosDeVida);
            } else {
                TextPlayer.instance.addToEndOfSequence  (listaSons.voceTem,
                                                        Resources.Load<AudioClip>(TextPlayer.SONS_NUMEROS + fghtr.hP),
                                                        listaSons.pontoDeVida);
            }
        } else {
            if(fghtr.hP != 1) {
                TextPlayer.instance.addToEndOfSequence  (listaSons.seuAdvTem,
                                                        Resources.Load<AudioClip>(TextPlayer.SONS_NUMEROS + fghtr.hP),
                                                        listaSons.pontosDeVida);
            } else {
                TextPlayer.instance.addToEndOfSequence  (listaSons.seuAdvTem,
                                                        Resources.Load<AudioClip>(TextPlayer.SONS_NUMEROS + fghtr.hP),
                                                        listaSons.pontoDeVida);
            }
        }

    }

    public IEnumerator PlayGameStatus() {
        suspendMoveCalculation = true;
        TextPlayer.instance.playInSequence  (listaSons.rodada,
                                            Resources.Load<AudioClip>(TextPlayer.SONS_NUMEROS + rodadaAtual.ToString()));
        string previousText = inputText.text;
        inputText.text = "Rodada " + rodadaAtual.ToString();
        
        PlayHPCounter(playerFighter);
        // while(TextPlayer.instance.SourcesPlaying()) yield return null;
        PlayHPCounter(enemyFighter);
        // TextPlayer.instance.addToEndOfSequence(audioToPlay);
        
        while(TextPlayer.instance.SourcesPlaying()) { 
            // if(TextPlayer.instance.ForcedToStop) {
            //     Debug.Log("Interrompido");
            //     inputText.text = previousText;
            //     suspendMoveCalculation = false;
            //     yield break;
            // }
            yield return null;
        }
        inputText.text = previousText;
        suspendMoveCalculation = false;
        yield break;
    }

    //No fim do jogo, o texto da UI é atualizado com a mensagem de vitória ou derrota
    //Além disso, o game do lutador respectivo é contabilizado e as vidas são reiniciadas
    private IEnumerator EndRound() {
        rodadaAtual++;
        suspendMoveCalculation = true;
        if(playerFighter.hP > 0) {
            playerFighter.games++;
            lastMove.text = "Round ganho";
            inputText.text = "Você ganhou a rodada!";
            TextPlayer.instance.addToEndOfSequence  (listaSons.voce,
                                                    listaSons.ganhouARodada);
        } else {
            enemyFighter.games++;
            lastMove.text = "Round perdido";
            inputText.text = "Você perdeu a rodada.";
            TextPlayer.instance.addToEndOfSequence  (listaSons.voce,
                                                    listaSons.perdeuARodada);
        }

        while(TextPlayer.instance.SourcesPlaying()) yield return null;

        if(playerFighter.games == gamesToWin || enemyFighter.games == gamesToWin) {
            StartCoroutine(EndGame());
            UpdateUI();
            yield break;
        }
        
        TextPlayer.instance.addToEndOfSequence  (listaSons.rodada,
                                            Resources.Load<AudioClip>(TextPlayer.SONS_NUMEROS + rodadaAtual.ToString()));
        while(TextPlayer.instance.SourcesPlaying()) yield return null;
        

        if(ShiftManagementScript.state == BattleState.PLAYERTURN) {
            while(TextPlayer.instance.SourcesPlaying()) yield return null;
            TextPlayer.instance.addToEndOfSequence(listaSons.facaUmMovimentoNovo);
        }
        
        playerFighter.hP = hpInicial;
        enemyFighter.hP = hpInicial;
        UpdateUI();
        suspendMoveCalculation = false;
    }
    IEnumerator EndGame() {
        suspendMoveCalculation = true;
        ShiftManagementScript.state = BattleState.END;
        AudioClip description = null;
        if(playerFighter.games == gamesToWin) {
            lastMove.text = ":DDDD";
            inputText.text = "Você ganhou a luta! Parabéns!!!";
            // yield return new WaitForSeconds(1);
            // sceneChanger.LoadGame("_StreetFighter");
            description = listaSons.victoryDescription;
        } else if(enemyFighter.games == gamesToWin) {
            lastMove.text = ":'(";
            inputText.text = "Você perdeu a luta. Mais sorte da próxima vez...";
            description = listaSons.defeatDescription;            
            // yield return new WaitForSeconds(3);
            // sceneChanger.LoadGame("_StreetFighter");

        }
        TextPlayer.instance.addToEndOfSequence(description);
        while(TextPlayer.instance.SourcesPlaying()) yield return null;       
        sceneChanger.LoadGame("_SequenciaDeCombate");

        yield break;
    }

    public void UpdateUI() {
        playerHP.text = "HP: " + playerFighter.hP;
        enemyHP.text = "HP: " + enemyFighter.hP;
        playerRounds.text = "Rounds: " + playerFighter.games;
        enemyRounds.text = "Rounds: " + enemyFighter.games;
    }

    public void PopUp(GameObject obj, float time) {
        if(!LeanTween.isTweening(obj)) {
            LeanTween.scale(obj,obj.transform.localScale * 2,time).setLoopPingPong(1); //Animação do texto
        }
    }

    
}
