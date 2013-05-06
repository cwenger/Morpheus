using System;

namespace Morpheus
{
    public partial class ProductSpectra
    {
        public static event EventHandler ReportTaskWithoutProgress;

        protected static void OnReportTaskWithoutProgress(EventArgs e)
        {
            EventHandler handler = ReportTaskWithoutProgress;

            if(handler != null)
            {
                handler(null, e);
            }
        }

        public static event EventHandler ReportTaskWithProgress;

        protected static void OnReportTaskWithProgress(EventArgs e)
        {
            EventHandler handler = ReportTaskWithProgress;

            if(handler != null)
            {
                handler(null, e);
            }
        }

        public static event EventHandler<ProgressEventArgs> UpdateProgress;

        protected static void OnUpdateProgress(ProgressEventArgs e)
        {
            EventHandler<ProgressEventArgs> handler = UpdateProgress;

            if (handler != null)
            {
                handler(null, e);
            }
        }
    }
}
