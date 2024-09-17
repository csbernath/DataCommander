using System.Drawing;
using System.Windows.Forms;
using Foundation.Assertions;
using Foundation.Configuration;

namespace Foundation.Windows.Forms;

public static class FormPosition
{
    public static void Save(Form form, ApplicationData applicationData)
    {
        System.Type type = form.GetType();
        string nodeName = ConfigurationNodeName.FromType(type);
        ConfigurationNode node = applicationData.CreateNode(nodeName);
        ConfigurationAttributeCollection attributes = node.Attributes;

        if (form.WindowState == FormWindowState.Minimized)
            form.WindowState = FormWindowState.Normal;

        attributes.SetAttributeValue("WindowState", form.WindowState);
        attributes.SetAttributeValue("Left", form.Left);
        attributes.SetAttributeValue("Top", form.Top);
        attributes.SetAttributeValue("Width", form.ClientSize.Width);
        attributes.SetAttributeValue("Height", form.ClientSize.Height);
    }

    public static void Load(ApplicationData applicationData, Form form)
    {
        Assert.IsTrue(applicationData != null);
        Assert.IsTrue(form != null);

        System.Type type = form.GetType();
        string nodeName = ConfigurationNodeName.FromType(type);
        ConfigurationNode node = applicationData.CreateNode(nodeName);
        node.Attributes.TryGetAttributeValue("WindowState", FormWindowState.Normal, out FormWindowState windowState);
        form.WindowState = windowState;

        if (windowState == FormWindowState.Normal)
        {
            if (node.Attributes.ContainsKey("Left"))
            {
                form.StartPosition = FormStartPosition.Manual;
                ConfigurationAttributeCollection attributes = node.Attributes;
                form.Left = attributes["Left"].GetValue<int>();
                form.Top = attributes["Top"].GetValue<int>();
                int width = attributes["Width"].GetValue<int>();
                int height = attributes["Height"].GetValue<int>();
                form.ClientSize = new Size(width, height);
            }
        }
    }
}