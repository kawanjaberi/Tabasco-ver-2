using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tabasco.Models
{
	public class PrometheusAlertPayload
	{
		public string Receiver { get; set; }
		public string Status { get; set; }
		public List<Alert> Alerts { get; set; }
		public Dictionary<string, string> GroupLabels { get; set; }
		public Dictionary<string, string> CommonLabels { get; set; }
		public Dictionary<string, string> CommonAnnotations { get; set; }
		public string ExternalURL { get; set; }
		public string Version { get; set; }
		public string GroupKey { get; set; }
		public int TruncatedAlerts { get; set; }
		
		 public class Alert
        {
            public string Status { get; set; }
            public Dictionary<string, string> Labels { get; set; }
            public Dictionary<string, string> Annotations { get; set; }
            public string StartsAt { get; set; }
            public string EndsAt { get; set; }
            public string GeneratorURL { get; set; }
            public string Fingerprint { get; set; }
        }
	}
}