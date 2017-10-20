using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResponseV_Configurator
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private Configuration.Cfg.RootObject config;

        private void Main_Load(object sender, EventArgs e)
        {
            string ConfigFile = $"{Application.StartupPath}\\ResponseV.json";

            if (System.IO.File.Exists(ConfigFile))
            {
                config = Serialization.JSON.Deserialize.GetFromFile<Configuration.Cfg.RootObject>(ConfigFile);
            }

            // Add all callouts to the combobox

            button1.Enabled = true;
        }

        private void cbCallouts_SelectedIndexChanged(object sender, EventArgs e)
        {
            // toggle the enabled checkbox when index changes based on the selection and the config
        }
    }
}
