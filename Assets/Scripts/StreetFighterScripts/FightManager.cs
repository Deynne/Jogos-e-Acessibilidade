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
    [SerializeField] private AudioClip[] punchSounds = new AudioClip[4];
    public SceneChanger sceneChanger;
    [SerializeField] private AudioClip playerDamageSound;
    [SerializeField] private AudioClip enemyDamageSound;
    [SerializeField] private AudioClip victoryDescription;
    [SerializeField] private AudioClip defeatDescription;
    #pragma warning restore CS0649
    public SoundString soundsList;
    public Fighter playerFighter;
    public Fighter enemyFighter;
    public bool suspendMoveCalculation = false;


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

        TextAsset jsn = Resources.Load(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/SequenciaDeCombateDescriptionsIndex") as TextAsset;
        if(jsn != null) {
            soundsList = JsonUtility.FromJson<SoundString>(jsn.text);
            Debug.Log(jsn.text);
        } else {
            Debug.LogError("Texto não encontrado.");
        }

        playerFighter = new Fighter(true,hpInicial);
        enemyFighter = new Fighter(false,hpInicial);
        
        UpdateUI();
    }
    //O método PerformMove adiciona golpes à respectiva lista dos golpes
    public IEnumerator PerformMove (Move newMove) {
        
        AudioClip audioToPlay = null;
        //Caso a lista temporária seja menor do que a final, adiciona o próximo golpe à temporária
        //e é checado se a sequência atual é correta.
        
        PopUp(lastMove.gameObject, 0.3f);

        if(tempMoves.Count < fightMoves.Count) {
            tempMoves.Add(newMove);
            //O áudio do golpe é tocado uma vez
            
            
            lastMove.text = newMove.ToString();

            //Caso a sequência não esteja correta, o personagem leva dano
            if(!RightSequence()) {
                lastMove.text = "Dano recebido!";
                StartCoroutine(TakeDamage());
            }
            else if (tempMoves.Count == fightMoves.Count && ShiftManagementScript.state== BattleState.PLAYERTURN) {
                audioToPlay = punchSounds[(int)newMove];
                TextPlayer.instance.playInSequence(audioToPlay);
                yield return new WaitForSeconds(audioToPlay.length);
                
                inputText.text = "Faça um movimento novo!";
                audioToPlay = Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/faca_um_movimento_novo");
                
                TextPlayer.instance.playInSequence(audioToPlay);
                yield return new WaitForSeconds(audioToPlay.length);                
            }
            else {
                TextPlayer.instance.playInSequence(punchSounds[(int)newMove]);
            }
            
            yield break;
        }
        //Se a sequência estiver correta, o golpe é adicionado à lista principal e a lista temporária é zerada
        fightMoves.Add(newMove);
        lastMove.text = newMove.ToString();
        //O áudio do golpe é tocado uma vez
        audioToPlay = punchSounds[(int)newMove];
        TextPlayer.instance.playInSequence(audioToPlay);
        yield return new WaitForSeconds(audioToPlay.length + 0.1f);
        tempMoves = new List<Move>();

        if(ShiftManagementScript.state == BattleState.ENEMYTURN) {
            suspendMoveCalculation = true;
            shiftManagementScript.enemyButton.onClick.Invoke();
            audioToPlay = Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/seu_inimigo_encerrou_seu_turno");
            TextPlayer.instance.playInSequence(audioToPlay);
            yield return new WaitForSeconds(audioToPlay.length);
            suspendMoveCalculation = false;
            //Caso o turno seja do jogador, invoca-se o botão de mudança de turno para o inimigo
            

        } else if (ShiftManagementScript.state == BattleState.PLAYERTURN) {
            //Caso contrário, invoca-se o botão de mudança de turno para o jogador
            suspendMoveCalculation = true;
            shiftManagementScript.playerButton.onClick.Invoke();
            audioToPlay = Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/seu_turno_acabou");
            TextPlayer.instance.playInSequence(audioToPlay);
            yield return new WaitForSeconds(audioToPlay.length);
            suspendMoveCalculation = false;
        }

        
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
            TextPlayer.instance.playInSequence  (playerDamageSound,
                                                Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/dano_recebido"));
        } else if(ShiftManagementScript.state == BattleState.ENEMYTURN) {
            enemyFighter.hP--;
            TextPlayer.instance.playInSequence  (enemyDamageSound,
                                                Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/dano_recebido"));
        }
        UpdateUI();
        yield return new WaitForSeconds(2f);
        PlayHPCounter(playerFighter);
        yield return new WaitForSeconds(3f);
        PlayHPCounter(enemyFighter);
        yield return new WaitForSeconds(5f);

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
            TextPlayer.instance.playInSequence(Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/faca_um_movimento_novo"));
        }
        yield return new WaitForSeconds(2f);
        suspendMoveCalculation = false;
    }

    public void PlayHPCounter(Fighter fghtr) {
        if(fghtr.player) {
            if(fghtr.hP != 1) {
                TextPlayer.instance.playInSequence  (   Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/voce_tem"),
                                                        Resources.Load<AudioClip>(TextPlayer.SONS_NUMEROS + fghtr.hP),
                                                        Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/pontos_de_vida"));
            } else {
                TextPlayer.instance.playInSequence  (   Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/voce_tem"),
                                                        Resources.Load<AudioClip>(TextPlayer.SONS_NUMEROS + fghtr.hP),
                                                        Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/ponto_de_vida"));
            }
        } else {
            if(fghtr.hP != 1) {
                TextPlayer.instance.playInSequence  (   Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/seu_adversario_tem"),
                                                        Resources.Load<AudioClip>(TextPlayer.SONS_NUMEROS + fghtr.hP),
                                                        Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/pontos_de_vida"));
            } else {
                TextPlayer.instance.playInSequence  (   Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/seu_adversario_tem"),
                                                        Resources.Load<AudioClip>(TextPlayer.SONS_NUMEROS + fghtr.hP),
                                                        Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/ponto_de_vida"));
            }
        }

    }

    //No fim do jogo, o texto da UI é atualizado com a mensagem de vitória ou derrota
    //Além disso, o game do lutador respectivo é contabilizado e as vidas são reiniciadas
    private IEnumerator EndRound() {
        if(playerFighter.hP > 0) {
            playerFighter.games++;
            lastMove.text = "Round ganho";
            TextPlayer.instance.playInSequence(Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/voce"),
                                            Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/ganhou_a_rodada"));
        } else {
            enemyFighter.games++;
            lastMove.text = "Round perdido";
            TextPlayer.instance.playInSequence(Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/voce"),
                                            Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/perdeu_a_rodada"));
        }

        yield return new WaitForSeconds(3f);

        if(playerFighter.games == gamesToWin || enemyFighter.games == gamesToWin) {
            StartCoroutine(EndGame());
            UpdateUI();
            yield break;
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
            lastMove.text = "Você ganhou a luta! :DDDD";
            // yield return new WaitForSeconds(1);
            // sceneChanger.LoadGame("_StreetFighter");
            description = victoryDescription;
        } else if(enemyFighter.games == gamesToWin) {
            lastMove.text = "Você perdeu a luta! :(";
            description = defeatDescription;            
            // yield return new WaitForSeconds(3);
            // sceneChanger.LoadGame("_StreetFighter");

        }
        TextPlayer.instance.playInSequence(description);
        yield return new WaitForSeconds(description.length);       
        sceneChanger.LoadGame("_Sequencia_de_combate");

        yield return null;
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
