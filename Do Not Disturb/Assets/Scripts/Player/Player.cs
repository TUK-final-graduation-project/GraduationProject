using UnityEngine;

public class Player
{
    public float HP { get; private set; }
    public float MaxHP { get; private set; }
    public float Speed { get; private set; }
    public float RunSpeed { get; private set; }
    public int CoinCount { get; private set; }
    public State state;


    public Player(float hp, float maxHP, float speed, float runSpeed, int coinCount)
    {
        HP = hp;
        MaxHP = maxHP;
        Speed = speed;
        RunSpeed = runSpeed;
        CoinCount = coinCount;
        state = State.IDLE;
    }

    public void SetHP(float hp)
    {
        HP = Mathf.Clamp(hp, 0, MaxHP);
    }

    public void SetMaxHP(float maxHP)
    {
        MaxHP = maxHP;
        HP = Mathf.Clamp(HP, 0, MaxHP);
    }

    public void SetSpeed(float speed)
    {
        Speed = speed;
    }

    public void SetRunSpeed(float runSpeed)
    {
        RunSpeed = runSpeed;
    }

    public void SetCoins(int count)
    {
        CoinCount = count;
    }

    public void AddCoins(int count)
    {
        CoinCount += count;
    }

    public void RemoveCoins(int count)
    {
        CoinCount = Mathf.Max(CoinCount - count, 0);
    }
}
