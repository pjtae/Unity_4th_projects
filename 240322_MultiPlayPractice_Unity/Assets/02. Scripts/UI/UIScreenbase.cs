using System.Threading;

namespace MP.UI
{
    public class UIScreenbase : UIBase
    {
        protected override void Awake()
        {
            base.Awake();

            UIManager.instance.RegisterScreen(this);
        }

        public override void Show()
        {
            base.Show();

            UIManager.instance.SetScreen(this);
        }
    }
}
