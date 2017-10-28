using System;
namespace Founders
{
    public class ProgressEventArgs :EventArgs
    {
        public ProgressEventArgs()
        {
        }
		public string Status { get; private set; }
		public int percentage { get; private set; }
        public string SecondStatus { get; private set; }
		public ProgressEventArgs(string status, int percentage = 0)
		{
			Status = status;
			this.percentage = percentage;
		}
		public ProgressEventArgs(string status,String secondStatus, int percentage = 0)
		{
			Status = status;
			this.percentage = percentage;
            this.SecondStatus = secondStatus;
		}
	}
}
