using System;
using System.Collections.Generic;

namespace Morpheus
{
    public partial class ProductSpectra
    {
        private static List<MSPeak> FilterPeaks(List<MSPeak> peaks, double absoluteThreshold, double relativeThresholdPercent, int maximumNumberOfPeaks)
        {
            List<MSPeak> filtered_peaks = new List<MSPeak>(peaks);

            double relative_threshold = -1.0;
            if(relativeThresholdPercent > 0.0)
            {
                double max_intensity = -1.0;
                foreach(MSPeak peak in filtered_peaks)
                {
                    double intensity = peak.Intensity;
                    if(intensity > max_intensity)
                    {
                        max_intensity = intensity;
                    }
                }
                relative_threshold = max_intensity * relativeThresholdPercent / 100.0;
            }

            double threshold = Math.Max(absoluteThreshold, relative_threshold);

            int p = 0;
            while(p < filtered_peaks.Count)
            {
                MSPeak peak = filtered_peaks[p];
                if(peak.Intensity < threshold)
                {
                    filtered_peaks.RemoveAt(p);
                }
                else
                {
                    p++;
                }
            }

            if(maximumNumberOfPeaks > 0 && filtered_peaks.Count > maximumNumberOfPeaks)
            {
                filtered_peaks.Sort(MSPeak.DescendingIntensityComparison);
                filtered_peaks.RemoveRange(maximumNumberOfPeaks, filtered_peaks.Count - maximumNumberOfPeaks);
            }

            return filtered_peaks;
        }
    }
}