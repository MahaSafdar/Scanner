using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using static WinFormsApp2.Dashboard;

namespace ProScanner.Database
{
    public class DatabaseHelper
    {
        /*private static string connectionString = ConfigurationManager.ConnectionStrings["ProScannerConnectionString"].ConnectionString;

        public static List<VulnerabilityItem> GetLatestVulnerabilities()
        {
            List<VulnerabilityItem> vulnerabilities = new List<VulnerabilityItem>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT VulnerabilityName, Severity, 'Open' AS Status, ScanDateTime FROM VulnerabilityScan ORDER BY ScanDateTime DESC", connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        vulnerabilities.Add(new VulnerabilityItem
                        {
                            Title = reader.GetString(0),
                            Severity = reader.GetString(1),
                            Status = reader.GetString(2),
                            DetectedDate = reader.GetDateTime(3)
                        });
                    }
                }
            }

            return vulnerabilities;
        }

        public static DataTable GetScanOverview()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT ScanDateTime, COUNT(*) AS ScanCount FROM ScanReports GROUP BY ScanDateTime ORDER BY ScanDateTime DESC", connection))
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable scanOverview = new DataTable();
                    dataAdapter.Fill(scanOverview);
                    return scanOverview;
                }
            }
        }

        public static DataTable GetDailyTaskStats()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT TaskStatus, COUNT(*) AS TaskCount FROM Tasks GROUP BY TaskStatus", connection))
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dailyTaskStats = new DataTable();
                    dataAdapter.Fill(dailyTaskStats);
                    return dailyTaskStats;
                }
            }
        }

        public static void CreateIPRangeRecord(string startIP, string endIP, string description)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("INSERT INTO IPRanges (StartIP, EndIP, Description) VALUES (@StartIP, @EndIP, @Description)", connection))
                {
                    command.Parameters.AddWithValue("@StartIP", startIP);
                    command.Parameters.AddWithValue("@EndIP", endIP);
                    command.Parameters.AddWithValue("@Description", description);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void LogICMPScanResult(string ipAddress, float responseTime, float packetLoss, DateTime scanDateTime)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("INSERT INTO ICMPScan (IPAddress, ResponseTime, PacketLoss, ScanDateTime) VALUES (@IPAddress, @ResponseTime, @PacketLoss, @ScanDateTime)", connection))
                {
                    command.Parameters.AddWithValue("@IPAddress", ipAddress);
                    command.Parameters.AddWithValue("@ResponseTime", responseTime);
                    command.Parameters.AddWithValue("@PacketLoss", packetLoss);
                    command.Parameters.AddWithValue("@ScanDateTime", scanDateTime);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void LogPortScanResult(string ipAddress, int port, string protocol, string state, DateTime scanDateTime)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("INSERT INTO PortScan (IPAddress, Port, Protocol, State, ScanDateTime) VALUES (@IPAddress, @Port, @Protocol, @State, @ScanDateTime)", connection))
                {
                    command.Parameters.AddWithValue("@IPAddress", ipAddress);
                    command.Parameters.AddWithValue("@Port", port);
                    command.Parameters.AddWithValue("@Protocol", protocol);
                    command.Parameters.AddWithValue("@State", state);
                    command.Parameters.AddWithValue("@ScanDateTime", scanDateTime);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void LogVulnerabilityScanResult(string ipAddress, int vulnerabilityID, string vulnerabilityName, string description, string severity, DateTime scanDateTime)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("INSERT INTO VulnerabilityScan (IPAddress, VulnerabilityID, VulnerabilityName, Description, Severity, ScanDateTime) VALUES (@IPAddress, @VulnerabilityID, @VulnerabilityName, @Description, @Severity, @ScanDateTime)", connection))
                {
                    command.Parameters.AddWithValue("@IPAddress", ipAddress);
                    command.Parameters.AddWithValue("@VulnerabilityID", vulnerabilityID);
                    command.Parameters.AddWithValue("@VulnerabilityName", vulnerabilityName);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@Severity", severity);
                    command.Parameters.AddWithValue("@ScanDateTime", scanDateTime);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void LogScanReport(string reportName, DateTime scanDateTime, string summary, string recommendations)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("INSERT INTO ScanReports (ReportName, ScanDateTime, Summary, Recommendations) VALUES (@ReportName, @ScanDateTime, @Summary, @Recommendations)", connection))
                {
                    command.Parameters.AddWithValue("@ReportName", reportName);
                    command.Parameters.AddWithValue("@ScanDateTime", scanDateTime);
                    command.Parameters.AddWithValue("@Summary", summary);
                    command.Parameters.AddWithValue("@Recommendations", recommendations);
                    command.ExecuteNonQuery();
                }
            }
        }*/
    }
    }
