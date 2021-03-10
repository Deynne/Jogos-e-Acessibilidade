using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum Move {UP, DOWN, LEFT, RIGHT}
public class Fighter {
    public int games = 0;
    public int hP = 0;
}
public class FightManager : MonoBehaviour
{
    //Esta classe inicializa duas listas, uma para os movimentos recorrentes da luta, e outra que abriga os temporários do jogador atual
    public int hpInicial = 3;
    public int gamesToWin = 2;
    public List<Move> fightMoves = new List<Move>();
    public List<Move> tempMoves = new List<Move>();
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
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip victoryDescription;
    [SerializeField] private AudioClip defeatDescription;
    private AudioSource left,right;
    public Fighter playerFighter = new Fighter();
    public Fighter enemyFighter = new Fighter();


    //No início, os HP dos lutadores são determinados
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
        playerFighter.hP = hpInicial;
        enemyFighter.hP = hpInicial;
        UpdateUI();
    }
    //O método PerformMove adiciona golpes à respectiva lista dos golpes
    public void PerformMove (Move newMove) {

        //Checando o início do jogo
        if(ShiftManagementScript.state == BattleState.START) {
            return;
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
                lastMove.text = "Dano recebido!";
                TakeDamage();
            }
            else if (tempMoves.Count == fightMoves.Count && ShiftManagementScript.state== BattleState.PLAYERTURN) {
                inputText.text = "Faça um movimento novo!";
                left.PlayOneShot(punchSounds[(int)newMove]);
                right.PlayOneShot(punchSounds[(int)newMove]);
            }
            else {
                left.PlayOneShot(punchSounds[(int)newMove]);
                right.PlayOneShot(punchSounds[(int)newMove]);
            }
            return;
        }     
        //Se a sequência estiver correta, o golpe é adicionado à lista principal e a lista temporária é zerada
        fightMoves.Add(newMove);
        //O áudio do golpe é tocado uma vez
        left.PlayOneShot(punchSounds[(int)newMove]);
        right.PlayOneShot(punchSounds[(int)newMove]);
        lastMove.text = newMove.ToString();
        tempMoves = new List<Move>();

        if(ShiftManagementScript.state == BattleState.ENEMYTURN) {
            //Caso o turno seja do jogador, invoca-se o botão de mudança de turno para o inimigo
            shiftManagementScript.enemyButton.onClick.Invoke();

        } else if (ShiftManagementScript.state == BattleState.PLAYERTURN) {
            //Caso contrário, invoca-se o botão de mudança de turno para o jogador
            shiftManagementScript.playerButton.onClick.Invoke();
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
    public void TakeDamage() {
        //Audio de dano toca.
        left.PlayOneShot(damageSound);
        right.PlayOneShot(damageSound);

        if(ShiftManagementScript.state == BattleState.PLAYERTURN) {
            playerFighter.hP--;
        } else if(ShiftManagementScript.state == BattleState.ENEMYTURN) {
            enemyFighter.hP--;
        }
        UpdateUI();

        //As listas são reinicializadas
        fightMoves = new List<Move>();
        tempMoves = new List<Move>();

        //Caso alguém chegue a 0 de vida, o jogo se encerra
        if(playerFighter.hP == 0 || enemyFighter.hP == 0) {
            EndTurn();
        }
    }

    //No fim do jogo, o texto da UI é atualizado com a mensagem de vitória ou derrota
    //Além disso, o game do lutador respectivo é contabilizado e as vidas são reiniciadas
    private void EndTurn() {
        if(playerFighter.hP > 0) {
            playerFighter.games++;
            lastMove.text = "Round ganho";
        } else {
            enemyFighter.games++;
            lastMove.text = "Round perdido";
        }

        if(playerFighter.games == gamesToWin || enemyFighter.games == gamesToWin)
            StartCoroutine(EndGame());
        
        playerFighter.hP = hpInicial;
        enemyFighter.hP = hpInicial;
        UpdateUI();
    }
    IEnumerator EndGame() {
        if(playerFighter.games == gamesToWin) {
            lastMove.text = "Você ganhou a luta! :DDDD";
            // yield return new WaitForSeconds(1);
            // sceneChanger.LoadGame("_StreetFighter");
            

            left.PlayOneShot(victoryDescription);
            right.PlayOneShot(victoryDescription);
        } else if(enemyFighter.games == gamesToWin) {
            lastMove.text = "Você perdeu a luta! :(";
            left.PlayOneShot(defeatDescription);
            right.PlayOneShot(defeatDescription);
            // yield return new WaitForSeconds(3);
            // sceneChanger.LoadGame("_StreetFighter");

        }
        while(left.isPlaying) yield return null;       
        sceneChanger.LoadGame("_StreetFighter");

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
