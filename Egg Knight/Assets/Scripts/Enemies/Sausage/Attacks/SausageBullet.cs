using UnityEngine;
using UnityEngine.Tilemaps;

public class SausageBullet : Projectile {
  protected override void Despawn() {
    Destroy(gameObject);
  }

  protected override void OnTriggerEnter2D(Collider2D collider) {
    PlayerHealth playerHealth = collider.gameObject.GetComponent<PlayerHealth>();
    
    if (playerHealth != null) {
      playerHealth.Damage(_damage);
    }

    if (collider.gameObject.layer == LayerMask.NameToLayer("Obstacle") || playerHealth != null) {
      StopCoroutine(DespawnTimer());
      Despawn();
    }
  }
}