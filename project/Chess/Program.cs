using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Chess
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
           DialogResult msgBox = MessageBox.Show("Chess960 Options", "Play with chess960 rules?", MessageBoxButtons.YesNo);
            if(msgBox == DialogResult.Yes)
            {

            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
