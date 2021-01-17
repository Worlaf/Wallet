using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Data.SqlClient;
using Wallet.Common;

namespace Wallet.Data.Infrastructure
{
    public static class SqlServerDbUtil
    {
        public static bool HasSchema(string connString, string schemaName)
        {
            using var cnn = new SqlConnection(connString);
            cnn.Open();

            using var cmd = new SqlCommand($"SELECT 1 FROM sys.schemas WHERE name = '{schemaName}'", cnn);
            return cmd.ExecuteScalar() != null;
        }

        public static bool DatabaseExists(string connString)
        {
            var scb = new SqlConnectionStringBuilder(connString);
            var dbName = scb.InitialCatalog;
            scb.InitialCatalog = "master";

            using var cnn = new SqlConnection(scb.ConnectionString);
            cnn.Open();

            using var cmd = new SqlCommand($"SELECT db_id('{dbName}')", cnn);
            return cmd.ExecuteScalar() != DBNull.Value;
        }

        public static void CreateDatabase(string connString, bool enableRcsi = false, string collation = null)
        {
            var scb = new SqlConnectionStringBuilder(connString);
            var dbName = scb.InitialCatalog;
            scb.InitialCatalog = "master";

            using (var cnn = new SqlConnection(scb.ConnectionString))
            {
                cnn.Open();

                var cmdText = collation.IsEmpty() ? $"CREATE DATABASE [{dbName}]" : $"CREATE DATABASE [{dbName}] COLLATE {collation}";
                using (var cmd = new SqlCommand(cmdText, cnn))
                {
                    cmd.ExecuteNonQuery();
                }

                if (enableRcsi)
                {
                    using var cmd = new SqlCommand($@"ALTER DATABASE [{dbName}] SET ALLOW_SNAPSHOT_ISOLATION ON; ALTER DATABASE [{dbName}] SET READ_COMMITTED_SNAPSHOT ON;", cnn);
                    cmd.ExecuteNonQuery();
                }
            }

            WaitUntilDbIsReady(connString);
        }

        public static void WaitUntilDbIsReady(string connString, int waitLimitInMilliseconds = 5000)
        {
            // after creation database is not ready immediately to accept incoming requests

            while (waitLimitInMilliseconds > 0)
            {
                try
                {
                    using var cnn = new SqlConnection(connString);
                    cnn.Open();

                    break;
                }
                catch (SqlException)
                {
                    Thread.Sleep(500);
                    waitLimitInMilliseconds -= 500;
                }
            }
        }
    }
}
