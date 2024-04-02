using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace MP.UI
{
    public class RoomListSlot : MonoBehaviour
    {
        public int roomIndex { get; set; }

        private TMP_Text _roomName;
        private TMP_Text _PlayerRatio;
        private Button _select;

        public event Action<int> onSelected;


        private void Awake()
        {
            _roomName = transform.Find("Text (TMP) - RoomName").GetComponent<TMP_Text>();
            _PlayerRatio = transform.Find("Text (TMP) - PlayerRatio").GetComponent<TMP_Text>();
            _select = GetComponent<Button>();
            _select.onClick.AddListener(() => onSelected?.Invoke(roomIndex));
        }

        public void Refresh(string roomName, int currentPlayerInRoom, int maxPlayerInRoom)
        {
            _roomName.text = roomName;
            _PlayerRatio.text = $"{currentPlayerInRoom} / {maxPlayerInRoom})";
        }
    }
}

