using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace KinesisSampleApp
{
	/// <summary>
	/// Describes a Record to be put to the Kinesis.
	/// </summary>
	[DataContract]
	public class KinesisRecord
	{
		#region >>> fields <<<

		/// <summary>
		/// A constant value that represents default size of a pulse data.
		/// </summary>
		private const int DefaultPulseDataSize = 48;

		#endregion

		#region >>> constructors <<<

		public KinesisRecord()
		{
			Time = DateTime.Now;
		}

		public KinesisRecord( byte[] data ) : this()
		{
			PulseData = data;
		}

		public KinesisRecord( byte[] data, int producerId ) : this( data )
		{
			ProducerId = producerId;
		}

		public KinesisRecord( byte[] data, int producerId, int unitId ) : this( data, producerId )
		{
			UnitId = unitId;
		}

		public KinesisRecord( byte[] data, int producerId, int unitId, int sequenceNumber ) : this( data, producerId, unitId )
		{
			UnitId = unitId;
		}

		public KinesisRecord( byte[] data, int producerId, int unitId, int sequenceNumber, DateTime time ) : this( data, producerId, unitId, sequenceNumber )
		{
			SequenceNumber = sequenceNumber;
			Time = time;
		}

		#endregion

		#region >>> public methods <<<

		/// <summary>
		/// Returns an array of bytes that represents the current object.
		/// </summary>
		/// <returns>An array of bytes that represents the current object. </returns>
		public byte[] ToBytes()
		{
			var buffer = new List<byte>();

			buffer.AddRange( BitConverter.GetBytes( ProducerId ) );
			buffer.AddRange( BitConverter.GetBytes( UnitId ) );
			buffer.AddRange( BitConverter.GetBytes( SequenceNumber ) );
			buffer.AddRange( BitConverter.GetBytes( Time.ToBinary() ) );
			buffer.AddRange( PulseData );

			return buffer.ToArray();
		}

		/// <summary>
		/// Returns an instance of KinesisRecord that has random data.
		/// </summary>
		/// <param name="producerId"></param>
		/// <param name="unitId"></param>
		/// <param name="sequenceNumber"></param>
		/// <returns></returns>
		public static KinesisRecord CreateRandomRecord( int producerId, int unitId, int sequenceNumber )
		{
			var buffer = new byte[DefaultPulseDataSize];
			var r = new Random();
			r.NextBytes( buffer );

			return new KinesisRecord( buffer, producerId, unitId, sequenceNumber );
		}
		
		#endregion

		#region >>> properties <<<

		/// <summary>
		/// Gets and Sets the Kinesis Producer's ID.
		/// </summary>
		[DataMember(Name="ProducerId")]
		public int ProducerId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets and Sets the Unit's ID.
		/// </summary>
		[DataMember(Name="UnitId")]
		public int UnitId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets and Sets the SequenceNumber of the Kinesis Record.
		/// </summary>
		[DataMember(Name="SequenceNumber")]
		public int SequenceNumber
		{
			get;
			set;
		}

		/// <summary>
		/// Gets and Sets the time when the Kinesis record is generated.
		/// </summary>
		[DataMember(Name="Time")]
		public DateTime Time
		{
			get;
			set;
		}

		/// <summary>
		/// Gets and Sets the PulseData.
		/// </summary>
		[DataMember(Name="PulseData")]
		public byte[] PulseData
		{
			get;
			set;
		}

		#endregion
	}
}
