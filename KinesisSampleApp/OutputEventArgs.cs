using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinesisSampleApp
{
	public class OutputEventArgs : EventArgs
	{
		public OutputEventArgs()
		{
			Output = new StringBuilder();
		}

		public StringBuilder Output
		{
			get;
			set;
		}
	}
}
