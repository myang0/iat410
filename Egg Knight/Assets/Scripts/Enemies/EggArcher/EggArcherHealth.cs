using System;
using System.Collections.Generic;
using UnityEngine;

public class EggArcherHealth : EnemyHealth {
  public event EventHandler<EnemyStatusEventArgs> OnEggArcherStatusDamage;

  protected override void Awake() {
    EggnaHealth.OnEggnaDeath += HandleEggnaDeath;

    base.Awake();
  }

  private void HandleEggnaDeath(object sender, EventArgs e) {
    base.Die();
  }

  public override void DamageWithStatuses(float amount, List<StatusCondition> statuses) {
    if (isInvulnerable == false) {
      OnEggArcherStatusDamage?.Invoke(this, new EnemyStatusEventArgs(statuses));

      Damage(amount); 
    }
  }

  public override bool DamageWithStatusesAndType(float amount, List<StatusCondition> statuses, DamageType type) {
    if (isInvulnerable == false) {
      bool isDamageDealt = DamageWithType(amount, type);
      if (isDamageDealt) OnEggArcherStatusDamage?.Invoke(this, new EnemyStatusEventArgs(statuses));

      return isDamageDealt;
    }

    return false;
  }

  private void OnDestroy() {
    EggnaHealth.OnEggnaDeath -= HandleEggnaDeath;
  }
}
