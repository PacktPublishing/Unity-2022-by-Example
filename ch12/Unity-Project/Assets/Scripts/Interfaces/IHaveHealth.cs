internal interface IHaveHealth
{
    void HealthChanged(int amount);

    void Died();
}