﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AnonymousPipesClass
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //args = new String[] { "item1", "Item2" };
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain(args));
        }
    }
}
