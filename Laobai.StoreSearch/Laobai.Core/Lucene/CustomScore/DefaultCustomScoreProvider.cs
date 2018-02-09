using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laobai.Lucene.Search.Function;
using Laobai.Lucene.Index;
using Laobai.Lucene.Search;

namespace Laobai.Core.Lucene
{
    public class DefaultCustomScoreProvider : CustomScoreProvider
    {
        //float[] scores = null;
        public DefaultCustomScoreProvider(IndexReader reader)
            : base(reader)
        {
            /*try
            {
                scores = FieldCache_Fields.DEFAULT.GetFloats(reader, "sort_index");//.DEFAULT.getStrings(reader, "filename");
            }
            catch (Exception)
            {

            }*/
        }
        /*
        /// <summary>
        /// 覆盖CustomScore
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="subQueryScore"></param>
        /// <param name="valSrcScores"></param>
        /// <returns></returns>
        public override float CustomScore(int doc, float subQueryScore, float[] valSrcScores)
        {
            if (valSrcScores.Length == 1)
            {
                return CustomScore(doc, subQueryScore, valSrcScores[0]);
            }
            if (valSrcScores.Length == 0)
            {
                return CustomScore(doc, subQueryScore, 1);
            }
            float score = subQueryScore;
            for (int i = 0; i < valSrcScores.Length; i++)
            {
                score *= valSrcScores[i];
            }
            return score + scores[doc];
        }
        */
        /// <summary>
        /// 覆盖CustomScore
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="subQueryScore"></param>
        /// <param name="valSrcScore"></param>
        /// <returns></returns>
        public override float CustomScore(int doc, float subQueryScore, float valSrcScore)
        {
            //float score = scores[doc];
            //if (valSrcScore < 0)
            //    return valSrcScore;
            return subQueryScore + valSrcScore;// +score;
        }
    }
}
