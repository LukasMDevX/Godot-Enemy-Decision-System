using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public partial class Enemy : Node3D
{
    //Moves
    public List<EnemyAttack> attacks = new();
    private EnemyAttack currentAttack;

    [Export]
    private float maxStamina = 100.0f;
    public float stamina;
    [Export]
    private float staminaRegenerationRate = 2.0f;
    [Export]
    public float attentionRange = 8.0f;
    private float globalCooldownTime = 1.5f;
    private bool IsInCooldown = false;
    private bool IsPerformingAttack = false;
    private float attackEndTime;

    private float distanceToPlayer;

    [Export]
    public Label DebugLabel;

    public override void _Ready()
    {
        stamina = maxStamina;

        InitAttacks();
    }

    protected virtual void InitAttacks()
    {

    }
    protected virtual void InitBossName()
    {

    }
    public void _on_distance_to_player_slider_value_changed(float value)
    {
        distanceToPlayer = value;
        GD.Print("Distance to player is now: " + distanceToPlayer);
    }

    public override void _Process(double delta)
    {
        RegenerateStamina((float)delta);

        if (TargetInAttentionRange())
        {
            DecideWhichAttackToUse();
        }

        if (IsPerformingAttack)
        {
            float remaining = attackEndTime - (Time.GetTicksMsec() / 1000f);
            DebugLabel.Text = $"Attacking: {currentAttack.Name}\nRemaining: {remaining:F2}s";
        }
    }


    private void RegenerateStamina(float delta)
    {
        stamina += staminaRegenerationRate * delta;
    }

    private void DecideWhichAttackToUse()
    {
        if (!IsInCooldown && !IsPerformingAttack)
        {
            var availableMoves = attacks.Where(a => a.CanUse(this)).ToList();
            if (availableMoves.Count > 0)
            {
                var chosen = availableMoves[(int)GD.Randi() % availableMoves.Count];
                PerformAttack(chosen);
            }
            else
            {
                StartGeneralCooldown();
            }
        }
    }

    public float GetPlayerDistance()
    {
        return distanceToPlayer;
    }

    private bool TargetInAttentionRange()
    {
        return distanceToPlayer < attentionRange;
    }

    private void PerformAttack(EnemyAttack attack)
    {
        IsPerformingAttack = true;
        attackEndTime = Time.GetTicksMsec() / 1000f + attack.Duration;
        currentAttack = attack;

        stamina -= attack.StaminaCost;
        attack.StartCooldown();
        StartGeneralCooldown();
        SimulateAttackDuration(attack.Duration);
    }

    private async void StartGeneralCooldown()
    {
        IsInCooldown = true;
        await ToSignal(GetTree().CreateTimer(globalCooldownTime), "timeout");
        IsInCooldown = false;
    }
    private async void SimulateAttackDuration(float duration)
    {
        await ToSignal(GetTree().CreateTimer(duration), "timeout");
        OnAttackFinished();
    }

    public void OnAttackFinished()
    {
        IsPerformingAttack = false;
        DebugLabel.Text = $"Attacking: ";
    }
}
