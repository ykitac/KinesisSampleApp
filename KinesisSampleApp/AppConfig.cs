using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace KinesisSampleApp
{
	/// <summary>
	/// Application configurations.
	/// </summary>
	public static class AppConfig
	{
		private static string _accesskeyId;
		private static string _secretAccesskey;
		private static int _producerId;

		private const string AccessKeyIdConfigName = "AccesskeyId";
		private const string SecretAccessKeyConfigName = "SecretAccessKey";
		private const string ProduceIdConfigName = "ProducerId";



		static AppConfig()
		{
			Initialize();
		}

		private static void Initialize()
		{
			_accesskeyId = ConfigurationManager.AppSettings[AccessKeyIdConfigName];
			_secretAccesskey = ConfigurationManager.AppSettings[SecretAccessKeyConfigName];

			try
			{
				_producerId = Convert.ToInt32( ConfigurationManager.AppSettings[ProduceIdConfigName] );
			}
			catch( Exception e )
			{
				if( e is FormatException || e is OverflowException )
				{
					// Sets default to _producerId.
					_producerId = 1;
				}
				else
				{
					throw e;
				}
			}
		}

		/// <summary>
		/// Gets an AWS access key ID.
		/// </summary>
		public static string AccessKeyId
		{
			get
			{
				return _accesskeyId;
			}
		}

		/// <summary>
		/// Gets an AWS secret access key.
		/// </summary>
		public static string SecretAccesskey
		{
			get
			{
				return _secretAccesskey;
			}
		}

		/// <summary>
		/// Gets the Producer Id.
		/// </summary>
		public static int  ProducerId
		{
			get
			{
				return _producerId;
			}
		}
	}
}
