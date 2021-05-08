using System;
using UnityEngine;

// Modulo Depreciado. Mantido para documentação futura em TCC.

// Esta classe é uma cópia adaptada do StandaloneInputModule e é utilizada para
// Adaptar a leitura de dados para o formato de acessibilidade
namespace UnityEngine.EventSystems {
    public class AccessibilityInputModule_old : PointerInputModule {

        // Armazena as posições do mouse
        private Vector2 _LastMousePosition;
        private Vector2 _MousePosition;

        // Armazena referência para o pointerEvent do mouse/touch
        private PointerEventData _InputPointerEvent;

        // Acessa o singleton contendo a lista de interagíveis
        private InteractableList interactableList;

        // Constante para determinar o número mínimo de clicks para que seja considerada
        // Uma interação
        private const int minClicks = 2;

        /* Aqui é criado um handler para leitura das descrições de audio
           Isto é necessário por que eu não quero que o efeito do click seja executado
           quando eu pressionar para ouvir o audio.

           O modelo do handler foi feito com base no script ExecuteEvents.

           Aqui é criado um apontador para a função de eventos que procura e executa
           nos objetos, a função especificada por DescriptionEventHandler
        */
        public static readonly ExecuteEvents.EventFunction<DescriptionEventHandler> pointerDescriptionHandler = Execute;

        // função de eventos que procura e executa nos objetos, a função especificada por
        // DescriptionEventHandler
        private static void Execute(DescriptionEventHandler handler, BaseEventData eventData)
        {
            handler.OnDescriptorPress(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
        }

        // Verifica se o sistem deve ignorar inputs na perda de foco
        private bool ShouldIgnoreEventsOnNoFocus() {
            switch (SystemInfo.operatingSystemFamily) {
                case OperatingSystemFamily.Windows:
                case OperatingSystemFamily.Linux:
                case OperatingSystemFamily.MacOSX:
#if UNITY_EDITOR
                    if (UnityEditor.EditorApplication.isRemoteConnected) {
                        return false;
                    }
#endif
                    return true;
                default:
                    return false;
            }
        }

        // Atualiza a posição do mouse e, se a tela perder o foco, libera todas as
        // operações com o mouse.
        public override void UpdateModule() {
            if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus()) {
                if (_InputPointerEvent != null && _InputPointerEvent.pointerDrag != null && _InputPointerEvent.dragging)
                {
                    ReleaseMouseButton(_InputPointerEvent, _InputPointerEvent.pointerCurrentRaycast.gameObject);
                }

                _InputPointerEvent = null;

                return;
            }
        
            _LastMousePosition = _MousePosition;
            _MousePosition = input.mousePosition;
        }

        // Executa ao liberar o botão do mouse.
        // O evento relacionado ao Click é executado neste momento
        private void ReleaseMouseButton(PointerEventData pointerEvent, GameObject currentGo) {
            // Realiza evento de "soltar o botão"
            ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

            var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentGo);
            
            // Se um click for realizado entra neste if
            // Em caso de um click pode ser um movimento de deslize, buscando o transido entre
            // Objetos interagíveis de forma cíclica
            // Em caso de dois clicks seguidos é uma interação
            if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
            {
                // Verifica se o deslize foi para esquerda ou direita
                // TODO Testar a necessidade de uma margem de erro para evitar deslizes indesejados
                float diff = _LastMousePosition.x - _MousePosition.x;
                // Uma flag para indicar se o movimento foi de toque ou deslize
                bool swipe = false;
                // Se o valor for negativo é um deslize para esquerda, caso contrário para direita
                // À esquerda o item anterior é selecionado, a direita o item posterior é selecionado
                // O movimento é realizado de forma cíclica pela interface
                if(diff < 0 && !interactableList.isEmpty) {
                    currentGo = interactableList.Next();
                    eventSystem.SetSelectedGameObject(currentGo);
                    swipe = true;
                    ExecuteEvents.ExecuteHierarchy(currentGo,pointerEvent,pointerDescriptionHandler);
                }
                else if(diff > 0 && !interactableList.isEmpty) {
                    currentGo = interactableList.Previous();
                    eventSystem.SetSelectedGameObject(currentGo);
                    swipe = true;
                    ExecuteEvents.ExecuteHierarchy(currentGo,pointerEvent,pointerDescriptionHandler);
                }
                // Se o minimo de clicks para interação foi realizado, permite a interação;
                if(pointerEvent.clickCount >= minClicks) {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                    pointerEvent.clickCount = 0;
                }
                // Se o movimento foi de toque, executa a descrição de audio
                else if(!swipe) {
                    ExecuteEvents.ExecuteHierarchy(currentGo,pointerEvent,pointerDescriptionHandler);
                }
            }
            // Trata eventos relacionado ao arrastar
            
            else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(currentGo, pointerEvent, ExecuteEvents.dropHandler);
            }
            // Reseta variáveis do pointer
            pointerEvent.eligibleForClick = false;
            pointerEvent.pointerPress = null;
            pointerEvent.rawPointerPress = null;

            // Se necessário finaliza o movimento de arrastar
            if (pointerEvent.pointerDrag != null && pointerEvent.dragging) {
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
            }

            // reseta as variaveis do pointer
            pointerEvent.dragging = false;
            pointerEvent.pointerDrag = null;

            // redo pointer enter / exit to refresh state
            // so that if we moused over something that ignored it before
            // due to having pressed on something else
            // it now gets it.
            if (currentGo != pointerEvent.pointerEnter)
            {
                HandlePointerExitAndEnter(pointerEvent, null);
                HandlePointerExitAndEnter(pointerEvent, currentGo);
            }

            _InputPointerEvent = pointerEvent;
        }

        protected bool SendUpdateEventToSelectedObject()
        {
            if (eventSystem.currentSelectedGameObject == null)
                return false;

            var data = GetBaseEventData();
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
            return data.used;
        }

        // Realiza o processamento das entradas
        public override void Process() {
            if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus() || DescriptionPlayer.playingTutorial)
                return;
            

            // bool usedEvent = SendUpdateEventToSelectedObject();

            // Trata o touch primeiro devido a problemas de emulação de mouse
            if (!ProcessTouchEvents() && input.mousePresent)
                ProcessMouseEvent(0);
            
            // // Deixei isso por não saber se pode vir a ser necessário futuramente.
            // if (eventSystem.sendNavigationEvents)
            // {
            //     if (!usedEvent) 
            //         usedEvent |= SendMoveEventToSelectedObject();

            //     if (!usedEvent)
            //         SendSubmitEventToSelectedObject();
            // }            
        }

        // Trata os eventos do touch
        private bool ProcessTouchEvents() {
            // Verifica quantos pontos de toque existem e então commeça o tratamento
            for (int i = 0; i < input.touchCount; ++i)
            {
                Touch touch = input.GetTouch(i);

                if (touch.type == TouchType.Indirect)
                    continue;

                bool released;
                bool pressed;
                var pointer = GetTouchPointerEventData(touch, out pressed, out released);
                // Trata o toque na entrada e na saída
                ProcessTouchPress(pointer, pressed, released);

                // Caso ocorra um toque contínuo ele provavelmente é um movimento ou um
                // arrasto
                if (!released)
                {
                    ProcessMove(pointer);
                    ProcessDrag(pointer);
                }
                // Caso tenha sido solto, libera o pointer do toque.
                else
                    RemovePointerData(pointer);
            }
            return input.touchCount > 0;
        }

        // Aqui é tratado as entradas e saídas do toque.
        // O objeto em currentOverGo é o objeto atualmente focado para interação
        protected void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released) {
            if(interactableList.isEmpty) return;
            var currentOverGo = interactableList.focusedGo; //eventSystem.currentSelectedGameObject;//pointerEvent.pointerCurrentRaycast.gameObject;
            if(!currentOverGo) return;
            // PointerDown notification
            if (pressed)
            {
                PressTouch(pointerEvent,currentOverGo);
            }

            // PointerUp notification
            if (released)
            {
                ReleaseTouch(pointerEvent,currentOverGo);
            }
        }

        // Quando o botão é pressionado verifica se é um botão previamente pressionado
        // e conta o toque pra ele, desde que esteja dentro do tempo limite.
        // Aqui também é tratado os eventos de "pressionar" do toque, mas não o de click
        private void PressTouch(PointerEventData pointerEvent, GameObject currentGo) {
            pointerEvent.eligibleForClick = true;
            pointerEvent.delta = Vector2.zero;
            pointerEvent.dragging = false;
            pointerEvent.useDragThreshold = true;
            // A posição de pressionamento deve ser a do objeto
            pointerEvent.pressPosition = pointerEvent.position;


            if (pointerEvent.pointerEnter != currentGo)
            {
                // send a pointer enter to the touched element if it isn't the one to select...
                HandlePointerExitAndEnter(pointerEvent, currentGo);
                pointerEvent.pointerEnter = currentGo;
            }

            // search for the control that will receive the press
            // if we can't find a press handler set the press
            // handler to be what would receive a click.
            GameObject newPressed = ExecuteEvents.GetEventHandler<IPointerDownHandler>(currentGo);
                
            // didnt find a press handler... search for a click handler
            if (newPressed == null)
                newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentGo);

            // Debug.Log("Pressed: " + newPressed);

            float time = Time.unscaledTime;

            if (newPressed == pointerEvent.lastPress)
            {
                var diffTime = time - pointerEvent.clickTime;
                if (diffTime < 0.3f)
                    ++pointerEvent.clickCount;
                else
                    pointerEvent.clickCount = 1;

                pointerEvent.clickTime = time;
            }
            else
            {
                pointerEvent.clickCount = 1;
            }

            // Caso seja uma interação que o evento evento de "pressionar ocorra"
            // Tem maiores efeitos na responsividade do sistema.
            if(pointerEvent.clickCount >= minClicks) {
                ExecuteEvents.ExecuteHierarchy(currentGo, pointerEvent, ExecuteEvents.pointerDownHandler);
            }

            pointerEvent.pointerPress = newPressed;
            pointerEvent.rawPointerPress = currentGo;

            pointerEvent.clickTime = time;

            // Save the drag handler as well
            pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentGo);

            if (pointerEvent.pointerDrag != null)
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);

            _InputPointerEvent = pointerEvent;
        }


        // Trata a soltura do evento para o touch
        // Funciona semelhante ao ReleaseMouseButton
        private void ReleaseTouch(PointerEventData pointerEvent, GameObject currentGo) {
            // Debug.Log("Executing pressup on: " + pointer.pointerPress);
            ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);


            // see if we mouse up on the same element that we clicked on...
            var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentGo);

            // PointerClick and Drop events
            if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
            {
                float diff = _LastMousePosition.x - _MousePosition.x;
                bool swipe = false;
                if(diff < 0 && !interactableList.isEmpty) {
                    swipe = true;
                    eventSystem.SetSelectedGameObject(interactableList.Next());
                    ExecuteEvents.ExecuteHierarchy(currentGo,pointerEvent,pointerDescriptionHandler);
                }
                else if(diff > 0 && !interactableList.isEmpty) {
                    swipe = true;
                    eventSystem.SetSelectedGameObject(interactableList.Previous());
                    ExecuteEvents.ExecuteHierarchy(currentGo,pointerEvent,pointerDescriptionHandler);
                }
                
                if(pointerEvent.clickCount > minClicks) {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                }
                else if(!swipe) {
                    ExecuteEvents.ExecuteHierarchy(currentGo,pointerEvent,pointerDescriptionHandler);
                }
            }
            else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(currentGo, pointerEvent, ExecuteEvents.dropHandler);
            }

            pointerEvent.eligibleForClick = false;
            pointerEvent.pointerPress = null;
            pointerEvent.rawPointerPress = null;

            if (pointerEvent.pointerDrag != null && pointerEvent.dragging) {
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
            }

            pointerEvent.dragging = false;
            pointerEvent.pointerDrag = null;

            // send exit events as we need to simulate this on touch up on touch device
            ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
            pointerEvent.pointerEnter = null;

            _InputPointerEvent = pointerEvent;
        }

        // Funciona semelhante ao ProcessTouchEvent
        // Mas como o mouse conta como apenas um toque, não há contagem de toques.
        protected void ProcessMouseEvent(int id) {
            var mouseData = GetMousePointerEventData(id);
            var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;


            // Process the first mouse button fully
            ProcessMousePress(leftButtonData);
            ProcessMove(leftButtonData.buttonData);
            ProcessDrag(leftButtonData.buttonData);

            
        }

        // Funciona semelhante ao ProcessTouchPress
        protected void ProcessMousePress(MouseButtonEventData data) {
             if(interactableList.isEmpty) return;
            var pointerEvent = data.buttonData;
            // O objeto selecionado é aquele que está focado, ja que a posição do mouse não importa.
            var currentOverGo = interactableList.focusedGo; //eventSystem.firstSelectedGameObject;//pointerEvent.pointerCurrentRaycast.gameObject;
            if(!currentOverGo) return;
            // PointerDown notification
            if (data.PressedThisFrame())
            {
                PressMouseButton(pointerEvent,currentOverGo);
            }

            // PointerUp notification
            if (data.ReleasedThisFrame())
            {
                ReleaseMouseButton(pointerEvent, currentOverGo);
            }
        }

        private void PressMouseButton(PointerEventData pointerEvent, GameObject currentGo) {
            pointerEvent.eligibleForClick = true;
            pointerEvent.delta = Vector2.zero;
            pointerEvent.dragging = false;
            pointerEvent.useDragThreshold = true;
            // A posição de pressionamento deve ser a do objeto
            pointerEvent.pressPosition = pointerEvent.position;
            //pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

            //DeselectIfSelectionChanged(currentGo, pointerEvent);

            // search for the control that will receive the press
            // if we can't find a press handler set the press
            // handler to be what would receive a click.
            GameObject newPressed = ExecuteEvents.GetEventHandler<IPointerDownHandler>(currentGo);
            
            // didnt find a press handler... search for a click handler
            if (newPressed == null)
                newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentGo);

            // Debug.Log("Pressed: " + newPressed);

            float time = Time.unscaledTime;

            if (newPressed == pointerEvent.lastPress)
            {
                var diffTime = time - pointerEvent.clickTime;
                if (diffTime < 0.3f)
                    ++pointerEvent.clickCount;
                else
                    pointerEvent.clickCount = 1;

                pointerEvent.clickTime = time;
            }
            else
            {
                pointerEvent.clickCount = 1;
            }

            if(pointerEvent.clickCount >= minClicks) {
                ExecuteEvents.ExecuteHierarchy(currentGo, pointerEvent, ExecuteEvents.pointerDownHandler);
            }

            pointerEvent.pointerPress = newPressed;
            pointerEvent.rawPointerPress = currentGo;

            pointerEvent.clickTime = time;

            // Save the drag handler as well
            pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentGo);
            if (pointerEvent.pointerDrag != null)
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);

            _InputPointerEvent = pointerEvent;
        }


        // Ao iniciar o module é realizada uma busca de objetos interagíveis.
        // TODO Verificar a possibilidade de popular a lista de outros meios
        protected override void Start() {
            base.Start();
            interactableList = ListSingleton.instance;
            // interactableList.ClearList();
            //interactableList.FindInteractables();
            if(!interactableList.isEmpty) {
                eventSystem.SetSelectedGameObject(interactableList.focusedGo);
                DescriptionPlayer dp = interactableList.focusedGo.GetComponent(typeof(DescriptionPlayer)) as DescriptionPlayer;
                dp.OnDescriptorPress(null);
            }
        }
        protected override void ProcessDrag(PointerEventData pointerEvent) {
            if(Input.GetKey(KeyCode.Space) || input.touchCount > 1)
                base.ProcessDrag(pointerEvent);
            
        }
    }
}