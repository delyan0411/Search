using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laobai.Lucene.Search.Function;
using Laobai.Lucene.Search;
using Laobai.Lucene.Index;

namespace Laobai.Core.Lucene
{
    public class DefaultCustomScoreQueryASC : CustomScoreQuery
    {
        public DefaultCustomScoreQueryASC(Query subQuery)
            : base(subQuery)
        {

        }

        public DefaultCustomScoreQueryASC(Query subQuery, ValueSourceQuery valSrcQuery)
            : base(subQuery, valSrcQuery)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override CustomScoreProvider GetCustomScoreProvider(IndexReader reader)
        {
            return new DefaultCustomScoreProviderASC(reader);
        }
    }
}