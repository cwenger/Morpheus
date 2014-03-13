using System;

namespace Morpheus
{
    public partial class TandemMassSpectra
    {
        public TandemMassSpectra() : base() { }

        public event EventHandler ReportTaskWithoutProgress;

        protected virtual void OnReportTaskWithoutProgress(EventArgs e)
        {
            EventHandler handler = ReportTaskWithoutProgress;

            if(handler != null)
            {
                handler(null, e);
            }
        }

        public event EventHandler ReportTaskWithProgress;

        protected virtual void OnReportTaskWithProgress(EventArgs e)
        {
            EventHandler handler = ReportTaskWithProgress;

            if(handler != null)
            {
                handler(null, e);
            }
        }

        public event EventHandler<ProgressEventArgs> UpdateProgress;

        protected virtual void OnUpdateProgress(ProgressEventArgs e)
        {
            EventHandler<ProgressEventArgs> handler = UpdateProgress;

            if(handler != null)
            {
                handler(null, e);
            }
        }
    }
}
