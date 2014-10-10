namespace DataCommander.Foundation.Windows.Forms
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.Windows.Forms;
    using DataCommander.Foundation.Configuration;    

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
            Type type = form.GetType();
            String nodeName = ConfigurationNodeName.FromType( type );
            ConfigurationNode node = applicationData.CreateNode( nodeName );
            ConfigurationAttributeCollection attributes = node.Attributes;

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
            Contract.Requires(applicationData != null);
            Contract.Requires(form != null);

            Type type = form.GetType();
            String nodeName = ConfigurationNodeName.FromType( type );
            ConfigurationNode node = applicationData.CreateNode( nodeName );
            FormWindowState windowState;
            node.Attributes.TryGetAttributeValue<FormWindowState>( "WindowState", FormWindowState.Normal, out windowState );
            form.WindowState = windowState;

            if (windowState == FormWindowState.Normal)
            {
                if (node.Attributes.ContainsKey( "Left" ))
                {
                    form.StartPosition = FormStartPosition.Manual;
                    ConfigurationAttributeCollection attributes = node.Attributes;
                    form.Left = attributes[ "Left" ].GetValue<Int32>();
                    form.Top = attributes[ "Top" ].GetValue<Int32>();
                    Int32 width = attributes[ "Width" ].GetValue<Int32>();
                    Int32 height = attributes[ "Height" ].GetValue<Int32>();
                    form.ClientSize = new Size( width, height );
                }
            }
        }
    }
}