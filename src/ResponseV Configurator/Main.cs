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
        private int prevIndex = -1;
        private string ConfigFile = $"{Application.StartupPath}\\ResponseV.json";

        private void Main_Load(object sender, EventArgs e)
        {

            if (System.IO.File.Exists(ConfigFile))
            {
                config = Serialization.JSON.Deserialize.GetFromFile<Configuration.Cfg.RootObject>(ConfigFile);
            }
            else
            {
                config = new Configuration.Cfg.RootObject();

                Serialization.JSON.Serialize.SerializeToFile(config, ConfigFile);
            }

            List<string> features = config.Callouts.Features.Keys.ToList();

            features.Sort();

            cbCallouts.DataSource = features;
        }

        private void cbCallouts_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = prevIndex;
            prevIndex = cbCallouts.SelectedIndex;

            if (cbCallouts.SelectedItem != null && config.Callouts.Features.ContainsKey(cbCallouts.SelectedItem.ToString()))
            {
                checkBox1.Checked = config.Callouts.Features[cbCallouts.SelectedItem.ToString()];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Serialization.JSON.Serialize.SerializeToFile(config, ConfigFile);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCallouts.SelectedItem != null && config.Callouts.Features.ContainsKey(cbCallouts.SelectedItem.ToString()))
            {
                bool current = config.Callouts.Features[cbCallouts.SelectedItem.ToString()];
                
                if (current != checkBox1.Checked)
                {
                    config.Callouts.Features[cbCallouts.SelectedItem.ToString()] = checkBox1.Checked;
                    button1.Enabled = true;
                }
            }
        }
    }
}
