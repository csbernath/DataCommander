namespace DataCommander.Foundation.Windows.Forms
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// 
    /// </summary>
    public static class ControlExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        public static void Invoke( this Control control, Action action )
        {
            control.Invoke( action );
        }
    }
}