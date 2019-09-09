using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;


namespace ReconnectNetwork
{
	public partial class ReconnectNetwork : ServiceBase
	{
		private int eventId = 1;
		private string LoginAPI = "https://bknet53.hust.edu.vn/login";
		public ReconnectNetwork()
		{
			InitializeComponent();
			eventLog1 = new System.Diagnostics.EventLog();
			if (!System.Diagnostics.EventLog.SourceExists("MySource"))
			{
				System.Diagnostics.EventLog.CreateEventSource(
					"MySource", "MyNewLog");
			}
			eventLog1.Source = "MySource";
			eventLog1.Log = "MyNewLog";
		}

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

		public async void OnTimer(object sender, ElapsedEventArgs args)
		{
			// TODO: Insert monitoring activities here.
			try
			{
				using (HttpClient client = new HttpClient())
				{
					try
					{
						string responseBody = await client.GetStringAsync(new Uri("https://www.google.com/"));
					}
					catch
					{
						eventLog1.WriteEntry("lost connect", EventLogEntryType.Information, eventId++);
						HttpWebRequest req = (HttpWebRequest)WebRequest.Create(new Uri(this.LoginAPI));
						req.Method = "POST";
						req.Host = "bknet53.hust.edu.vn";
						req.KeepAlive = true;
						req.ContentLength = 56;
						req.Headers.Add("Cache-Control", "max-age=0");
						req.Headers.Add("Origin", "https://bknet53.hust.edu.vn");
						req.Headers.Add("Upgrade-Insecure-Requests:", "1");
						req.ContentType = "application/x-www-form-urlencoded";
						req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.132 Safari/537.36";
						req.Headers.Add("Sec-Fetch-Mode", "navigate");
						req.Headers.Add("Sec-Fetch-User", "?1");
						req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
						req.Headers.Add("Sec-Fetch-Site", "same-origin");
						req.Referer = "https://bknet53.hust.edu.vn/login?";
						req.Headers.Add("Accept-Encoding", "gzip, deflate, br");
						req.Headers.Add("Accept-Language", "vi-VN,vi;q=0.9,fr-FR;q=0.8,fr;q=0.7,en-US;q=0.6,en;q=0.5");
						req.Headers.Add("Cookie", "_ga=GA1.3.2125825283.1566549382; _gid=GA1.3.921147183.1568027280");
						req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
						try
						{
							string body = string.Empty;
							using (var streamWriter = new StreamWriter(req.GetRequestStream()))
							{
								body = "username=thuan.nguyendinh&password=ThuanND%402013&dst=http%3A%2F%2Fwww.hust.edu.vn%2F&popup=true";
								streamWriter.Write(body);
								streamWriter.Flush();
								streamWriter.Close();
								try
								{
									string res = string.Empty;
									using (var httpRes = (HttpWebResponse)(req.GetResponse()))
									{
										using (var reader = new StreamReader(httpRes.GetResponseStream()))
										{
											res = reader.ReadToEnd();
										}
									}
								}
								catch
								{

								}
							}
						}
						catch
						{

						}
					}
				}
			}
			catch
			{
			}
		}

		protected override void OnStart(string[] args)
		{
			eventLog1.WriteEntry("In OnStart.");
			ServiceStatus serviceStatus = new ServiceStatus();
			serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
			serviceStatus.dwWaitHint = 100000;
			SetServiceStatus(this.ServiceHandle, ref serviceStatus);
			Timer timer = new Timer();
			timer.Interval = 30000;
			timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
			timer.Start();
			serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
			SetServiceStatus(this.ServiceHandle, ref serviceStatus);
		}

		protected override void OnStop()
		{
			eventLog1.WriteEntry("In OnStop.");
			ServiceStatus serviceStatus = new ServiceStatus();
			serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
			serviceStatus.dwWaitHint = 100000;
			SetServiceStatus(this.ServiceHandle, ref serviceStatus);
			serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
			SetServiceStatus(this.ServiceHandle, ref serviceStatus);
		}

		protected override void OnContinue()
		{
			eventLog1.WriteEntry("In OnContinue.");
		}
	}

	public enum ServiceState
	{
		SERVICE_STOPPED = 0x00000001,
		SERVICE_START_PENDING = 0x00000002,
		SERVICE_STOP_PENDING = 0x00000003,
		SERVICE_RUNNING = 0x00000004,
		SERVICE_CONTINUE_PENDING = 0x00000005,
		SERVICE_PAUSE_PENDING = 0x00000006,
		SERVICE_PAUSED = 0x00000007,
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ServiceStatus
	{
		public int dwServiceType;
		public ServiceState dwCurrentState;
		public int dwControlsAccepted;
		public int dwWin32ExitCode;
		public int dwServiceSpecificExitCode;
		public int dwCheckPoint;
		public int dwWaitHint;
	};
}
