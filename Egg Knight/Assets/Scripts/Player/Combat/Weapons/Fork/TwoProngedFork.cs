using UnityEngine;

public class TwoProngedFork : BasePlayerWeapon {
  [SerializeField] private Transform _attackPoint;
  [SerializeField] private float _attackWidth;
  [SerializeField] private float _attackHeight;

  [SerializeField] private LayerMask _enemyLayer;
  [SerializeField] private LayerMask _obstacleLayer;

  public override void EnableHitbox() {
    float hitboxAngle = transform.eulerAngles.z;

    Collider2D[] enemiesInRange = Physics2D.OverlapBoxAll(
      _attackPoint.position,
      new Vector2(_attackWidth, _attackHeight),
      hitboxAngle,
      _enemyLayer
    );
    
    Collider2D[] obstaclesInRange = Physics2D.OverlapBoxAll(
      _attackPoint.position,
      new Vector2(_attackWidth, _attackHeight),
      hitboxAngle,
      _obstacleLayer
    );

    Collider2D[] enemiesHit = new Collider2D[enemiesInRange.Length + obstaclesInRange.Length];
    enemiesInRange.CopyTo(enemiesHit, 0);
    obstaclesInRange.CopyTo(enemiesHit, enemiesInRange.Length);
    
    Collider2D[] coinsInRange = Physics2D.OverlapBoxAll(
      _attackPoint.position,
      new Vector2(_attackWidth, _attackHeight),
      hitboxAngle,
      _coinLayer
    );

    DamageEnemies(enemiesHit);
    CollectCoins(coinsInRange);
  }

  protected override void OnDrawGizmosSelected() {
    Gizmos.color = Color.red;
    Gizmos.DrawWireCube(_attackPoint.position, new Vector3(_attackWidth, _attackHeight, 1));
  }
}
