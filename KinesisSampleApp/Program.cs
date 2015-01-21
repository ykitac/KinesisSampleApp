using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Kinesis;

namespace KinesisSampleApp
{
	class Program
	{
		private static KinesisSampleAppViewModel _vm;

		public static void Main( string[] args )
		{
			Console.Write( GetServiceOutput() );
			Initialize( args );

			while( true )
			{
				ConsoleMessageLoop();
			}
		}

		public static string GetServiceOutput()
		{
			StringBuilder sb = new StringBuilder( 1024 );
			using( StringWriter sr = new StringWriter( sb ) )
			{
				sr.WriteLine( "===========================================" );
				sr.WriteLine( "Welcome to the AWS .NET SDK!" );
				sr.WriteLine( "===========================================" );
			}
			return sb.ToString();
		}

		/// <summary>
		/// Initializes program informations.
		/// </summary>
		/// <param name="args"></param>
		private static void Initialize( string[] args )
		{
			_vm = new KinesisSampleAppViewModel();
			_vm.Output += new EventHandler<OutputEventArgs>( OnOutput );
		}

		/// <summary>
		/// Hundles the "Output" event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		static void OnOutput( object sender, OutputEventArgs e )
		{
			Console.Write( e.Output.ToString() );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private static void ConsoleMessageLoop()
		{
			StringBuilder sb = new StringBuilder( 1024 );
			using( StringWriter sr = new StringWriter( sb ) )
			{
				sr.WriteLine( "Select command." );
				sr.WriteLine( "1.start" );
				sr.WriteLine( "2.stop" );
				sr.WriteLine( "3.exit" );

				Console.Write( sr.GetStringBuilder().ToString() );
			}

			var input = Console.ReadLine();
			int menuCode;

			try
			{
				menuCode = Convert.ToInt32( input );
			}
			catch( FormatException )
			{
				Console.WriteLine( "error: not a number." );
				return;
			}
			catch( OverflowException )
			{
				Console.WriteLine( "error: too large number." );
				return;
			}

			switch( menuCode )
			{
				case 1:
					_vm.StartThreads();
					break;
				case 2:
					_vm.StopThreads();
					break;
				case 3:
					Environment.Exit( 0 );
					break;
				default:
					Console.WriteLine( "error: can't interpret your input." );
					break;
			}
		}
	}
}