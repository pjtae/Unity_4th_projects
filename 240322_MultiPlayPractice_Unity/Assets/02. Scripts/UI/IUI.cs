using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MP.UI
{
    public interface IUI
    {
        int sortingOrder { get; set; }
        bool inputActionEnable { get; set; }

        void Show();
        void Hide();
        void InputAction();
        bool Raycast(List<RaycastResult> results);
    }
}
