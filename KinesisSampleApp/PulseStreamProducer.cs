using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Runtime.Serialization.Json;

using Amazon;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Amazon.Runtime;

namespace KinesisSampleApp
{
	/// <summary>
	/// Adds Kinesis records to the thread-safe collection.
	/// </summary>
	public class PulseStreamProducer
	{
		#region >>> fields <<<

		/// <summary>
		/// A thread safe collection which stores generated data.
		/// </summary>
		private readonly BlockingCollection<PutRecordsRequestEntry> _records;

		/// <summary>
		/// PartitionKey format.
		/// </summary>
		private const string PartitionKey = "00000-00000-#####";

		/// <summary>
		/// An error message that is displayed if the list of records is disposed.
		/// </summary>
		private const string RecordsListDisposed = "送信待ちレコードのコレクションが破棄または変更されたため、送信を中止しました。";

		/// <summary>
		/// Describes the number of units that sends pulse data.
		/// </summary>
		private const int NumberOfUnit = 500;

		private const int StartSequenceNumber = 1;

		#endregion

		#region >>> constructors　<<<

		/// <summary>
		/// Initializes a new instance of the PulseStreamProducer class with the specified reference set to collection which stored generated data.
		/// </summary>
		/// <param name="records">Reference set to collection which stored generated data.</param>
		public PulseStreamProducer( BlockingCollection<PutRecordsRequestEntry> records )
		{
			_records = records;
			InitializeSequence();
		}

		#endregion

		#region >>> public methods <<<

		/// <summary>
		/// Adds records to the collection that stores requests for put record.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token.</param>
		public void Produce( CancellationToken cancellationToken )
		{
			// 送信待ちレコードがnullなら処理を抜ける。
			if( _records == null )
			{
				Debug.WriteLine( RecordsListDisposed );
				return;
			}

			var pid = AppConfig.ProducerId;
			var random = new Random();
			
			while( true )
			{
				var uid = random.Next( NumberOfUnit );
				int seq;
				if( !SequenceManager.TryGetValue( uid, out seq ) )
				{
					continue;
				}

				// Serializes a KinesisRecord object to a memory stream.
				var mem = new MemoryStream();
				var serializer = new DataContractJsonSerializer( typeof( KinesisRecord ) );				

				// Writes JSON data to the stream.
				serializer.WriteObject( mem, KinesisRecord.CreateRandomRecord( pid, uid, seq ) );

				var entry = new PutRecordsRequestEntry();
				entry.Data = mem;
				entry.PartitionKey = uid.ToString( PartitionKey );

				try
				{
					// Adds the JSON data stream to the collection that stores requests for put record.
					_records.Add( entry, cancellationToken );
				}
				catch( OperationCanceledException e )
				{
					Debug.WriteLine( e.Message );
				}
				if( cancellationToken.IsCancellationRequested )
				{
					break;
				}
			}
		}

		/// <summary>
		/// パルスストリームデータを生成して、送信待ちレコードのコレクションにデータを追加します。
		/// </summary>
		public void Produce()
		{
			var ct = new CancellationToken();
			// キャンセルされることのないダミーのトークンを引数に指定
			Produce( ct );
		}
		
		#endregion

		#region >>> private methods <<<

		private void InitializeSequence()
		{
			SequenceManager = SequenceManager ?? new ConcurrentDictionary<int, int>();

			for( int i = 1; i <= NumberOfUnit; i++ )
			{
				// Initializes sequence number.
				SequenceManager.AddOrUpdate( i, StartSequenceNumber, ( x, y ) => StartSequenceNumber );
			}
		}

		#endregion

		#region >>> propaties <<<

		/// <summary>
		/// Manages the sequence numbers of the Kinesis records.
		/// </summary>
		private ConcurrentDictionary<int, int> SequenceManager
		{
			get;
			set;
		}
		
		#endregion
	}
}
