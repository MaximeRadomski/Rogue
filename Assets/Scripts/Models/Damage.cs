public class Damage
{
    public int Amount;
    public bool Critical;

    public Damage(int amount = 0, bool critical = false)
    {
        Amount = amount;
        Critical = critical;
    }
}
