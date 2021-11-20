using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaPodBehaviour : EnemyBehaviour {
  [SerializeField] private float _attackDamage;

  protected override void Awake() {
    PeaPodHealth peaPodHealth = gameObject.GetComponent<PeaPodHealth>();
    peaPodHealth.OnPeaPodStatusDamage += HandleStatusDamage;

    EnemyBehaviour enemyBehaviour = gameObject.GetComponent<EnemyBehaviour>();
    enemyBehaviour.OnElectrocuted += HandleElectrocuted;

    Health = peaPodHealth;
    base.Awake();
  }

  private void HandleElectrocuted(object sender, EventArgs e) {
    StartCoroutine(Electrocute());
  }

  protected override IEnumerator AttackPlayer() {
    yield break;
  }

  private void OnTriggerEnter2D(Collider2D col) {
    if (col.gameObject.CompareTag("Player")) {
      GameObject playerObject = col.gameObject;
      PlayerHealth playerHealth = playerObject?.GetComponent<PlayerHealth>();

      playerHealth?.Damage(_attackDamage);
    }
  }
}