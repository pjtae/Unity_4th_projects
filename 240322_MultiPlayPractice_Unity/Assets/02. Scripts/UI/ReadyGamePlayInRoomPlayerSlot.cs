using UnityEngine;
using TMPro;

namespace UI.Slot
{
    public class ReadyGamePlayInRoomPlayerSlot : MonoBehaviour
    {
        public bool isReady
        {
            set
            {
                _isReady.enabled = value;
            }
        }

        public string nickname
        {
            get => _nickname.text;
            set
            {
                _nickname.text = value;
            }
        }

        private TMP_Text _nickname;
        private TMP_Text _isReady;

        private void Awake()
        {
            _isReady = transform.Find("Text (TMP) - isReady").GetComponent<TMP_Text>();
            _nickname = transform.Find("Text (TMP) - Nickname").GetComponent<TMP_Text>();
        }
    }

}
