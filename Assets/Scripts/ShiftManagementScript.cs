﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Estados possíveis da batalha
// Lembrar de adicionar WIN e LOST depois
public enum BattleState { START, PLAYERTURN, ENEMYTURN }

public class ShiftManagementScript : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Button playerButton;
    public Button enemyButton;
    
    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public BattleState state;

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
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);        

        yield return new WaitForSeconds(2f);

        UIText.text = "Player, inicie a batalha!";

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
        UIText.text = "Player acabou de jogar, vez do inimigo!";        
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
        UIText.text = "Inimigo acabou de jogar, vez do player!";
        state = BattleState.PLAYERTURN;
        ManageBattle();
    }

    public void ManageBattle()
    {
        /*
         A troca de estados é essencial pra que a mecânica de turnos funcione.
         Assim que o ManageBattle é chamado, o estado está em player.
         */                 
        if (state == BattleState.PLAYERTURN)
            // Clicar no botão de player permite que ele faça algo
            playerButton.onClick.AddListener(PlayerDoesSomething);
        else if (state == BattleState.ENEMYTURN)
            // Clicar no botão de imigo permite que ele faça algo
            enemyButton.onClick.AddListener(EnemyDoesSomething);
        // Adicionar estado de finalização da batalha
    }


}
