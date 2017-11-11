using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Route66
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate(My.ExePath+"initialmap.html");
            //webBrowser1.Url = new Uri("http://www.openstreetmap.org");
            //NGEngine engine = NGEngine.createNew();
            //var status = engine.init(dataDir: "C:\\ProgramData\\Aebi-Schmidt\\AutologicRouteCreator\\map\\data", mapDir: "C:\\ProgramData\\Aebi-Schmidt\\AutologicRouteCreator\\map");
            //var result = engine.authorize(cid: "-----------CID-for-NaviGenie-team----------", aid: "PC_NaviGenie");
            //var versie = engine.versionInfo();
        }
    }
}
