using System.Collections.Generic;

namespace Morpheus
{
    public static class FalseDiscoveryRate
    {
        public static List<IdentificationWithFalseDiscoveryRate<T>> DoFalseDiscoveryRateAnalysis<T>(IEnumerable<T> items, double decoysOverTargetsRatios) where T : ITargetDecoy
        {
            List<IdentificationWithFalseDiscoveryRate<T>> ids = new List<IdentificationWithFalseDiscoveryRate<T>>();

            int cumulative_target = 0;
            int cumulative_decoy = 0;
            foreach(T item in items)
            {
                if(item.Decoy)
                {
                    cumulative_decoy++;
                }
                else
                {
                    cumulative_target++;
                }
                double temp_q_value = (double)(cumulative_decoy / decoysOverTargetsRatios) / cumulative_target;
                ids.Add(new IdentificationWithFalseDiscoveryRate<T>(item, cumulative_target, cumulative_decoy, temp_q_value));
            }

            double min_q_value = double.PositiveInfinity;
            for(int i = ids.Count - 1; i >= 0; i--)
            {
                IdentificationWithFalseDiscoveryRate<T> id = ids[i];
                if(id.QValue > min_q_value)
                {
                    id.QValue = min_q_value;
                }
                else if(id.QValue < min_q_value)
                {
                    min_q_value = id.QValue;
                }
            }

            return ids;
        }

        public static void DetermineMaximumIdentifications<T>(IEnumerable<IdentificationWithFalseDiscoveryRate<T>> items, bool lowerScoresAreBetter, double maximumFalseDiscoveryRate,
            out double scoreThreshold, out int targetIdentifications, out int decoyIdentifications, out double falseDiscoveryRate) where T : ITargetDecoy
        {
            scoreThreshold = lowerScoresAreBetter ? double.NegativeInfinity : double.PositiveInfinity;
            targetIdentifications = 0;
            decoyIdentifications = 0;
            falseDiscoveryRate = double.NaN;
            foreach(IdentificationWithFalseDiscoveryRate<T> id in items)
            {
                if(id.QValue <= maximumFalseDiscoveryRate && id.CumulativeTarget > targetIdentifications)
                {
                    scoreThreshold = id.Identification.Score;
                    targetIdentifications = id.CumulativeTarget;
                    decoyIdentifications = id.CumulativeDecoy;
                    falseDiscoveryRate = id.QValue;
                }
            }
        }
    }
}
