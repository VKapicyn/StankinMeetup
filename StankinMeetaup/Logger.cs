using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StankinMeetaup
{
    public static class Logger
    {
        public static Form1 Forma = (Application.OpenForms[0] as Form1);

        public static void AddEvent(string source, string text)
        {
            string str = "";
            DateTime time = DateTime.Now;

            ListViewItem item = new ListViewItem((Forma.listView1.Items.Count + 1).ToString());
            for (int k = 1; k < Forma.listView1.Columns.Count; k++)
            {
                item.SubItems.Add("");
                string s = "";
                ColumnHeader col = Forma.listView1.Columns[k];
                if (col.Text == "Date")
                    s = time.ToShortDateString();
                else if (col.Text == "Time")
                    s = String.Format("{0}.{1:000}", time.ToLongTimeString(), time.Millisecond);
                else if (col.Text == "Source")
                    s = source;
                else if (col.Text == "Event")
                    s = text;
                item.SubItems[k].Text = s;
                str = str + s;
            }

            Forma.listView1.Invoke(new Action(() => Forma.listView1.Items.Add(item)));
        }
        
    }
}
