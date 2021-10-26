using System;
using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;

public class StrawberryBehaviour : EnemyBehaviour {
  [SerializeField] private GameObject _projectilePrefab;

  [SerializeField] private float _delayBetweenShots;

  private List<Vector2> _projectileVectors;

  protected override void Awake() {
    StrawberryHealth strawberryHealth = gameObject.GetComponent<StrawberryHealth>();
    strawberryHealth.OnStrawberryStatusDamage += HandleStatusDamage;

    EnemyBehaviour enemyBehaviour = gameObject.GetComponent<EnemyBehaviour>();
    enemyBehaviour.OnElectrocuted += HandleElectrocuted;

    maxDistanceToAttack = 5;

    Health = strawberryHealth;
    isWallCollisionOn = true;
    base.Awake();

    _projectileVectors = new List<Vector2> {
      Vector2.up,
      Vector2.right,
      Vector2.down,
      Vector2.left
    };
  }

  private void HandleElectrocuted(object sender, EventArgs e) {
    StartCoroutine(Electrocute());
  }

  public override void Attack() {
    StartCoroutine(AttackPlayer());
  }

  protected override IEnumerator AttackPlayer() {
    _rb.velocity = Vector2.zero;

    while (true) {
      for (int i = 0; i < _projectileVectors.Count; i++) {
        GameObject projectileObject = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
        StrawberryProjectile projectile = projectileObject?.GetComponent<StrawberryProjectile>();

        Vector2 direction = _projectileVectors[i];
        projectile.SetDirection(direction, Vector2.SignedAngle(Vector2.up, direction));

        _projectileVectors[i] = Quaternion.Euler(0, 0, 45) * direction;
      }

      yield return new WaitForSeconds(_delayBetweenShots);
    }
  }
}