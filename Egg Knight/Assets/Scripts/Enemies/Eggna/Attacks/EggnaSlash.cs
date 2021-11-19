using System.Collections;
using UnityEngine;

public class EggnaSlash : MonoBehaviour {
  private Rigidbody2D _rb;

  [SerializeField] private float _animationDuration;

  [SerializeField] private float _forwardsMomentum;
  [SerializeField] private float _drag;

  [SerializeField] private Transform _attackPivot;
  [SerializeField] private Transform _attackPoint;

  [SerializeField] private GameObject _slashObject;

  private Transform _playerTransform;

  private void Awake() {
    _rb = GetComponent<Rigidbody2D>();

    _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

    StartAttack();
  }

  public void StartSlash() {
    _attackPivot.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, GetVectorToPlayer()));

    if (_slashObject != null) {
      Instantiate(_slashObject, transform.position, Quaternion.identity);
    }
  }

  public void StartAttack() {
    StartCoroutine(SlashMomentum());
  }

  private IEnumerator SlashMomentum() {
    Vector2 vectorToPlayer = GetVectorToPlayer();

    _rb.velocity = vectorToPlayer * _forwardsMomentum;
    _rb.drag = _drag;

    yield return new WaitForSeconds(_animationDuration);

    _rb.velocity = Vector2.zero;
    _rb.drag = 0;
  }

  private Vector2 GetVectorToPlayer() {
    return VectorHelper.GetVectorToPoint(transform.position, _playerTransform.position);
  }
}
