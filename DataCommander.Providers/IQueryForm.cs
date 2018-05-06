using System.Drawing;

namespace DataCommander.Providers
{
    public interface IQueryForm
    {
        void SetStatusbarPanelText(string text, Color color);
    }
}