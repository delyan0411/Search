using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laobai.Model;
using Laobai.Core.Data;
using Laobai.Core.Common;
using Laobai.Core.Lucene;
using Laobai.Core.Common.Json;

namespace Laobai.IndexService
{
    public class ProductWmTimer
    {
        private static bool isStart = false;//是否开始
        private int queueCount = 0;//队列数
        private int processedCount = 0;//处理完成数
        private WmProductReader reader = new WmProductReader();

        public ProductWmTimer()
        {
            System.Timers.Timer t = new System.Timers.Timer(1000 * 60);//间隔为20秒刷新一次(60000毫秒)
            t.Elapsed += new System.Timers.ElapsedEventHandler(Start);//到达时间的时候执行事件；
            //t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)
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
                return (reader.LastModifyTime.AddMinutes(ServiceConfig.WmIntervalMinute) < DateTime.Now);
            }
        }
        private bool isCreate = false;//是否创建
        private DateTime lastModifyTime = DateTime.Now;
        private DateTime endTime = DateTime.Now;
        public void Start(object source, System.Timers.ElapsedEventArgs e)
        {
            if ((!reader.IsBegin || isStartTime) && !isStart)
            {
                isStart = true;//如果索引尚未建立
                this.queueCount = 10;//队列置为10
                this.processedCount = 0;//已完成数归0
                this.lastModifyTime = reader.LastModifyTime.AddMinutes(-1);//.AddMinutes(Config.IntervalMinute);//上次更新时间
                this.endTime = DateTime.Now;//当前启动时间
                if (!reader.IsBegin) this.isCreate = true;//创建
                else if (this.isCreate) this.isCreate = false;//更新
                Logger.Log("开始生成微脉商品索引...");
            }
            else if (isStart && this.processedCount >= this.queueCount)
            {
                isStart = false;
                this.queueCount = 0;//队列归0
                this.isCreate = false;
                Logger.Log("微脉商品索引生成完毕(共" + this.dataCount + "条记录,分" + this.pageCount + "页)...");
            }
            if (isStart && this.queueCount > 0 && this.processedCount < this.queueCount)
            {
                this.processedCount++;//页码增加
                Logger.Log("生成微脉商品第" + this.processedCount + "页...");
                this.createIndex(this.processedCount);
            }
        }

        private readonly int pageSize = ServiceConfig.PageSize_Product;//每页多少条记录
        private int dataCount = 0;
        private int pageCount = 0;
        /// <summary>
        /// 创建索引(索引每次都全部更新)
        /// </summary>
        /// <param name="idx"></param>
        private void createIndex(int idx)
        {
            try
            {
                int count = 0;
                int pages = 0;
                List<ProductWmInfo> list = new List<ProductWmInfo>();
                if (this.isCreate)
                {
                    Logger.Log("isCreate");
                    list = ProductWmDB.GetList(this.pageSize, idx, ref count, ref pages);//创建
                }
                else
                {
                    Logger.Log("Modify");
                    DateTime sDate = this.lastModifyTime;
                    DateTime eDate = this.endTime;
                    list = ProductWmDB.GetModifyList(this.pageSize, idx
                        , sDate, eDate
                        , ref count, ref pages);//更新
                }
                new WmProductWriter().CreateIndex(list);
                if (idx == 1)
                {
                    this.dataCount = count;
                    this.pageCount = pages;
                    this.queueCount = pages;//设置队列数
                }
            }
            catch (Exception e)
            {
                Logger.Error("WmProductDocument\r\n" + e.ToString());
            }
        }
    }
}