using System.ComponentModel;
using System.Windows.Forms;

namespace DataCommander.Application.Connection
{
    internal partial class ConnectionStringBuilderForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            providersComboBox = new ComboBox();
            label1 = new Label();
            dataSourcesComboBox = new ComboBox();
            dataSourceLabel = new Label();
            userIdTextBox = new TextBox();
            label3 = new Label();
            passwordTextBox = new TextBox();
            label4 = new Label();
            initialCatalogComboBox = new ComboBox();
            label5 = new Label();
            okButton = new Button();
            cancelButton = new Button();
            integratedSecurityCheckBox = new CheckBox();
            refreshButton = new Button();
            connectionNameTextBox = new TextBox();
            label6 = new Label();
            testButton = new Button();
            oleDbProvidersComboBox = new ComboBox();
            oleDbProviderLabel = new Label();
            trustServerCertificateCheckBox = new CheckBox();
            SuspendLayout();
            // 
            // providersComboBox
            // 
            providersComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            providersComboBox.FormattingEnabled = true;
            providersComboBox.Location = new System.Drawing.Point(127, 36);
            providersComboBox.Margin = new Padding(4, 3, 4, 3);
            providersComboBox.Name = "providersComboBox";
            providersComboBox.Size = new System.Drawing.Size(440, 25);
            providersComboBox.TabIndex = 2;
            providersComboBox.SelectedIndexChanged += HandleProvidersComboBoxSelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(2, 41);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(60, 17);
            label1.TabIndex = 1;
            label1.Text = "Provider:";
            // 
            // dataSourcesComboBox
            // 
            dataSourcesComboBox.FormattingEnabled = true;
            dataSourcesComboBox.Location = new System.Drawing.Point(127, 108);
            dataSourcesComboBox.Margin = new Padding(4, 3, 4, 3);
            dataSourcesComboBox.Name = "dataSourcesComboBox";
            dataSourcesComboBox.Size = new System.Drawing.Size(440, 25);
            dataSourcesComboBox.TabIndex = 3;
            dataSourcesComboBox.DropDown += HandleDataSourcesComboBoxDropDown;
            dataSourcesComboBox.SelectedIndexChanged += HandleDataSourcesComboBoxSelectedIndexChanged;
            // 
            // dataSourceLabel
            // 
            dataSourceLabel.AutoSize = true;
            dataSourceLabel.Location = new System.Drawing.Point(2, 111);
            dataSourceLabel.Margin = new Padding(4, 0, 4, 0);
            dataSourceLabel.Name = "dataSourceLabel";
            dataSourceLabel.Size = new System.Drawing.Size(81, 17);
            dataSourceLabel.TabIndex = 3;
            dataSourceLabel.Text = "Data source:";
            // 
            // userIdTextBox
            // 
            userIdTextBox.Location = new System.Drawing.Point(127, 172);
            userIdTextBox.Margin = new Padding(4, 3, 4, 3);
            userIdTextBox.Name = "userIdTextBox";
            userIdTextBox.Size = new System.Drawing.Size(440, 25);
            userIdTextBox.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(2, 177);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(54, 17);
            label3.TabIndex = 5;
            label3.Text = "User ID:";
            // 
            // passwordTextBox
            // 
            passwordTextBox.AcceptsReturn = true;
            passwordTextBox.Location = new System.Drawing.Point(127, 206);
            passwordTextBox.Margin = new Padding(4, 3, 4, 3);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.PasswordChar = '*';
            passwordTextBox.Size = new System.Drawing.Size(440, 25);
            passwordTextBox.TabIndex = 7;
            passwordTextBox.TextChanged += HandlePasswordTextBoxTextChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(2, 211);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(67, 17);
            label4.TabIndex = 7;
            label4.Text = "Password:";
            // 
            // initialCatalogComboBox
            // 
            initialCatalogComboBox.FormattingEnabled = true;
            initialCatalogComboBox.Location = new System.Drawing.Point(127, 240);
            initialCatalogComboBox.Margin = new Padding(4, 3, 4, 3);
            initialCatalogComboBox.Name = "initialCatalogComboBox";
            initialCatalogComboBox.Size = new System.Drawing.Size(440, 25);
            initialCatalogComboBox.TabIndex = 8;
            initialCatalogComboBox.DropDown += HandleInitialCatalogComboBoxDropDown;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(2, 245);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(88, 17);
            label5.TabIndex = 9;
            label5.Text = "Initial catalog:";
            // 
            // okButton
            // 
            okButton.Location = new System.Drawing.Point(479, 308);
            okButton.Margin = new Padding(4, 3, 4, 3);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(88, 31);
            okButton.TabIndex = 9;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += HandleOKClicked;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new System.Drawing.Point(575, 308);
            cancelButton.Margin = new Padding(4, 3, 4, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(88, 31);
            cancelButton.TabIndex = 10;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // integratedSecurityCheckBox
            // 
            integratedSecurityCheckBox.AutoSize = true;
            integratedSecurityCheckBox.Location = new System.Drawing.Point(4, 143);
            integratedSecurityCheckBox.Margin = new Padding(4, 3, 4, 3);
            integratedSecurityCheckBox.Name = "integratedSecurityCheckBox";
            integratedSecurityCheckBox.Size = new System.Drawing.Size(135, 21);
            integratedSecurityCheckBox.TabIndex = 5;
            integratedSecurityCheckBox.Text = "Integrated security";
            integratedSecurityCheckBox.UseVisualStyleBackColor = true;
            integratedSecurityCheckBox.CheckedChanged += HandleIntegratedSecurityCheckBoxCheckedChanged;
            // 
            // refreshButton
            // 
            refreshButton.Location = new System.Drawing.Point(575, 104);
            refreshButton.Margin = new Padding(4, 3, 4, 3);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new System.Drawing.Size(88, 31);
            refreshButton.TabIndex = 4;
            refreshButton.Text = "&Refresh";
            refreshButton.UseVisualStyleBackColor = true;
            refreshButton.Click += HandleRefreshButtonClicked;
            // 
            // connectionNameTextBox
            // 
            connectionNameTextBox.Location = new System.Drawing.Point(127, 2);
            connectionNameTextBox.Margin = new Padding(4, 3, 4, 3);
            connectionNameTextBox.Name = "connectionNameTextBox";
            connectionNameTextBox.Size = new System.Drawing.Size(440, 25);
            connectionNameTextBox.TabIndex = 1;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(2, 7);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(112, 17);
            label6.TabIndex = 15;
            label6.Text = "Connection name:";
            // 
            // testButton
            // 
            testButton.Location = new System.Drawing.Point(127, 308);
            testButton.Margin = new Padding(4, 3, 4, 3);
            testButton.Name = "testButton";
            testButton.Size = new System.Drawing.Size(88, 31);
            testButton.TabIndex = 16;
            testButton.Text = "&Test";
            testButton.UseVisualStyleBackColor = true;
            testButton.Click += HandleTestButtonClicked;
            // 
            // oleDbProvidersComboBox
            // 
            oleDbProvidersComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            oleDbProvidersComboBox.FormattingEnabled = true;
            oleDbProvidersComboBox.Location = new System.Drawing.Point(127, 71);
            oleDbProvidersComboBox.Margin = new Padding(4, 3, 4, 3);
            oleDbProvidersComboBox.MaxDropDownItems = 16;
            oleDbProvidersComboBox.Name = "oleDbProvidersComboBox";
            oleDbProvidersComboBox.Size = new System.Drawing.Size(440, 25);
            oleDbProvidersComboBox.TabIndex = 17;
            // 
            // oleDbProviderLabel
            // 
            oleDbProviderLabel.AutoSize = true;
            oleDbProviderLabel.Location = new System.Drawing.Point(2, 75);
            oleDbProviderLabel.Margin = new Padding(4, 0, 4, 0);
            oleDbProviderLabel.Name = "oleDbProviderLabel";
            oleDbProviderLabel.Size = new System.Drawing.Size(108, 17);
            oleDbProviderLabel.TabIndex = 18;
            oleDbProviderLabel.Text = "OLE DB provider:";
            // 
            // trustServerCertificateCheckBox
            // 
            trustServerCertificateCheckBox.AutoSize = true;
            trustServerCertificateCheckBox.Location = new System.Drawing.Point(5, 273);
            trustServerCertificateCheckBox.Margin = new Padding(4, 3, 4, 3);
            trustServerCertificateCheckBox.Name = "trustServerCertificateCheckBox";
            trustServerCertificateCheckBox.Size = new System.Drawing.Size(155, 21);
            trustServerCertificateCheckBox.TabIndex = 19;
            trustServerCertificateCheckBox.Text = "Trust server certificate";
            trustServerCertificateCheckBox.UseVisualStyleBackColor = true;
            // 
            // ConnectionStringBuilderForm
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new System.Drawing.Size(668, 348);
            Controls.Add(trustServerCertificateCheckBox);
            Controls.Add(oleDbProviderLabel);
            Controls.Add(oleDbProvidersComboBox);
            Controls.Add(testButton);
            Controls.Add(label6);
            Controls.Add(connectionNameTextBox);
            Controls.Add(refreshButton);
            Controls.Add(integratedSecurityCheckBox);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(label5);
            Controls.Add(initialCatalogComboBox);
            Controls.Add(label4);
            Controls.Add(passwordTextBox);
            Controls.Add(label3);
            Controls.Add(userIdTextBox);
            Controls.Add(dataSourceLabel);
            Controls.Add(dataSourcesComboBox);
            Controls.Add(label1);
            Controls.Add(providersComboBox);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4, 3, 4, 3);
            Name = "ConnectionStringBuilderForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Connection properties";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox providersComboBox;
        private Label label1;
        private ComboBox dataSourcesComboBox;
        private Label dataSourceLabel;
        private TextBox userIdTextBox;
        private Label label3;
        private TextBox passwordTextBox;
        private Label label4;
        private ComboBox initialCatalogComboBox;
        private Label label5;
        private Button okButton;
        private Button cancelButton;
        private CheckBox integratedSecurityCheckBox;
        private Button refreshButton;
        private TextBox connectionNameTextBox;
        private Label label6;
        private Button testButton;
        private ComboBox oleDbProvidersComboBox;
        private Label oleDbProviderLabel;
        private CheckBox trustServerCertificateCheckBox;
    }
}