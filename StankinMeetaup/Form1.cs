using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StankinMeetaup
{
    public partial class Form1 : Form
    {

        Client client = new Client(); 

        public Form1()
        {
            InitializeComponent();
            InitEvents();
        }

        public void InitEvents()
        {
            listView1.Clear();
            listView1.Columns.Add("N", 20, HorizontalAlignment.Right);
            listView1.Columns.Add("Date", 70, HorizontalAlignment.Left);
            listView1.Columns.Add("Time", 70, HorizontalAlignment.Left);
            listView1.Columns.Add("Source", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Event", 200, HorizontalAlignment.Left);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //start
        private void button1_Click(object sender, EventArgs e)
        {
            Strategy.clients.Add(client);
            Strategy.start();
        }

        //stop
        private void button2_Click(object sender, EventArgs e)
        {
            Strategy.stop();
            Strategy.clients.Remove(client);
        }

        //buy
        private void button4_Click(object sender, EventArgs e)
        {
            client.makeOrder("B", 1);
        }

        //sell
        private void button3_Click(object sender, EventArgs e)
        {
            client.makeOrder("S", 1);
        }    
    }
}
