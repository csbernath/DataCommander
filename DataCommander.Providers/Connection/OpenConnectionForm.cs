namespace DataCommander.Providers
{
    using System;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;
    using DataCommander.Foundation.Diagnostics;

    /// <summary>
    /// Summary description for OpenConnectionForm.
    /// </summary>
    internal sealed class OpenConnectionForm : System.Windows.Forms.Form
    {
        private static ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox tbTimer;
        private TextBox textBox;
        private System.Windows.Forms.Timer timer;
        private System.ComponentModel.IContainer components;
        private ConnectionProperties connectionProperties;
        private AsyncConnector connector;
        private Stopwatch stopwatch = new Stopwatch();
        private EventWaitHandle handleCreatedEvent = new EventWaitHandle( false, EventResetMode.ManualReset );

        public long Duration
        {
            get
            {
                return this.connector.Duration;
            }
        }

        private void EndConnectionOpenInvoke( Exception exception )
        {
            DialogResult dialogResult;

            if (exception == null)
            {
                dialogResult = DialogResult.OK;
            }
            else
            {
                log.Write( LogLevel.Error, exception.ToString() );
                IProvider provider = connectionProperties.provider;
                string message;

                if (provider != null)
                {
                    try
                    {
                        message = provider.GetExceptionMessage( exception );
                    }
                    catch
                    {
                        message = exception.ToString();
                    }
                }
                else
                {
                    message = exception.ToString();
                }

                MessageBox.Show(
                    this,
                    message,
                    "Failed to open connection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error );

                dialogResult = DialogResult.Cancel;
            }

            DialogResult = dialogResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        private void EndConnectionOpen( Exception exception )
        {
            timer.Enabled = false;
            this.handleCreatedEvent.WaitOne();
            this.Invoke( new EndConnectionOpen( EndConnectionOpenInvoke ), new object[] { exception } );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public OpenConnectionForm( ConnectionProperties connectionProperties )
        {
            this.InitializeComponent();
            this.HandleCreated += new EventHandler( OpenConnectionForm_HandleCreated );

            stopwatch.Start();
            timer.Enabled = true;

            DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder();
            dbConnectionStringBuilder.ConnectionString = connectionProperties.connectionString;
            object dataSourceObject;
            bool contains = dbConnectionStringBuilder.TryGetValue( ConnectionStringProperty.DataSource, out dataSourceObject );
            object userId;
            dbConnectionStringBuilder.TryGetValue( ConnectionStringProperty.UserId, out userId );
            string dataSource = (string) dataSourceObject;
            textBox.Text = string.Format( "Connection name: {0}\r\nProvider name: {1}\r\nData Source: {2}\r\nUserId: {3}", connectionProperties.connectionName, connectionProperties.providerName, dataSource, userId );
            this.connectionProperties = connectionProperties;
            Cursor = Cursors.AppStarting;
            connector = new AsyncConnector( connectionProperties );
            connector.BeginOpen( new EndConnectionOpen( EndConnectionOpen ) );
        }

        private void OpenConnectionForm_HandleCreated( object sender, EventArgs e )
        {
            this.handleCreatedEvent.Set();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbTimer = new System.Windows.Forms.TextBox();
            this.textBox = new System.Windows.Forms.TextBox();
            this.timer = new System.Windows.Forms.Timer( this.components );
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point( 296, 81 );
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size( 75, 23 );
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler( this.btnCancel_Click );
            // 
            // tbTimer
            // 
            this.tbTimer.BackColor = System.Drawing.SystemColors.Control;
            this.tbTimer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbTimer.Location = new System.Drawing.Point( 8, 86 );
            this.tbTimer.Name = "tbTimer";
            this.tbTimer.Size = new System.Drawing.Size( 64, 14 );
            this.tbTimer.TabIndex = 1;
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point( 8, 6 );
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.Size = new System.Drawing.Size( 647, 75 );
            this.textBox.TabIndex = 2;
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler( this.timer_Tick );
            // 
            // OpenConnectionForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size( 5, 14 );
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size( 667, 106 );
            this.Controls.Add( this.textBox );
            this.Controls.Add( this.tbTimer );
            this.Controls.Add( this.btnCancel );
            this.Font = new System.Drawing.Font( "Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (238)) );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "OpenConnectionForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Opening connection...";
            this.ResumeLayout( false );
            this.PerformLayout();

        }
        #endregion

        private void btnCancel_Click( object sender, System.EventArgs e )
        {
            connector.Cancel();
        }

        private void timer_Tick( object sender, System.EventArgs e )
        {
            long ticks = stopwatch.ElapsedTicks;
            this.tbTimer.Text = StopwatchTimeSpan.ToString( ticks, 3 );
        }
    }

    delegate void EndConnectionOpen( Exception e );
}