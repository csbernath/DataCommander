using System.Drawing;
using System.Windows.Forms;
using Foundation.Configuration;

namespace Foundation.Windows.Forms
{
    /// <exclude/>
    public static class FormPosition
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="applicationData"></param>
        public static void Save(
            Form form,
            ApplicationData applicationData )
        {
            var type = form.GetType();
            var nodeName = ConfigurationNodeName.FromType( type );
            var node = applicationData.CreateNode( nodeName );
            var attributes = node.Attributes;

            if (form.WindowState == FormWindowState.Minimized)
            {
                form.WindowState = FormWindowState.Normal;
            }

            attributes.SetAttributeValue( "WindowState", form.WindowState );
            attributes.SetAttributeValue( "Left", form.Left );
            attributes.SetAttributeValue( "Top", form.Top );
            attributes.SetAttributeValue( "Width", form.ClientSize.Width );
            attributes.SetAttributeValue( "Height", form.ClientSize.Height );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationData"></param>
        /// <param name="form"></param>
        public static void Load(
            ApplicationData applicationData,
            Form form )
        {
#if CONTRACTS_FULL
            FoundationContract.Requires(applicationData != null);
            FoundationContract.Requires(form != null);
#endif

            var type = form.GetType();
            var nodeName = ConfigurationNodeName.FromType( type );
            var node = applicationData.CreateNode( nodeName );
            FormWindowState windowState;
            node.Attributes.TryGetAttributeValue( "WindowState", FormWindowState.Normal, out windowState );
            form.WindowState = windowState;

            if (windowState == FormWindowState.Normal)
            {
                if (node.Attributes.ContainsKey( "Left" ))
                {
                    form.StartPosition = FormStartPosition.Manual;
                    var attributes = node.Attributes;
                    form.Left = attributes[ "Left" ].GetValue<int>();
                    form.Top = attributes[ "Top" ].GetValue<int>();
                    var width = attributes[ "Width" ].GetValue<int>();
                    var height = attributes[ "Height" ].GetValue<int>();
                    form.ClientSize = new Size( width, height );
                }
            }
        }
    }
}