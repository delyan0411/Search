using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace Laobai.IndexService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            //Application.Run(new Form1());
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new IndexService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
