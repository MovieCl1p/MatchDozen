
namespace Core.Gui.ViewManager
{
    public class View
    {
        public string ViewId { get; private set; }
        public object Options { get; private set; }

        public View(string viewId, object options = null)
        {
            ViewId = viewId;
            Options = options;
        }
    }
}
