using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MP.UI
{
    [RequireComponent(typeof(Canvas))]
    public abstract class UIBase : MonoBehaviour, IUI
    {
        public int sortingOrder
        {
            get => _canvas.sortingOrder;
            set => _canvas.sortingOrder = value;
        }

        public bool inputActionEnable
        {
            get => _inputActionEnable;
            set
            {
                if(_inputActionEnable ==value)
                    return;

                _inputActionEnable = value;
                onInputActionEnableChanged?.Invoke(value);
            }
        }

        private bool _inputActionEnable;
        private Canvas _canvas;
        private GraphicRaycaster _module;
        private EventSystem _eventSystem;

        public event Action<bool> onInputActionEnableChanged;
        //event 연산자로 제어가능하게함

        protected virtual void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _module = GetComponent<GraphicRaycaster>();
            _eventSystem = EventSystem.current;
        }
        public virtual void Show()
        {
            //UIManager.instance.PushPopup(this);
            _canvas.enabled = true;
        }

        public virtual void Hide()
        {
            //UIManager.instance.PopPopup(this);
            _canvas.enabled = false;
        }

        public virtual void InputAction()
        {
            
        }

        public bool Raycast(List<RaycastResult> results)
        {
            int count = results.Count;
            PointerEventData pointerEventData = new PointerEventData(_eventSystem); //마우스 이벤트 등록
            pointerEventData.position = Input.mousePosition;
            _module.Raycast(pointerEventData, results);
            return count < results.Count;
        }

    }
}

