using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;

namespace CS2Executes.Managers
{
	public sealed class QueueManager : BaseManager
	{
		private readonly HashSet<CCSPlayerController> _waiting = new();

		public QueueManager() {}

		public bool AddToWaitingQueue(CCSPlayerController player)
		{
			if(_waiting.Contains(player))
			{
				return false;
			}
			
			_waiting.Add(player);

			return true;
		}

		public bool RemoveFromWaitingQueue(CCSPlayerController player)
		{
			if(!_waiting.Contains(player))
			{
				return false;
			}
			
			_waiting.Remove(player);

			return true;
		}

		public bool IsInWaitingQueue(CCSPlayerController player)
		{
			return _waiting.Contains(player);
		}

	}
}