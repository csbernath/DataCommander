namespace DataCommander.Providers.Connection
{
    using System.ComponentModel;
    using System.Windows.Forms;

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
            this.providersComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataSourcesComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.userIdTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.initialCatalogComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.integratedSecurityCheckBox = new System.Windows.Forms.CheckBox();
            this.refreshButton = new System.Windows.Forms.Button();
            this.connectionNameTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.testButton = new System.Windows.Forms.Button();
            this.oleDbProvidersComboBox = new System.Windows.Forms.ComboBox();
            this.oleDbProviderLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // providersComboBox
            // 
            this.providersComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.providersComboBox.FormattingEnabled = true;
            this.providersComboBox.Location = new System.Drawing.Point(109, 28);
            this.providersComboBox.Name = "providersComboBox";
            this.providersComboBox.Size = new System.Drawing.Size(378, 21);
            this.providersComboBox.TabIndex = 2;
            this.providersComboBox.SelectedIndexChanged += new System.EventHandler(this.providersComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Provider:";
            // 
            // dataSourcesComboBox
            // 
            this.dataSourcesComboBox.FormattingEnabled = true;
            this.dataSourcesComboBox.Location = new System.Drawing.Point(109, 82);
            this.dataSourcesComboBox.Name = "dataSourcesComboBox";
            this.dataSourcesComboBox.Size = new System.Drawing.Size(378, 21);
            this.dataSourcesComboBox.TabIndex = 3;
            this.dataSourcesComboBox.DropDown += new System.EventHandler(this.dataSourcesComboBox_DropDown);
            this.dataSourcesComboBox.SelectedIndexChanged += new System.EventHandler(this.dataSourcesComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Data source:";
            // 
            // userIdTextBox
            // 
            this.userIdTextBox.Location = new System.Drawing.Point(109, 132);
            this.userIdTextBox.Name = "userIdTextBox";
            this.userIdTextBox.Size = new System.Drawing.Size(378, 20);
            this.userIdTextBox.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "User ID:";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.AcceptsReturn = true;
            this.passwordTextBox.Location = new System.Drawing.Point(109, 158);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(378, 20);
            this.passwordTextBox.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 161);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Password:";
            // 
            // initialCatalogComboBox
            // 
            this.initialCatalogComboBox.FormattingEnabled = true;
            this.initialCatalogComboBox.Location = new System.Drawing.Point(109, 184);
            this.initialCatalogComboBox.Name = "initialCatalogComboBox";
            this.initialCatalogComboBox.Size = new System.Drawing.Size(378, 21);
            this.initialCatalogComboBox.TabIndex = 8;
            this.initialCatalogComboBox.DropDown += new System.EventHandler(this.initialCatalogComboBox_DropDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 187);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Initial catalog:";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(412, 218);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 9;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OK_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(493, 218);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // integratedSecurityCheckBox
            // 
            this.integratedSecurityCheckBox.AutoSize = true;
            this.integratedSecurityCheckBox.CheckAlign = System.Drawing.ContentAlignment.TopRight;
            this.integratedSecurityCheckBox.Location = new System.Drawing.Point(1, 109);
            this.integratedSecurityCheckBox.Name = "integratedSecurityCheckBox";
            this.integratedSecurityCheckBox.Size = new System.Drawing.Size(122, 17);
            this.integratedSecurityCheckBox.TabIndex = 5;
            this.integratedSecurityCheckBox.Text = "Integrated security:  ";
            this.integratedSecurityCheckBox.UseVisualStyleBackColor = true;
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(493, 80);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(75, 23);
            this.refreshButton.TabIndex = 4;
            this.refreshButton.Text = "&Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // connectionNameTextBox
            // 
            this.connectionNameTextBox.Location = new System.Drawing.Point(109, 2);
            this.connectionNameTextBox.Name = "connectionNameTextBox";
            this.connectionNameTextBox.Size = new System.Drawing.Size(378, 20);
            this.connectionNameTextBox.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(2, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Connection name:";
            // 
            // testButton
            // 
            this.testButton.Location = new System.Drawing.Point(109, 218);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(75, 23);
            this.testButton.TabIndex = 16;
            this.testButton.Text = "&Test";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // oleDbProvidersComboBox
            // 
            this.oleDbProvidersComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.oleDbProvidersComboBox.FormattingEnabled = true;
            this.oleDbProvidersComboBox.Location = new System.Drawing.Point(109, 55);
            this.oleDbProvidersComboBox.MaxDropDownItems = 16;
            this.oleDbProvidersComboBox.Name = "oleDbProvidersComboBox";
            this.oleDbProvidersComboBox.Size = new System.Drawing.Size(378, 21);
            this.oleDbProvidersComboBox.TabIndex = 17;
            // 
            // oleDbProviderLabel
            // 
            this.oleDbProviderLabel.AutoSize = true;
            this.oleDbProviderLabel.Location = new System.Drawing.Point(2, 57);
            this.oleDbProviderLabel.Name = "oleDbProviderLabel";
            this.oleDbProviderLabel.Size = new System.Drawing.Size(81, 13);
            this.oleDbProviderLabel.TabIndex = 18;
            this.oleDbProviderLabel.Text = "OleDb provider:";
            // 
            // ConnectionStringBuilderForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(573, 247);
            this.Controls.Add(this.oleDbProviderLabel);
            this.Controls.Add(this.oleDbProvidersComboBox);
            this.Controls.Add(this.testButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.connectionNameTextBox);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.integratedSecurityCheckBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.initialCatalogComboBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.userIdTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataSourcesComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.providersComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConnectionStringBuilderForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ConnectionStringBuilderForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComboBox providersComboBox;
        private Label label1;
        private ComboBox dataSourcesComboBox;
        private Label label2;
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
    }
}