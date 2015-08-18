using System;
using System.Data.SqlClient;

namespace BuildVersioning.Data
{
	public static class SqlDataReaderExtensions
	{
		public static int ReadInt32(this SqlDataReader sqlDataReader, string columnName, int defaultValue = -1)
		{
			if (string.IsNullOrWhiteSpace(columnName))
				throw new ArgumentNullException("columnName");

			var obj = sqlDataReader[columnName];
			return (obj == DBNull.Value) ? defaultValue : (int) obj;
		}

		public static long ReadInt64(this SqlDataReader sqlDataReader, string columnName, long defaultValue = -1)
		{
			if (string.IsNullOrWhiteSpace(columnName))
				throw new ArgumentNullException("columnName");

			var obj = sqlDataReader[columnName];
			return (obj == DBNull.Value) ? defaultValue : (long)obj;
		}

		public static string ReadString(this SqlDataReader sqlDataReader, string columnName)
		{
			if (string.IsNullOrWhiteSpace(columnName))
				throw new ArgumentNullException("columnName");

			var obj = sqlDataReader[columnName];
			return (obj == DBNull.Value) ? null : obj as string;
		}
	}
}