using Godot;
using System;
using System.Buffers;

public class EnemyAttack
{
    public string Name;
    public float StaminaCost;
    public float CooldownTime;
    public bool IsInCooldown;
    public int Damage;
    public float Duration;

    private Func<Enemy, bool> condition;

    private Node owner;

    public EnemyAttack(string name, float staminaCost, float cooldownTime, int damage,float duration, Node owner, Func<Enemy, bool> condition = null)
    {
        Name = name;
        StaminaCost = staminaCost;
        CooldownTime = cooldownTime;
        IsInCooldown = false;
        Damage = damage;
        Duration = duration;
        this.owner = owner;
        this.condition = condition ?? (enemy => true); // Default: always usable
    }

    public async void StartCooldown()
    {
        IsInCooldown = true;
        await owner.ToSignal(owner.GetTree().CreateTimer(CooldownTime), "timeout");
        IsInCooldown = false;
    }

    public bool CanUse(Enemy enemy)
    {
        return !IsInCooldown &&
               enemy.stamina >= StaminaCost &&
               condition(enemy);
    }
}
