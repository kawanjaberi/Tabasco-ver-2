using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Tabasco.Models;
using Tabasco.Services;

namespace Tabasco.Controllers
{
	[ApiController]
	[Route("v1/api/sms")]
	public class SmsController : ControllerBase
	{
		private readonly ILogger<SmsController> _logger;
		private readonly ISmsService _smsService;
		private readonly IContactService _contactService;
		
		// private readonly string senderNumber = Environment.GetEnvironmentVariable(KAVEHNEGAR_SENDER_NUMBER);
		
		public SmsController (ISmsService smsService, IContactService contactService, ILogger<SmsController> logger)
		{
			_smsService		= 	smsService;
			_contactService = 	contactService;
			_logger			= 	logger;
		}
		
		[HttpPost("sendteam/{team}")]
		
		public async Task<IActionResult> SendSms(string team, [FromBody] SmsRequest request)
		{	
			try
			{
				_logger.LogInformation("CONT:Received request to send SMS for team: {team}", team);
				// Get the recipients from the environment variable
				var recipientsJson = Environment.GetEnvironmentVariable(team);

				if (string.IsNullOrEmpty(recipientsJson))
				{
					_logger.LogWarning("CONT:Invalid team or no recipients found for the team: {team}", team);
					return BadRequest("CONT:Invalid team or no recipients found for the team.");
				}

				var recipients = JsonSerializer.Deserialize<List<string>>(recipientsJson);
				if (recipients == null || recipients.Count == 0)
				{
					_logger.LogWarning("CONT:No recipients found after parsing JSON for team: {team}", team);
					return BadRequest("CONT:Invalid team or no recipients found for the team.");
				}

				foreach (var recipient in recipients)
				{
					var smsRequest = new SmsRequest
					{
						Sender = "",
						Recipient = recipient,
						Message = request.Message
					};
					await _smsService.SendSmsAsync(smsRequest);
				}

				_logger.LogInformation("CONT:SMS sent successfully for team: {team}", team);
				return Ok("SMS Sent Successfully");
			}
			catch (Exception ex)
			{
				// Log the exception here if necessary
				// _logger.LogError(ex, "CONT:Exception occurred while sending SMS for team: {team}", team);
				_logger.LogError("CONT:Exception occurred while sending SMS for team: {team}", team);
				return StatusCode(500, new { message = ex.Message });
			}
			
		}
		
		[HttpPost("sendsingle")]
		public async Task<IActionResult> SendSms([FromBody] SmsRequest request)
		{
			try
			{
				_logger.LogInformation("Received request to send SMS to {Recipient}", request.Recipient);
				await _smsService.SendSmsAsync(request);
				_logger.LogInformation("SMS sent successfully for member: {member}", request.Recipient);
				return Ok("SMS Sent Successfully");
			}
			catch (Exception ex)
			{
				// Log the exception here if necessary
				// _logger.LogError(ex, "Exception occurred while sending SMS to {Recipient}", request.Recipient);
				_logger.LogError("Exception occurred while sending SMS to {Recipient}", request.Recipient);
				return StatusCode(500, new { message = ex.Message });
			}
		}
		
		[HttpPost("webhook")]
		public async Task<IActionResult> HandlePrometheusAlert([FromBody] PrometheusAlertPayload payload)
		{
			try 
			{
				_logger.LogInformation("Received Prometheus alert");
				
				foreach	 (var alert in payload.Alerts)
				{

					if (alert.Labels.TryGetValue("sms", out var smsEnabled) && smsEnabled == "on")
					{

						var team			= alert.Labels.TryGetValue("smsteam", out var smsteam) ? smsteam : "devops";
						_logger.LogInformation("-------------------");
						_logger.LogInformation(team);
						_logger.LogInformation("-------------------");
						var contactNumbers	= _contactService.GetContactsNumbers(team);
						_logger.LogInformation("Team: {team}, Contacts: {contacts}", team, string.Join(", ", contactNumbers));
						var message	 		= $"!Alert [{alert.Status} - {alert.Labels["severity"]}]\n" +
											$"{alert.Labels["alertname"]}\n" +
											$"job: {alert.Labels["job"]}\n" +
											$"instance: {alert.Labels["instance"]}\n" +
											$"Description: {alert.Annotations["description"]}";
											
						foreach	(var number in contactNumbers)
						{
							var smsRequest = new SmsRequest
							{
								Sender		= "",
								Recipient	= number,
								Message		= message
							};
							await _smsService.SendSmsAsync(smsRequest);
							
						}
					}
				}
				
				return Ok("Alerts processed successfully");
			}
			catch (Exception ex)
			{
				// _logger.LogError(ex, "Exception occurred while processing Prometheus alert");
				_logger.LogError("Exception occurred while processing Prometheus alert");
				return StatusCode(500, new { message = ex.Message });
			}
		}
		
	}
}