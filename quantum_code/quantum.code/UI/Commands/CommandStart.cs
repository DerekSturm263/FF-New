using Photon.Deterministic;

namespace Quantum
{
	public unsafe class CommandStart : DeterministicCommand
	{
		public override void Serialize(BitStream stream)
		{

		}

		public void Execute(Frame f)
		{
			Log.Debug("Online match started!");

			f.Events.OnOnlineStart();
		}
	}
}
