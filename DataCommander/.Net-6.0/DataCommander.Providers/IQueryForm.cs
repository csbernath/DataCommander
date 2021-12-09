using System;
using System.Data;
using System.Drawing;

namespace DataCommander.Providers
{
    public interface IQueryForm
    {
        void ShowMessage(Exception exception);
        void ShowDataSet(DataSet dataSet);
        void SetStatusbarPanelText(string text, Color color);
        ColorTheme ColorTheme { get; }
    }
}