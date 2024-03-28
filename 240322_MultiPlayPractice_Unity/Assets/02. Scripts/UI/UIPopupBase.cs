using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MP.UI
{
    [RequireComponent(typeof(Canvas))]
    public abstract class UIPopupBase : MonoBehaviour, IUI
    {
        public int sortingOrder
        {
            get => _canvas.sortingOrder;
            set => _canvas.sortingOrder = value;
        }

        public bool inputActionEnable { get; set; }

        private Canvas _canvas;
        private GraphicRaycaster _module;
        private EventSystem _eventSystem;


        protected virtual void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _module = GetComponent<GraphicRaycaster>();
            _eventSystem = EventSystem.current;
            UIManager.instance.RegisterPopup(this);
        }
        public void Show()
        {
            UIManager.instance.PushPopup(this);
            _canvas.enabled = true;
        }

        public void Hide()
        {
            UIManager.instance.PopPopup(this);
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

