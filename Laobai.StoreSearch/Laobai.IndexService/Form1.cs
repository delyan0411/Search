using Laobai.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Laobai.IndexService
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //new WordsTimer();
            //new ProductTimer();
            //new ProductWmTimer();
            //int count = 0;
            //int pages = 0;
            //ProductDB.GetList(800, 7, ref count, ref pages);
            Thread t2 = new Thread(new ThreadStart(this.RunProductThread));
            t2.IsBackground = true;
            t2.Start();
        }
        #region RunProductThread 商品索引
        private void RunProductThread()
        {
            new Laobai.IndexService.ProductTimer();
        }
        #endregion
    }
}
