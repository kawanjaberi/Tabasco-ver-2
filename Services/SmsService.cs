// Services/SmsService.cs
using Kavenegar;
using Kavenegar.Exceptions;
using Tabasco.Models;

namespace Tabasco.Services
{
	public class SmsService : ISmsService
	{
		private readonly ILogger<SmsService> _logger;
		private readonly KavenegarApi _api;
		private readonly string senderNumber = Environment.GetEnvironmentVariable("KAVEHNEGAR_SENDER_NUMBER");
		
		public SmsService(string apiKey, ILogger<SmsService> logger)
		{
			_api 	= new KavenegarApi(apiKey);
			_logger	= logger;
		}

		public async Task SendSmsAsync(SmsRequest request)
		{
			try
			{
				_logger.LogInformation("SVC:Sending SMS to {Recipient}", request.Recipient);
				var result = await Task.Run(() => _api.Send(senderNumber, request.Recipient, request.Message));
				_logger.LogInformation("SVC:SMS sent successfully. Message ID: {MessageId}", result.Messageid.ToString());

			}
			catch (ApiException ex)
			{
				// Handle API exception: Kavehnegar Service don't return 200
				_logger.LogError(ex, "SVC:API error occurred while sending SMS to {Recipient}", request.Recipient);
				throw new Exception("SVC:Error sending SMS via Kavenegar API: " + ex.Message);
			}
			
			catch (HttpException ex)
			{
				// Handle HTTP exception: Kavehnegar Service connection in problem
				_logger.LogError(ex, "SVC:HTTP error occurred while sending SMS to {Recipient}", request.Recipient);
				throw new Exception("SVC:HTTP error when connecting to Kavenegar API: " + ex.Message);
			}
			
		}
	}
}
