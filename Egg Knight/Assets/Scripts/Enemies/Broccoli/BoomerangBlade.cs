using System;
using System.Collections;
using UnityEngine;

public class BoomerangBlade : MonoBehaviour {
  [SerializeField] private float _damage;

  private float _speed;
  private float _speedMultiplier = 1.0f;
  private Vector2 _origVelocity;

  private bool _isReturning = false;

  private Transform _bossTransform;

  private Rigidbody2D _rb;
  private Collider2D _collider;

  [SerializeField] private GameObject _singleTimeSound;
  [SerializeField] private AudioClip _clip;

  private void Awake() {
    _bossTransform = GameObject.Find("BrigandBroccoli")?.transform;

    _collider = GetComponent<Collider2D>();
    _rb = GetComponent<Rigidbody2D>();

    Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    _speed = Vector2.Distance((Vector2)transform.position, (Vector2)playerTransform?.position);

    Vector2 vectorToPlayer = VectorHelper.GetVectorToPoint(transform.position, playerTransform.position);
    _rb.velocity = vectorToPlayer * _speed;
    _origVelocity = _rb.velocity;

    BroccoliHealth.OnBroccoliDeath += HandleBroccoliDeath;

    StartCoroutine(DisableCollider());
    StartCoroutine(ReturnToSender());
    StartCoroutine(PlaySounds());
    StartCoroutine(DespawnDelay());
  }

  private void HandleBroccoliDeath(object sender, EventArgs e) {
    Destroy(gameObject);
  }

  private IEnumerator DisableCollider() {
    _collider.enabled = false;

    yield return new WaitForSeconds(0.5f);

    _collider.enabled = true;
  }

  private IEnumerator ReturnToSender() {
    yield return new WaitForSeconds(1.0f);

    _isReturning = true;
  }

  private IEnumerator PlaySounds() {
    while (true) {
      Instantiate(_singleTimeSound, transform.position, Quaternion.identity)
        .GetComponent<SingleTimeSound>()
        .LoadClipAndPlay(_clip);

      yield return new WaitForSeconds(0.1f);
    }
  }

  private IEnumerator DespawnDelay() {
    yield return new WaitForSeconds(5);

    Destroy(gameObject);
  }

  private void FixedUpdate() {
    if (_isReturning) {
      _speedMultiplier -= 0.0375f;
      _rb.velocity = _origVelocity * _speedMultiplier;
    }
  }

  private void OnTriggerEnter2D(Collider2D other) {
    PlayerHealth pHealth = other.GetComponent<PlayerHealth>();
    BroccoliHealth bHealth = other.GetComponent<BroccoliHealth>();

    if (pHealth != null) {
      pHealth.Damage(_damage);
    }

    if (bHealth != null) {
      Animator bAnim = other.GetComponent<Animator>();
      if (bAnim.GetBool("IsThrowing")) {
        bAnim.SetBool("IsThrowing", false);
      }

      Destroy(gameObject);
    }
  }

  private void OnDestroy() {
    BroccoliHealth.OnBroccoliDeath -= HandleBroccoliDeath;
  }
}
