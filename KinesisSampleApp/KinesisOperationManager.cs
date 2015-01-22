using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

using Amazon;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Amazon.Runtime;

namespace KinesisSampleApp
{
	public sealed class KinesisOperationManager
	{
		#region >>> fields <<<

		/// <summary>
		/// A thread-safe collection that stores the records to be sent to the Kinesis.
		/// </summary>
		private readonly BlockingCollection<PutRecordsRequestEntry> _records;

		/// <summary>
		/// An instance of the class that sends the records to the Kinesis.
		/// </summary>
		private KinesisModel _consumer;
		
		/// <summary>
		/// An instance of the class that generates the records to be sent to the Kinesis.
		/// </summary>
		private PulseStreamProducer _producer;

		/// <summary>
		/// A thread that manages the process of the consumer.
		/// </summary>
		private Thread _consumerThread;

		/// <summary>
		/// A thread that manages the process of the producer.
		/// </summary>
		private Thread _producerThread;

		/// <summary>
		/// Describes the capacity of the "_records" field.
		/// </summary>
		private const int CollectionCapacity = 100;

		#endregion

		#region >>> constructors <<<

		/// <summary>
		/// Initializes a new instance of KinesisOperationManager class.
		/// </summary>
		public KinesisOperationManager()
		{
			// Initializes the "_records" field.
			_records = new BlockingCollection<PutRecordsRequestEntry>( CollectionCapacity );

			_producer = null;
			_consumer = null;
			_producerThread = null;
			_consumerThread = null;
		}
		
		#endregion

		#region >>> public methods <<<

		/// <summary>
		/// Starts the producer and the consumer.
		/// </summary>
		public void Start()
		{
			// Sets a new instance to the field if it is NULL.
			_producer = _producer ?? new PulseStreamProducer( _records );
			_consumer = _consumer ?? new KinesisModel( _records );

			// Implements if either _producerThread is null or Stopped. 
			if( _producerThread == null || _producerThread.ThreadState != ThreadState.Stopped )
			{
				// The producer thread is running.
				_producerThread = new Thread( () => _producer.Produce() );
				_producerThread.Start();
			}

			if( _consumerThread == null || _consumerThread.ThreadState != ThreadState.Stopped )
			{
				// The consumer thread is running.
				_consumerThread = new Thread( () => _consumer.Consume() );
				_consumerThread.Start();
			}
		}

		public void Stop()
		{
			_producerThread.Abort();
			_consumerThread.Abort();
		}

		#endregion

	}
}
