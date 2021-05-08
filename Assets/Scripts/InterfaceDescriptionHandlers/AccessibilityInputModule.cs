using System;
using UnityEngine;
using UnityEngine.Serialization;

// Modulo para interação segundo funções de acessibilidade do jogo.
namespace UnityEngine.EventSystems
{
    [AddComponentMenu("Event/Accessibility Input Module")]
    /// <summary>
    /// A BaseInputModule designed for mouse / keyboard / controller input.
    /// </summary>
    /// <remarks>
    /// Input module for working with, mouse, keyboard, or controller.
    /// </remarks>
    public class AccessibilityInputModule : PointerInputModule
    {
        private bool useRaycastObject;
        private bool holding;
        private bool moving;
        private bool dragging;
        private bool swipe;
        // Acessa o singleton contendo a lista de interagíveis
        private InteractableList interactableList;

        // Constante para determinar o número mínimo de clicks para que seja considerada
        // Uma interação
        private const int minClicks = 2;

        private float lastPressedTime;

        private float m_PrevActionTime;
        private Vector2 m_LastMoveVector;
        private int m_ConsecutiveMoveCount = 0;

        private Vector2 m_LastMousePosition;
        private Vector2 m_MousePosition;

        private GameObject m_CurrentFocusedGameObject;

        private PointerEventData m_InputPointerEvent;

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


        protected AccessibilityInputModule()
        {
        }

        [Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
        public enum InputMode
        {
            Mouse,
            Buttons
        }

        [Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
        public InputMode inputMode
        {
            get { return InputMode.Mouse; }
        }

        [SerializeField]
        private string m_HorizontalAxis = "Horizontal";

        /// <summary>
        /// Name of the vertical axis for movement (if axis events are used).
        /// </summary>
        [SerializeField]
        private string m_VerticalAxis = "Vertical";

        /// <summary>
        /// Name of the submit button.
        /// </summary>
        [SerializeField]
        private string m_SubmitButton = "Submit";

        /// <summary>
        /// Name of the submit button.
        /// </summary>
        [SerializeField]
        private string m_CancelButton = "Cancel";

        [SerializeField]
        private float m_InputActionsPerSecond = 10;

        [SerializeField]
        private float m_RepeatDelay = 0.5f;

        [SerializeField]
        [FormerlySerializedAs("m_AllowActivationOnMobileDevice")]
        private bool m_ForceModuleActive;

        [Obsolete("allowActivationOnMobileDevice has been deprecated. Use forceModuleActive instead (UnityUpgradable) -> forceModuleActive")]
        public bool allowActivationOnMobileDevice
        {
            get { return m_ForceModuleActive; }
            set { m_ForceModuleActive = value; }
        }

        /// <summary>
        /// Force this module to be active.
        /// </summary>
        /// <remarks>
        /// If there is no module active with higher priority (ordered in the inspector) this module will be forced active even if valid enabling conditions are not met.
        /// </remarks>
        public bool forceModuleActive
        {
            get { return m_ForceModuleActive; }
            set { m_ForceModuleActive = value; }
        }

        /// <summary>
        /// Number of keyboard / controller inputs allowed per second.
        /// </summary>
        public float inputActionsPerSecond
        {
            get { return m_InputActionsPerSecond; }
            set { m_InputActionsPerSecond = value; }
        }

        /// <summary>
        /// Delay in seconds before the input actions per second repeat rate takes effect.
        /// </summary>
        /// <remarks>
        /// If the same direction is sustained, the inputActionsPerSecond property can be used to control the rate at which events are fired. However, it can be desirable that the first repetition is delayed, so the user doesn't get repeated actions by accident.
        /// </remarks>
        public float repeatDelay
        {
            get { return m_RepeatDelay; }
            set { m_RepeatDelay = value; }
        }

        /// <summary>
        /// Name of the horizontal axis for movement (if axis events are used).
        /// </summary>
        public string horizontalAxis
        {
            get { return m_HorizontalAxis; }
            set { m_HorizontalAxis = value; }
        }

        /// <summary>
        /// Name of the vertical axis for movement (if axis events are used).
        /// </summary>
        public string verticalAxis
        {
            get { return m_VerticalAxis; }
            set { m_VerticalAxis = value; }
        }

        /// <summary>
        /// Maximum number of input events handled per second.
        /// </summary>
        public string submitButton
        {
            get { return m_SubmitButton; }
            set { m_SubmitButton = value; }
        }

        /// <summary>
        /// Input manager name for the 'cancel' button.
        /// </summary>
        public string cancelButton
        {
            get { return m_CancelButton; }
            set { m_CancelButton = value; }
        }

        private bool ShouldIgnoreEventsOnNoFocus()
        {
            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.Windows:
                case OperatingSystemFamily.Linux:
                case OperatingSystemFamily.MacOSX:
#if UNITY_EDITOR
                    if (UnityEditor.EditorApplication.isRemoteConnected)
                        return false;
#endif
                    return true;
                default:
                    return false;
            }
        }

        public override void UpdateModule()
        {
            if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
            {
                if (m_InputPointerEvent != null && m_InputPointerEvent.pointerDrag != null && m_InputPointerEvent.dragging)
                {
                    ReleaseMouse(m_InputPointerEvent, m_InputPointerEvent.pointerCurrentRaycast.gameObject);
                }

                m_InputPointerEvent = null;

                return;
            }

            m_LastMousePosition = m_MousePosition;
            m_MousePosition = input.mousePosition;
        }

        private void ReleaseMouse(PointerEventData pointerEvent, GameObject currentOverGo)
        {
            
            ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
            
            var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
            
            // PointerClick and Drop events
            if ((pointerEvent.pointerPress == pointerUpHandler || !Input.GetKey(KeyCode.Space)) && pointerEvent.eligibleForClick)
            {
                if(useRaycastObject)
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                else {
                    // Verifica se o deslize foi para esquerda ou direita
                    // TODO Testar a necessidade de uma margem de erro para evitar deslizes indesejados
                    float diff = m_LastMousePosition.x - m_MousePosition.x;

                    // Uma flag para indicar se o movimento foi de toque ou deslize
                    swipe = false;
                    // Se o valor for negativo é um deslize para esquerda, caso contrário para direita
                    // À esquerda o item anterior é selecionado, a direita o item posterior é selecionado
                    // O movimento é realizado de forma cíclica pela interface
                    if(!dragging && diff < 0 && !interactableList.isEmpty) {
                        currentOverGo = interactableList.Next();
                        eventSystem.SetSelectedGameObject(currentOverGo);
                        swipe = true;
                        ExecuteEvents.ExecuteHierarchy(currentOverGo,pointerEvent,pointerDescriptionHandler);
                    }
                    else if(!dragging && diff > 0 && !interactableList.isEmpty) {
                        currentOverGo = interactableList.Previous();
                        eventSystem.SetSelectedGameObject(currentOverGo);
                        swipe = true;
                        ExecuteEvents.ExecuteHierarchy(currentOverGo,pointerEvent,pointerDescriptionHandler);
                    }
                    // Se o minimo de clicks para interação foi realizado, permite a interação;
                    if(pointerEvent.clickCount >= minClicks) {
                        ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                        pointerEvent.clickCount = 0;
                    }
                    // Se o movimento foi de toque, executa a descrição de audio
                    else if(!swipe) {
                        ExecuteEvents.ExecuteHierarchy(currentOverGo,pointerEvent,pointerDescriptionHandler);
                    }
                }
            }
            else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
            }

            pointerEvent.eligibleForClick = false;
            pointerEvent.pointerPress = null;
            pointerEvent.rawPointerPress = null;

            if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

            pointerEvent.dragging = false;
            pointerEvent.pointerDrag = null;

            // redo pointer enter / exit to refresh state
            // so that if we moused over something that ignored it before
            // due to having pressed on something else
            // it now gets it.
            if (currentOverGo != pointerEvent.pointerEnter)
            {
                HandlePointerExitAndEnter(pointerEvent, null);
                HandlePointerExitAndEnter(pointerEvent, currentOverGo);
            }

            m_InputPointerEvent = pointerEvent;
        }

        public override bool IsModuleSupported()
        {
            return m_ForceModuleActive || input.mousePresent || input.touchSupported;
        }

        public override bool ShouldActivateModule()
        {
            if (!base.ShouldActivateModule())
                return false;

            var shouldActivate = m_ForceModuleActive;
            shouldActivate |= input.GetButtonDown(m_SubmitButton);
            shouldActivate |= input.GetButtonDown(m_CancelButton);
            shouldActivate |= !Mathf.Approximately(input.GetAxisRaw(m_HorizontalAxis), 0.0f);
            shouldActivate |= !Mathf.Approximately(input.GetAxisRaw(m_VerticalAxis), 0.0f);
            shouldActivate |= (m_MousePosition - m_LastMousePosition).sqrMagnitude > 0.0f;
            shouldActivate |= input.GetMouseButtonDown(0);

            if (input.touchCount > 0)
                shouldActivate = true;

            return shouldActivate;
        }

        /// <summary>
        /// See BaseInputModule.
        /// </summary>
        public override void ActivateModule()
        {
            if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
                return;

            base.ActivateModule();
            m_MousePosition = input.mousePosition;
            m_LastMousePosition = input.mousePosition;

            var toSelect = eventSystem.currentSelectedGameObject;
            if (toSelect == null)
                toSelect = eventSystem.firstSelectedGameObject;

            eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
        }

        /// <summary>
        /// See BaseInputModule.
        /// </summary>
        public override void DeactivateModule()
        {
            base.DeactivateModule();
            ClearSelection();
        }

        public override void Process()
        {
            if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus() || DescriptionPlayer.playingTutorial)
                return;

            bool usedEvent = SendUpdateEventToSelectedObject();

            // case 1004066 - touch / mouse events should be processed before navigation events in case
            // they change the current selected gameobject and the submit button is a touch / mouse button.

            // touch needs to take precedence because of the mouse emulation layer
            if (!ProcessTouchEvents() && input.mousePresent)
                ProcessMouseEvent();

            // if (eventSystem.sendNavigationEvents)
            // {
            //     if (!usedEvent)
            //         usedEvent |= SendMoveEventToSelectedObject();

            //     if (!usedEvent)
            //         SendSubmitEventToSelectedObject();
            // }
        }

        private bool ProcessTouchEvents()
        {
            for (int i = 0; i < input.touchCount; ++i)
            {
                Touch touch = input.GetTouch(i);

                if (touch.type == TouchType.Indirect)
                    continue;

                bool released;
                bool pressed;
                var pointer = GetTouchPointerEventData(touch, out pressed, out released);
                ProcessTouchPress(pointer, pressed, released);

                if (!released && i == 0)
                {
                    ProcessMove(pointer);
                    ProcessDrag(pointer);
                }
                else
                    RemovePointerData(pointer);
            }
            return input.touchCount > 0;
        }

        /// <summary>
        /// This method is called by Unity whenever a touch event is processed. Override this method with a custom implementation to process touch events yourself.
        /// </summary>
        /// <param name="pointerEvent">Event data relating to the touch event, such as position and ID to be passed to the touch event destination object.</param>
        /// <param name="pressed">This is true for the first frame of a touch event, and false thereafter. This can therefore be used to determine the instant a touch event occurred.</param>
        /// <param name="released">This is true only for the last frame of a touch event.</param>
        /// <remarks>
        /// This method can be overridden in derived classes to change how touch press events are handled.
        /// </remarks>
        protected void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
        {
            float holdTime = Time.unscaledTime;
            // useRaycastObject = holdTime - lastPressedTime >= 0.5f;
            // lastPressedTime = Time.unscaledTime;

            var currentOverGo = useRaycastObject?pointerEvent.pointerCurrentRaycast.gameObject:interactableList.focusedGo;

            // PointerDown notification
            if (pressed)
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                if(useRaycastObject)
                    DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                // if (pointerEvent.pointerEnter != currentOverGo)
                // {
                //     // send a pointer enter to the touched element if it isn't the one to select...
                //     HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                //     pointerEvent.pointerEnter = currentOverGo;
                // }

                GameObject newPressed;
                if(useRaycastObject)
                    // search for the control that will receive the press
                    // if we can't find a press handler set the press
                    // handler to be what would receive a click.
                    newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                else
                    newPressed = ExecuteEvents.GetEventHandler<IPointerDownHandler>(currentOverGo);
                // didnt find a press handler... search for a click handler
                if (newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);


                float time = Time.unscaledTime;

                var currentPointerEvent = m_InputPointerEvent != null?m_InputPointerEvent:pointerEvent;

                if (newPressed == currentPointerEvent.lastPress)
                {
                    var diffTime = time - currentPointerEvent.clickTime;
                    if (diffTime < 0.3f)
                        pointerEvent.clickCount += currentPointerEvent.clickCount+1;
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
                if(!useRaycastObject && pointerEvent.clickCount >= minClicks) {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);
                }
                
                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = time;

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if (pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);

                m_InputPointerEvent = pointerEvent;
            }

            // PointerUp notification
            if (released)
            {
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                // see if we mouse up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // PointerClick and Drop events
                if ((pointerEvent.pointerPress == pointerUpHandler || input.touchCount > 0) && pointerEvent.eligibleForClick)
                {
                    if(useRaycastObject)
                        ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                    else if(input.touchCount == 1) {
                        float diff = m_LastMousePosition.x - m_MousePosition.x;
                        swipe = false;
                        if(diff < -1 && !interactableList.isEmpty) {
                            swipe = true;
                            currentOverGo = interactableList.Next();
                            eventSystem.SetSelectedGameObject(currentOverGo);
                            ExecuteEvents.ExecuteHierarchy(currentOverGo,pointerEvent,pointerDescriptionHandler);
                        }
                        else if(diff > 1 && !interactableList.isEmpty) {
                            swipe = true;
                            currentOverGo = interactableList.Previous();
                            eventSystem.SetSelectedGameObject(currentOverGo);
                            ExecuteEvents.ExecuteHierarchy(currentOverGo,pointerEvent,pointerDescriptionHandler);
                        }
                        if(m_InputPointerEvent.clickCount >= minClicks) {
                            ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                        }
                        else if(!swipe) {
                            ExecuteEvents.ExecuteHierarchy(currentOverGo,pointerEvent,pointerDescriptionHandler);
                        }
                    }
                }
                else if (pointerEvent.pointerDrag != null && pointerEvent.dragging && input.touchCount > 1)
                {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                // send exit events as we need to simulate this on touch up on touch device
                ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
                pointerEvent.pointerEnter = null;

                // m_InputPointerEvent = pointerEvent;
            }
        }

        /// <summary>
        /// Calculate and send a submit event to the current selected object.
        /// </summary>
        /// <returns>If the submit event was used by the selected object.</returns>
        protected bool SendSubmitEventToSelectedObject()
        {
            if (eventSystem.currentSelectedGameObject == null)
                return false;

            var data = GetBaseEventData();
            if (input.GetButtonDown(m_SubmitButton))
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);

            if (input.GetButtonDown(m_CancelButton))
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
            return data.used;
        }

        private Vector2 GetRawMoveVector()
        {
            Vector2 move = Vector2.zero;
            move.x = input.GetAxisRaw(m_HorizontalAxis);
            move.y = input.GetAxisRaw(m_VerticalAxis);

            if (input.GetButtonDown(m_HorizontalAxis))
            {
                if (move.x < 0)
                    move.x = -1f;
                if (move.x > 0)
                    move.x = 1f;
            }
            if (input.GetButtonDown(m_VerticalAxis))
            {
                if (move.y < 0)
                    move.y = -1f;
                if (move.y > 0)
                    move.y = 1f;
            }
            return move;
        }

        /// <summary>
        /// Calculate and send a move event to the current selected object.
        /// </summary>
        /// <returns>If the move event was used by the selected object.</returns>
        protected bool SendMoveEventToSelectedObject()
        {
            float time = Time.unscaledTime;

            Vector2 movement = GetRawMoveVector();
            if (Mathf.Approximately(movement.x, 0f) && Mathf.Approximately(movement.y, 0f))
            {
                m_ConsecutiveMoveCount = 0;
                return false;
            }

            bool similarDir = (Vector2.Dot(movement, m_LastMoveVector) > 0);

            // If direction didn't change at least 90 degrees, wait for delay before allowing consequtive event.
            if (similarDir && m_ConsecutiveMoveCount == 1)
            {
                if (time <= m_PrevActionTime + m_RepeatDelay)
                    return false;
            }
            // If direction changed at least 90 degree, or we already had the delay, repeat at repeat rate.
            else
            {
                if (time <= m_PrevActionTime + 1f / m_InputActionsPerSecond)
                    return false;
            }

            var axisEventData = GetAxisEventData(movement.x, movement.y, 0.6f);

            if (axisEventData.moveDir != MoveDirection.None)
            {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
                if (!similarDir)
                    m_ConsecutiveMoveCount = 0;
                m_ConsecutiveMoveCount++;
                m_PrevActionTime = time;
                m_LastMoveVector = movement;
            }
            else
            {
                m_ConsecutiveMoveCount = 0;
            }

            return axisEventData.used;
        }

        protected void ProcessMouseEvent()
        {
            ProcessMouseEvent(0);
        }

        [Obsolete("This method is no longer checked, overriding it with return true does nothing!")]
        protected virtual bool ForceAutoSelect()
        {
            return false;
        }

        /// <summary>
        /// Process all mouse events.
        /// </summary>
        protected void ProcessMouseEvent(int id)
        {
            var mouseData = GetMousePointerEventData(id);
            var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;

            m_CurrentFocusedGameObject = useRaycastObject?leftButtonData.buttonData.pointerCurrentRaycast.gameObject:interactableList.focusedGo;

            // Process the first mouse button fully
            ProcessMousePress(leftButtonData);
            ProcessMove(leftButtonData.buttonData);
            ProcessDrag(leftButtonData.buttonData);

            // Now process right / middle clicks
            // ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData);
            // ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
            // ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
            // ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);

            // if (!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f))
            // {
            //     var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(leftButtonData.buttonData.pointerCurrentRaycast.gameObject);
            //     ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
            // }
        }

        protected bool SendUpdateEventToSelectedObject()
        {
            if (eventSystem.currentSelectedGameObject == null)
                return false;

            var data = GetBaseEventData();
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
            return data.used;
        }

        /// <summary>
        /// Calculate and process any mouse button state changes.
        /// </summary>
        protected void ProcessMousePress(MouseButtonEventData data)
        {
            var pointerEvent = data.buttonData;

            float holdTime = Time.unscaledTime;
            // useRaycastObject = holdTime - lastPressedTime >= 0.5f;
            // lastPressedTime = Time.unscaledTime;
            
            var currentOverGo = useRaycastObject?pointerEvent.pointerCurrentRaycast.gameObject:interactableList.focusedGo;

            

            // PointerDown notification
            if (data.PressedThisFrame())
            {
                
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
                
                if(useRaycastObject)
                    DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                GameObject newPressed;
                if(useRaycastObject)
                    // search for the control that will receive the press
                    // if we can't find a press handler set the press
                    // handler to be what would receive a click.
                    newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);
                else
                    newPressed = ExecuteEvents.GetEventHandler<IPointerDownHandler>(currentOverGo);
                // didnt find a press handler... search for a click handler
                if (newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
                

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
                if(!useRaycastObject && pointerEvent.clickCount >= minClicks) {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = time;

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if (pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);

                m_InputPointerEvent = pointerEvent;
                holding = true;
                lastPressedTime = Time.unscaledTime;
            }
            // PointerUp notification
            if (data.ReleasedThisFrame())
            {
                holding = false;
                moving = false;
                ReleaseMouse(pointerEvent, currentOverGo);
                dragging = false;
            }
        }

        protected GameObject GetCurrentFocusedGameObject()
        {
            return m_CurrentFocusedGameObject;
        }

        // Ao iniciar o module é realizada uma busca de objetos interagíveis.
        // TODO Verificar a possibilidade de popular a lista de outros meios
        protected override void Start() {
            base.Start();
            useRaycastObject = false;
            interactableList = ListSingleton.instance;
            // interactableList.ClearList();
            //interactableList.FindInteractables();
            if(!interactableList.isEmpty) {
                eventSystem.SetSelectedGameObject(interactableList.focusedGo);
                DescriptionPlayer dp = interactableList.focusedGo.GetComponent(typeof(DescriptionPlayer)) as DescriptionPlayer;
                dp.OnDescriptorPress(null);
            }
        }

        // Só permite arrastar em situações específicas. Segurando espaço para PC e com 2 toques para touch
        protected override void ProcessDrag(PointerEventData pointerEvent) {
            if((Input.GetKey(KeyCode.Space) || input.touchCount > 1) && !moving){
                dragging = true;
                base.ProcessDrag(pointerEvent);
            }
        }

        // A ideia é que a interação possa ser feita movendo o mouse enquanto clica/toca na tela e move
        // o dedo. Interação mais fluida.
        protected override void ProcessMove(PointerEventData pointerEvent) {
            float time = Time.unscaledTime;
            float diff = time - lastPressedTime;
                if(holding && !dragging && diff > 0.5f && pointerEvent.clickCount < 2) {
                    var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;
                    if(currentOverGo == null) return;
                    int index = interactableList.Find(currentOverGo);
                    if(index >= 0) {
                        moving = true;
                        currentOverGo = interactableList.Get(index);
                        eventSystem.SetSelectedGameObject(currentOverGo);
                        ExecuteEvents.ExecuteHierarchy(currentOverGo,pointerEvent,pointerDescriptionHandler);
                    }
                           
                }
                

                base.ProcessMove(pointerEvent);
        }
    }
}
