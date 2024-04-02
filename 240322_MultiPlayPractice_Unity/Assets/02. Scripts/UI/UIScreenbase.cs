using System.Threading;

namespace MP.UI
{
    public class UIScreenbase : UIBase
    {
        protected override void Awake()
        {
            base.Awake();

            UIManager.instance.RegisterPopup(this);
        }
    }
}
