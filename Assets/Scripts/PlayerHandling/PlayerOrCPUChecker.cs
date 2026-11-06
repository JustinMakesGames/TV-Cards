using UnityEngine;

public enum PlayerOrCPU
{
    Player,
    CPU
}
public class PlayerOrCPUChecker : MonoBehaviour
{

    public PlayerOrCPU playerOrCPU;

    public void SetPlayer(PlayerOrCPU playerOrCPU)
    {
        this.playerOrCPU = playerOrCPU;
    }

    public PlayerOrCPU PlayerOrCPU()
    {
        return playerOrCPU;
    }
}
