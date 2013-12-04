using AliaSQL.Core.Model;

namespace AliaSQL.Core.Services
{
	
	public interface ILogMessageGenerator
	{
		string GetInitialMessage(TaskAttributes taskAttributes);
	}
}