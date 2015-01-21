using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Runtime.Serialization.Json;

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
		private readonly BlockingCollection<MemoryStream> _records;

		/// <summary>
		/// Describes the max size of one record.
		/// </summary>
		private const int MaxDataSize = 5000000;

		/// <summary>
		/// Describes the default number of ports.
		/// </summary>
		private const int NumberOfPorts = 12;

#if DEBUG
		private const string RecordsListDisposed = "送信待ちレコードのコレクションが破棄または変更されたため、送信を中止しました。";
		
		/// <summary>
		/// Describes the ID of the producer.
		/// </summary>
		private const int ProducerId = 1;

		private const int UnitId = 1;

		private const int SeqNum = 1;
#endif

		#endregion

		#region >>> constructors　<<<

		/// <summary>
		/// Initializes a new instance of the PulseStreamProducer class with the specified reference set to collection which stored generated data.
		/// </summary>
		/// <param name="records">Reference set to collection which stored generated data.</param>
		public PulseStreamProducer( BlockingCollection<MemoryStream> records )
		{
			_records = records;
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

#if DEBUG
			var pid = ProducerId;
			var uid = UnitId;
			var seq = SeqNum;
#endif	
			
			while( true )
			{
				// Serializes a KinesisRecord object to a memory stream.
				var mem = new MemoryStream();
				var serializer = new DataContractJsonSerializer( typeof( KinesisRecord ) );

				// Writes JSON data to the stream.
				serializer.WriteObject( mem, KinesisRecord.CreateRandomRecord( pid, uid, seq ) );

				try
				{
					// Adds the JSON data stream to the collection that stores requests for put record.
					_records.Add( mem, cancellationToken );
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

		/// <summary>
		/// 乱数によりレコードを生成します。
		/// </summary>
		/// <returns>乱数により生成したレコード</returns>
		private MemoryStream CreateRandomPulseRecord()
		{
			var buffer = new List<Byte>();

			// 現在時刻を表すバイト配列を追加
			buffer.AddRange( BitConverter.GetBytes( DateTime.Now.ToBinary() ) );

			var r = new Random();

			// パルスの入力ポート数分繰り返す。
			for( int i = 0; i < NumberOfPorts; i++ )
			{
				buffer.AddRange( BitConverter.GetBytes( r.Next() ) ); 
			}

			return new MemoryStream( buffer.ToArray() );
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
