using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomHealth : EnemyHealth
{
    public event EventHandler<EnemyStatusEventArgs> OnMushroomStatusDamage;

    public override void DamageWithStatuses(float amount, List<StatusCondition> statuses) {
        OnMushroomStatusDamage?.Invoke(this, new EnemyStatusEventArgs(statuses));

        Damage(amount);
    }

    public override void DamageWithStatusesAndType(float amount, List<StatusCondition> statuses, DamageType type) {
        OnMushroomStatusDamage?.Invoke(this, new EnemyStatusEventArgs(statuses));

        DamageWithType(amount, type);
    }
}