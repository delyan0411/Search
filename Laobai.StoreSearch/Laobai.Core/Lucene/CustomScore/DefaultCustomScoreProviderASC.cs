using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laobai.Lucene.Search.Function;
using Laobai.Lucene.Index;
using Laobai.Lucene.Search;

namespace Laobai.Core.Lucene
{
    public class DefaultCustomScoreProviderASC : CustomScoreProvider
    {
        public DefaultCustomScoreProviderASC(IndexReader reader)
            : base(reader)
        {

        }
        /// <summary>
        /// 覆盖CustomScore
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="subQueryScore"></param>
        /// <param name="valSrcScore"></param>
        /// <returns></returns>
        public override float CustomScore(int doc, float subQueryScore, float valSrcScore)
        {
            return (subQueryScore + valSrcScore) * -1;
        }
    }
}