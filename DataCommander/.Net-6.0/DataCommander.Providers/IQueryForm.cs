using System;
using System.Data;

namespace DataCommander.Providers
{
    public interface IQueryForm
    {
        void ShowMessage(Exception exception);
        void ShowDataSet(DataSet dataSet);
    }
}