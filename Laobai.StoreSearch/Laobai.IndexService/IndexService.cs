using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace Laobai.IndexService
{
    public partial class IndexService : ServiceBase
    {
        public IndexService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Thread t1 = new Thread(new ThreadStart(this.RunWordThread));
            t1.IsBackground = true;
            t1.Start();
            //this.WordDocument();//词库
            Thread t2 = new Thread(new ThreadStart(this.RunProductThread));
            t2.IsBackground = true;
            t2.Start();

            Thread t3 = new Thread(new ThreadStart(this.RunWmProductThread));
            t3.IsBackground = true;
            t3.Start();
        }

        protected override void OnStop()
        {
        }

        #region RunWordThread 词库索引
        private void RunWordThread()
        {
            new Laobai.IndexService.WordsTimer();
        }
        #endregion

        #region RunProductThread 商品索引
        private void RunProductThread()
        {
            new Laobai.IndexService.ProductTimer();
        }
        #endregion

        #region RunProductThread 微脉商品索引
        private void RunWmProductThread()
        {
            new Laobai.IndexService.ProductWmTimer();
        }
        #endregion
    }
}
