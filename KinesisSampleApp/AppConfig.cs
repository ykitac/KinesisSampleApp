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

		private const string AccessKeyIdConfigName = "AccesskeyId";
		private const string SecretAccessKeyConfigName = "SecretAccessKey";

		static AppConfig()
		{
			Initialize();
		}

		private static void Initialize()
		{
			_accesskeyId = ConfigurationManager.AppSettings[AccessKeyIdConfigName];
			_secretAccesskey = ConfigurationManager.AppSettings[SecretAccessKeyConfigName];
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
	}
}
