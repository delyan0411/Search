using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laobai.Model;
using Laobai.Core.Data;
using Laobai.Core.Common;
using Laobai.Core.Lucene;

namespace Laobai.IndexService
{
    public class WordsTimer
    {
        private static bool isStart_Words = false;//是否开始
        private int queueCount = 0;//队列数
        private int processedCount = 0;//处理完成数
        private WordReader reader = new WordReader();

        public WordsTimer()
        {
            System.Timers.Timer t = new System.Timers.Timer(1000 *60);//间隔为30秒刷新一次(60000毫秒)
            t.Elapsed += new System.Timers.ElapsedEventHandler(Start);//到达时间的时候执行事件；
            // t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
        }
        /// <summary>
        /// 判断是否为创建索引的时间
        /// </summary>
        private bool isStartTime
        {
            get
            {
                DateTime now = DateTime.Now;//每天凌晨3点至3点半
                DateTime date = DateTime.Parse(now.ToString("yyyy-MM-dd") + " 03:00:00");
                //string dateString = date.ToString("yyyy-MM-dd");
                 return (now >= date && now <= date.AddMinutes(30));
                //return true;
            }
        }

        public void Start(object source, System.Timers.ElapsedEventArgs e)
        {
            if ((!reader.IsBegin || isStartTime) && !isStart_Words)
            {
                isStart_Words = true;//如果索引尚未建立
                this.queueCount = 10;//队列置为10
                this.processedCount = 0;//已完成数归0
                Logger.Log("开始生成词库索引...");
            }
            else if (isStart_Words && this.processedCount >= this.queueCount)
            {
                isStart_Words = false;
                this.queueCount = 0;//队列归0
                Logger.Log("词库索引生成完毕(共" + this.dataCount + "条记录,分" + this.pageCount + "页)...");
            }
            if (isStart_Words && this.queueCount > 0 && this.processedCount < this.queueCount)
            {
                this.processedCount++;//页码增加
                this.createIndex(this.processedCount);
            }
        }

        private readonly int pageSize = ServiceConfig.PageSize_Word;//每页多少条记录
        private int dataCount = 0;
        private int pageCount = 0;
        /// <summary>
        /// 创建索引(词库的索引每次都全部更新)
        /// </summary>
        /// <param name="idx"></param>
        private void createIndex(int idx)
        {
            try
            {
                int count = 0;
                int pages = 0;
                new WordWriter().CreateIndex(WordDB.GetList(this.pageSize, idx, ref count, ref pages));
                if (idx == 1)
                {
                    this.dataCount = count;
                    this.pageCount = pages;
                    this.queueCount = pages;//设置队列数
                }
            }
            catch (Exception e)
            {
                Logger.Error("WordDocument\r\n" + e.ToString());
            }
        }
    }
}
