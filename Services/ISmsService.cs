using Tabasco.Models;

namespace Tabasco.Services
{
	public interface ISmsService
	{
		Task SendSmsAsync(SmsRequest smsRequest);
	}
}