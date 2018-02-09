using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laobai.Lucene.Search.Function;
using Laobai.Lucene.Search;
using Laobai.Lucene.Index;

namespace Laobai.Core.Lucene
{
    public class DefaultCustomScoreQuery : CustomScoreQuery
    {
        public DefaultCustomScoreQuery(Query subQuery)
            : base(subQuery)
        {

        }

        public DefaultCustomScoreQuery(Query subQuery, ValueSourceQuery valSrcQuery)
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
            return new DefaultCustomScoreProvider(reader);
        }
    }
}
