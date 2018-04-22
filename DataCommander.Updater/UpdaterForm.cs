using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataCommander.Updater
{
    public partial class UpdaterForm : Form
    {
        private readonly string _applicationExeFileName;

        public UpdaterForm(string applicationExeFileName)
        {
            _applicationExeFileName = applicationExeFileName;
            InitializeComponent();
        }

        private void Start()
        {
        }
    }
}