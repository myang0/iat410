using UnityEngine;

public class EggGuardSpear : BaseEnemyWeapon {
  [SerializeField] private Transform _attackPoint;
  [SerializeField] private float _attackWidth;
  [SerializeField] private float _attackHeight;

  [SerializeField] private LayerMask _playerLayer;

  public override void EnableHitbox() {
    float hitboxAngle = transform.eulerAngles.z;

    Collider2D[] playersInRange = Physics2D.OverlapBoxAll(
      _attackPoint.position,
      new Vector2(_attackWidth, _attackHeight),
      hitboxAngle,
      _playerLayer
    );

    if (playersInRange.Length > 0) {
      DamagePlayer(playersInRange[0]);
    }
  }

  public override void OnAnimationEnd() {
    RotateToPlayer();
  }

  protected override void OnDrawGizmosSelected() {
    Gizmos.color = Color.red;
    Gizmos.DrawWireCube(_attackPoint.position, new Vector3(_attackWidth, _attackHeight, 1));
  }

  public void RotateToPlayer() {
    Vector2 vectorToPlayer = VectorHelper.GetVectorToPoint(transform.position, _playerObject.transform.position);
    float angleToPlayer = Vector2.SignedAngle(Vector2.up, vectorToPlayer);

    Vector3 currentRotation = transform.eulerAngles;

    transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y, angleToPlayer);
  }
}
