using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;

namespace MP.UI
{
    public class UIWarningWindow : UIBase
    {
        private TMP_Text _message;
        private Button _confirm;

        protected override void Awake()
        {
            base.Awake(); 
            _message = transform.Find("Panel/Text (TMP) - Message").GetComponent<TMP_Text>();
            _confirm = transform.Find("Panel/Button - Confirm").GetComponent<Button>();
            _confirm.onClick.AddListener(Hide);
        }

        //경고창은 전부 다를수 있기때문에 새로 지정
        public void Show(string message)
        {
            base.Show();
            _message.text = message;
        }
    }
}
