using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Concurrent;

using Amazon.Kinesis;

namespace KinesisSampleApp
{
	internal sealed class KinesisSampleAppViewModel
	{
		#region >>> fields <<<

		/// <summary>
		/// A instance that manages the Kinesis operations.
		/// </summary>
		private KinesisOperationManager _kom;

		#endregion

		#region >>> constructors <<<

		/// <summary>
		/// Initializes a new instance of KinesisSampleAppViewModel class.
		/// </summary>
		internal KinesisSampleAppViewModel()
		{
			_kom = new KinesisOperationManager();
		}

		#endregion

		#region >>> public methods <<<
		
		/// <summary>
		/// The Command that starts the producer and the consumer threads.
		/// </summary>
		public void StartThreads()
		{
			_kom.Start();
			using( var w = new StringWriter() )
			{
				w.WriteLine( "Amazon Kinesis にレコードを送信中です。" );
				RaiseOutputEvent( w.GetStringBuilder() );
			}
		}

		/// <summary>
		/// The Command that stops the producer and the consumer threads.
		/// </summary>
		public void StopThreads()
		{
			_kom.Stop();
			using( var w = new StringWriter() )
			{
				w.WriteLine( "Amazon Kinesis へのレコード送信を停止します。" );
				RaiseOutputEvent( w.GetStringBuilder() );
			}
		}

		#endregion

		#region >>> private methods <<<

		private void RaiseOutputEvent( StringBuilder output )
		{
			var e = new OutputEventArgs();
			e.Output = output;
			Output( this, e );
		}

		#endregion

		#region >>> events <<<

		public event EventHandler<OutputEventArgs> Output;
		
		#endregion
	}
}
