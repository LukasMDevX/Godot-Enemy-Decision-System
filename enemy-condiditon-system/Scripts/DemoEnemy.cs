using Godot;
using System;

public partial class DemoEnemy : Enemy
{
    protected override void InitAttacks()
    {
        attacks.Add(new EnemyAttack("Slash", 5f, 5f, 10, 0.8f, this, enemy => enemy.GetPlayerDistance() < 2.5f));
        attacks.Add(new EnemyAttack("Scream", 20f, 15f, 5, 2.2f, this, enemy => enemy.GetPlayerDistance() < 4.5f));
        attacks.Add(new EnemyAttack("Jump", 40f, 20f, 15, 1.4f, this, enemy => enemy.GetPlayerDistance() > 4.5f));
    }
}
    // Example additional conditions (not used in demo):
    // - PlayerInAreaCondition
    // - IsNight/DayTimeCondition
    // - EnemyHealthBelowThresholdCondition

