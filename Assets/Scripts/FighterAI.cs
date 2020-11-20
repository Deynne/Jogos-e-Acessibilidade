using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAI : MonoBehaviour
{
    
    [Range(0,100)] public float hitChance = 80f;
    [SerializeField] private FightManager fightManagerScript;
    
    
    void Awake()
    {
         //A classe encontra o objeto do tipo FightManager e atribui à variável devida.
        fightManagerScript = GameObject.FindObjectOfType<FightManager>();
        if(fightManagerScript == null) {
            Debug.LogWarning("FightManager não encontrado.");
        }    
    }

    private void Update() {
        if(ShiftManagementScript.state == BattleState.ENEMYTURN) {
            CalculateMove();
        }
    }

    public void CalculateMove() {

        Move toPerform = new Move();
        Move lastValid = new Move();
        if(fightManagerScript.fightMoves.Count > 0) {
            lastValid = fightManagerScript.fightMoves[fightManagerScript.fightMoves.Count-1];
        }

        if (fightManagerScript.tempMoves.Count < fightManagerScript.fightMoves.Count) {
            float prob = Random.Range(0f,100f);
            if(prob <= hitChance) {
                toPerform = lastValid;
            } else {
                while(toPerform == lastValid) {
                    toPerform = (Move)Random.Range(0,5);
                }
            }
        } else {
            toPerform = (Move)Random.Range(0,5);
        }

        fightManagerScript.PerformMove(toPerform);
    }
}
