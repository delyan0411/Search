using System;

namespace Laobai.Model
{
    public enum EAnalyzerMode
    {
        /// <summary>
        /// 根据词库分词
        /// </summary>
        Analyzer = 0,
        /// <summary>
        /// 每个字当作一个词
        /// </summary>
        Spliter = 1
    }
}
