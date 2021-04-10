using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Estados possíveis da batalha
// Lembrar de adicionar WIN e LOST depois
public enum BattleState { START, PLAYERTURN, ENEMYTURN, PERFORMING, END }

public class ShiftManagementScript : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Button playerButton;
    public Button enemyButton;
    
    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    //Variável colocada como static para acesso mais fácil por outros scripts
    public static BattleState state;

    public Text UIText;

    private void Start()
    {
        // Instancia os botões 
        playerButton = playerButton.GetComponent<Button>();
        enemyButton = enemyButton.GetComponent<Button>();

        // Inicia a batalha
        state = BattleState.START;
        StartCoroutine(SetUpBattle());
    }

    IEnumerator SetUpBattle()
    {
        //Método que instancia player e inimigo no inicio da batalha
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerGO.transform.localPosition = new Vector3(0,5,1);        
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyGO.transform.localPosition = new Vector3(0,5,1);        

        yield return new WaitForSeconds(2f);

        UIText.text = "Inicie a batalha! Faça o primeiro movimento!";
        TextPlayer.instance.playInSequence(Resources.Load<AudioClip>(TextPlayer.SONS_GAMES + "SequenciaDeCombateDescriptions/inicie_a_batalha"));

        // Muda o estado da batalha pra que o player inicie e chama o manipulador de players
        state = BattleState.PLAYERTURN;
        ManageBattle();
        
    }

    void PlayerDoesSomething()
    {
        // Função teste que só troca o texto da ui pra dizer que é a vez do player
        // A troca de estados pra ENEMYTURN impede que o player jogue novamente.
        /*
         AQUI, VOCÊ DEVE CHAMAR OS SCRIPTS QUE IRÃO CONTROLAR O GAME OBJECT DO PLAYER
         */        
        //UIText.text = "Player acabou de jogar, vez do inimigo!";        
        state = BattleState.ENEMYTURN;
        ManageBattle();
    }

    void EnemyDoesSomething()
    {
        // Função teste que só troca o texto da ui pra dizer que é a vez do inimigo
        // A troca de estados pra ENEMYTURN impede que o inimigo jogue novamente.
        /*
         AQUI, VOCÊ DEVE CHAMAR OS SCRIPTS QUE IRÃO CONTROLAR O GAME OBJECT DO INIMIGO
         */
        //UIText.text = "Inimigo acabou de jogar, vez do player!";
        state = BattleState.PLAYERTURN;
        ManageBattle();
    }

    public void ManageBattle()
    {
        /*
         A troca de estados é essencial pra que a mecânica de turnos funcione.
         Assim que o ManageBattle é chamado, o estado está em player.
         */                 
        if (state == BattleState.PLAYERTURN) {
            // Clicar no botão de player permite que ele faça algo
            playerButton.onClick.AddListener(PlayerDoesSomething);
            }
        else if (state == BattleState.ENEMYTURN) {
            // Aqui precisa-se adicionar um áudio de movimento do adversário.
            // Clicar no botão de imigo permite que ele faça algo
            enemyButton.onClick.AddListener(EnemyDoesSomething);
        }
        // Adicionar estado de finalização da batalha
    }

}
