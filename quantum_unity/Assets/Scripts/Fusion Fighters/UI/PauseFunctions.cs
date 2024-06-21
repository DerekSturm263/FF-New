using UnityEngine;

public class PauseFunctions : MonoBehaviour
{
    public void Pause()
    {
        Quantum.CommandPause pause = new();
        QuantumRunner.Default.Game.SendCommand(pause);
    }

    public void Play()
    {
        Quantum.CommandPlay play = new();
        QuantumRunner.Default.Game.SendCommand(play);
    }
}
