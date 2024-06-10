using System;
using System.Collections.Generic;
using System.Linq;
// using System.Text.Json;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Tabasco.Services
{
	public class ContactService : IContactService
	{
		// private readonly ILogger<ContactService> _logger;
		private readonly Dictionary<string, List<string>> _contacts;
		private readonly ILogger<ContactService> _logger;
		
		public ContactService(ILogger<ContactService> logger)
		{
			_logger = logger;
			
			try
			{
				var json 	 = File.ReadAllText("contacts.json");
				_logger.LogInformation("JSON Content: {json}", json);
				// var contacts = JsonSerializer.Deserialize<Contacts>(json);
				var contacts = JsonConvert.DeserializeObject<Contacts>(json);
				if (contacts != null && contacts.Teams != null)
				{
					_contacts = contacts.Teams;
					_logger.LogInformation("Contacts loaded successfully: {contacts}", JsonConvert.SerializeObject(_contacts));
				}
				else
				{
					_contacts = new Dictionary<string, List<string>>();
					_logger.LogError("Failed to deserialize contacts.json: contacts or contacts.Teams is null");
				}
				// _contacts = contacts?.Teams ?? new Dictionary<string, List<string>>();
				// _contacts = contacts.Teams;
				// _logger.LogInformation("Contacts loaded successfully: {contacts}", JsonSerializer.Serialize(_contacts));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading contacts.json");
				_contacts = new Dictionary<string, List<string>>();
			}
			/*
			catch (System.Exception)
			{	
				// _logger.LogError("point");
				throw;
			}*/
		}
		
		public List<string> GetContactsNumbers(string team)
		{			
			_logger.LogInformation("-------------------");
			_logger.LogInformation("Team: " + team);
			_logger.LogInformation("Contacts: " + JsonConvert.SerializeObject(_contacts));
			_logger.LogInformation("-------------------");
			
			if (_contacts.TryGetValue(team, out var numbers))
			{
				_logger.LogInformation("-------------------");
				_logger.LogInformation("Numbers for team " + team + ": " + string.Join(", ", numbers));
				_logger.LogInformation("-------------------");	
				return numbers;
			}
			/// return _contacts["devops"];
			var adminContacts = _contacts.ContainsKey("admin") ? _contacts["admin"] : new List<string>();
			_logger.LogInformation("Admin Numbers: " + string.Join(", ", adminContacts));
			return adminContacts;
		}
		
		private class Contacts
		{
			public Dictionary<string, List<string>>? Teams { get; set; }
		}
	}
}