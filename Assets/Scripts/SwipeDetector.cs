using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    //Variável pública que representa o deslocamento para que o swipe seja detectado
    public float swipeOffset = 100f;

    [SerializeField] private FightManager fightManagerScript;

    private void Awake() {
        //A classe encontra o objeto do tipo FightManager e atribui à variável devida.
        fightManagerScript = GameObject.FindObjectOfType<FightManager>();
        if(fightManagerScript == null) {
            Debug.LogWarning("FightManager não encontrado.");
        }
    }
    private void OnEnable() {
        EnhancedTouchSupport.Enable(); //Inicializando a classe suporte para touch
    }
    private void OnDisable() {
        EnhancedTouchSupport.Disable();
    }

    private void Update() {
        //Descomentar o próximo if quando a IA estiver implementada
        if(ShiftManagementScript.state == BattleState.PLAYERTURN)
            CheckSwipe();
    }

    public void CheckSwipe() {
        
        //Aqui checa-se se existe exatamente um dedo ativo na tela
        if(Touch.activeFingers.Count == 1) {
            
            Touch activeTouch = Touch.activeFingers[0].currentTouch;
            
            //O código so continua caso o toque esteja em sua fase final ("Ended")
            if(activeTouch.phase != TouchPhase.Ended) {
                return;
            }
            
            //As diferenças entre os valores inicial e final de X e Y são calculadas
            float difX = activeTouch.screenPosition.x - activeTouch.startScreenPosition.x;
            float difY = activeTouch.screenPosition.y - activeTouch.startScreenPosition.y;

            //Aqui é checado qual deslocamento no eixo foi maior
            //Caso o maior seja X, o código detecta esquerda ou direita
            //Caso o maior seja Y, o código detecta baixo ou cima
            if(Mathf.Abs(difX) > Mathf.Abs(difY)) {

                //Aqui checa-se se difX é positivo ou negativo
                if(difX < -swipeOffset) {

                    //CHAMAR AQUI COMANDOS PARA SWIPE ESQUERDO
                    //Debug.LogWarning("ESQUERDA");
                    fightManagerScript.PerformMove(Move.LEFT);

                } else if(difX > swipeOffset) {

                    //CHAMAR AQUI COMANDOS PARA SWIPE DIREITO
                    //Debug.LogWarning("DIREITA");
                    fightManagerScript.PerformMove(Move.RIGHT);
                }
            } else {
                //Aqui checa-se se difY é positivo ou negativo
                if(difY < -swipeOffset) {

                    //CHAMAR AQUI COMANDOS PARA SWIPE PARA BAIXO
                    //Debug.LogWarning("BAIXO");
                    fightManagerScript.PerformMove(Move.DOWN);

                } else if(difY > swipeOffset) {
                    //CHAMAR AQUI COMANDOS PARA SWIPE PARA CIMA
                    //Debug.LogWarning("CIMA");
                    fightManagerScript.PerformMove(Move.UP);
                }
            }
        }
    }
}
