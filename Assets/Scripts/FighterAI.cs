using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAI : MonoBehaviour
{
    //Esta classe é encarregada da inteligência artificial do inimigo.
    //As variáveis mais importantes são "hitChance" e "decreaseChanceByMove".
    //A primeira é a chance de acertar um golpe. O valor escolhido no editor é o valor inicial.
    //Este valor descresce pela quantidade de "decreaseChanceByMove" a cada movimento executado.
    //(Discutir outras possíveis abordagens para a IA)
    [Range(0,100)] public float inicialHitChance = 95f;
    [SerializeField]private float hitChance = 0;
    public float decreaseChanceByMove = 0.5f;
    private bool enableCoroutine = true;
    [SerializeField] private FightManager fightManagerScript;
    
    
    void Awake()
    {
        hitChance = inicialHitChance;
         //A classe encontra o objeto do tipo FightManager e atribui à variável devida.
        fightManagerScript = GameObject.FindObjectOfType<FightManager>();
        if(fightManagerScript == null) {
            Debug.LogWarning("FightManager não encontrado.");
        }
    }

    private void Update() {
        //A cada frame é checado se o turno é o do inimigo
        if(ShiftManagementScript.state != BattleState.ENEMYTURN) {
            return;
        }

        //Se for, e se o cáculo estiver habilitado, a corotina da IA é iniciada e as próximas corotinas são desabilitadas
        if(enableCoroutine) {
            StartCoroutine(AICoroutine());
            enableCoroutine = false;
        }

    }

    //Este método aplica a chance de acerto para acerto ou erro do golpe
    public void CalculateMove() {

        Move toPerform = new Move();
        Move nextValid = new Move();

        //Checa-se os tamanhos das listas de golpes
        if (fightManagerScript.tempMoves.Count < fightManagerScript.fightMoves.Count) {

            //O golpe na lista verdadeira no índice equivalente ao tamanho da lista temporária é atribuído à variável local
            nextValid = fightManagerScript.fightMoves[fightManagerScript.tempMoves.Count];

            //O valor aleatório é sorteado
            float prob = Random.Range(0f,100f);

            //Caso ele seja menor ou igual à chance de acerto, o lutador acertará o golpe
            if(prob <= hitChance) {
                Debug.LogWarning("Acertará");

                //E a variável "toPerform" recebe o próximo golpe corretamente
                toPerform = nextValid;
            } else {
                Debug.LogWarning("Errará");
                
                //Caso contrário, o golpe será errado e "toPerform" recebe outro golpe aleatório
                while(toPerform == nextValid) {
                    toPerform = (Move)Random.Range(0,4);
                }
            }
        } else {
            //Caso as listas tenham o mesmo tamanho, "toPerform" recebe um valor aleatório
            toPerform = (Move)Random.Range(0,4);
        }

    //O golpe é executado e a chance de acertar o próximo diminui de acordo com "decreaseChanceByMove"
        fightManagerScript.PerformMove(toPerform);
        hitChance -= decreaseChanceByMove;

        if(fightManagerScript.fightMoves.Count == 0) {
            hitChance = inicialHitChance;
        }
    }

    //Esta corotina executa um cálculo a cada segundo enquanto for o turno do inimigo, depois habilita a chamada dela mesma 
    IEnumerator AICoroutine() {
        while(ShiftManagementScript.state == BattleState.ENEMYTURN) {
            yield return new WaitForSeconds(1);
            CalculateMove();
        }
        enableCoroutine = true;
    }
}
