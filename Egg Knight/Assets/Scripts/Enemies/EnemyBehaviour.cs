using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Stage;
using UnityEngine;
using UnityEngine.Assertions;
using Pathfinding;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public abstract class EnemyBehaviour : MonoBehaviour {
  [HideInInspector] public Rigidbody2D rb;

  [SerializeField] protected float _maxSpeed;
  protected float _currentSpeed;

  protected Health Health;

  public event EventHandler OnYolked;
  public event EventHandler OnFrosted;
  public event EventHandler OnIgnited;
  public event EventHandler OnElectrocuted;
  public event EventHandler OnBleed;
  public event EventHandler OnWeakened;

  public float maxDistanceToAttack;
  public float minDistanceToAttack;
  public float attackCooldownMax;
  public bool isAttackOffCooldown;
  public bool isInAttackAnimation;
  public bool isStunned;
  public float alertRange;
  public bool isTurningEnabled;
  public bool decrementEnemyCountOnDeath;
  public bool spawnedByEggna = false;
  public bool disableRegularDrops;
  public bool disableDeathRotation;
  public bool notAffectedByDropMods;

  public bool isWandering;
  private Vector3 _wanderDestination;

  protected Transform _playerTransform;
  protected PlayerInventory _playerInventory;
  public PlayerInventory PlayerInventory {
    get {
      return _playerInventory;
    }
  }

  public bool isWallCollisionOn;
  private EnemyMovement _eMovement;

  [SerializeField] private Animator alertAnimator;
  public bool isDead;

  protected virtual void Awake() {
    Assert.IsNotNull(Health);
    _currentSpeed = _maxSpeed;
    alertRange = 4f;

    rb = gameObject.GetComponent<Rigidbody2D>();
    isAttackOffCooldown = true;
    _eMovement = gameObject.GetComponent<EnemyMovement>();

    Health.OnPreDeath += HandlePreDeath;
    Health.OnDeath += HandleDeath;

    GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
    _playerTransform = playerObject.transform;
    _playerInventory = playerObject.GetComponent<PlayerInventory>();

    Vector3 pos = transform.position;
    transform.position = new Vector3(pos.x, pos.y, ZcoordinateConsts.Character);
    InvokeRepeating(nameof(InterruptWander), 0f, 2f);
  }

  protected virtual void Update() {
    if (isDead) return;
    if (!isWandering && isTurningEnabled) {
      SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
      spriteRenderer.flipX = transform.position.x - _playerTransform.position.x > 0;
    }
  }

  private void HandlePreDeath(object sender, EventArgs e) {
    StartCoroutine(FadeOutDeath());
    if (!disableRegularDrops) FindObjectOfType<CoinDrop>().DropCoin(transform.position);
  }

  public IEnumerator FadeOutDeath() {
    isDead = true;
    GetComponent<Collider2D>().enabled = false;
    
    Quaternion newRotation = Quaternion.Euler(0, 0, 90);
    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    var newPos = transform.position;
    transform.position = new Vector3(newPos.x, newPos.y, ZcoordinateConsts.Interactable);
    sr.sortingLayerName = "Object";

    while (sr.color.a > 0 || sr != null) {
      if (!disableDeathRotation) transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 7.5f);
      var color = sr.color;
      float newAlpha = color.a -= 0.001f;
      sr.color = new Color(color.r, color.g, color.b, newAlpha);
      yield return null;
    }
  }

  private void HandleDeath(object sender, EventArgs e) {
    if (spawnedByEggna) {
      EggnaEnemySpawner spawner = GameObject.Find("LadyEggna")?.GetComponent<EggnaEnemySpawner>();
      spawner.DecrementEnemies();
    }

    GameObject.FindGameObjectWithTag("LevelManager")
      .GetComponent<LevelManager>()
      .GetCurrentStage()
      .RemoveEnemy(this);
  }

  private void InterruptWander() {
    if (isWandering) {
      isWandering = false;
    }
  }


  protected virtual void HandleStatusDamage(object sender, EnemyStatusEventArgs e) {
    List<StatusCondition> statuses = e.statuses;

    foreach (StatusCondition s in statuses) {
      HandleStatus(s);
    }
  }

  private void HandleStatus(StatusCondition status) {
    switch (status) {
      case StatusCondition.Yolked: {
        SoundManager.Instance.PlaySound(Sound.Yolked);
        OnYolked?.Invoke(this, EventArgs.Empty);
        StartCoroutine(Yolked());
        break;
      }
      case StatusCondition.Ignited: {
        SoundManager.Instance.PlaySound(Sound.Ignited, 2.0f);
        OnIgnited?.Invoke(this, EventArgs.Empty);
        break;
      }
      case StatusCondition.Frosted: {
        SoundManager.Instance.PlaySound(Sound.Frosted);
        OnFrosted?.Invoke(this, EventArgs.Empty);
        StartCoroutine(Frosted());
        break;
      }
      case StatusCondition.Electrocuted: {
        SoundManager.Instance.PlaySound(Sound.Electrocuted, volumeScaling: 2.5f);
        OnElectrocuted?.Invoke(this, EventArgs.Empty);
        break;
      }
      case StatusCondition.Bleeding: {
        OnBleed?.Invoke(this, EventArgs.Empty);
        break;
      }
      case StatusCondition.Weakened: {
        OnWeakened?.Invoke(this, EventArgs.Empty);
        break;
      }
      default: {
        Debug.Log("Unknown status condition");
        break;
      }
    }
  }

  protected virtual IEnumerator Yolked() {
    _currentSpeed = _currentSpeed * StatusConfig.YolkedSpeedModifier;

    yield return new WaitForSeconds(StatusConfig.YolkedDuration);

    _currentSpeed = _currentSpeed / StatusConfig.YolkedSpeedModifier;
  }

  protected virtual IEnumerator Frosted() {
    _currentSpeed = _currentSpeed * StatusConfig.FrostSpeedMultiplier;

    yield return new WaitForSeconds(StatusConfig.FrostDuration);

    _currentSpeed = _currentSpeed / StatusConfig.FrostSpeedMultiplier;
  }
  
  protected virtual IEnumerator Electrocute() {
    isStunned = true;

    StopMoving();

    yield return new WaitForSeconds(StatusConfig.ElectrocuteStunDuration);
    isStunned = false;
  }

  public void Move() {
    _eMovement.MoveToPlayer(_currentSpeed);
  }

  public void Flee() {
    _eMovement.Flee(_currentSpeed);
  }

  public void StopMoving() {
    if (rb != null) rb.velocity = Vector2.zero;
  }

  public void Wander() {
    if (isWandering) return;
    StartCoroutine(WanderIEnumerator());
  }

  private IEnumerator WanderIEnumerator() {
    isWandering = true;
    var position = transform.position;
    float newLocationX = position.x + Random.Range(-2f, 2f);
    float newLocationY = position.y + Random.Range(-2f, 2f);
    
    transform.position = new Vector3(position.x, position.y, ZcoordinateConsts.Character);
    _wanderDestination = new Vector3(newLocationX, newLocationY, ZcoordinateConsts.Character);

    while (Vector2.Distance(transform.position, _wanderDestination) > 0.2 && !isStunned) {
      if (isStunned || !isWandering) yield break;
      transform.position = Vector3.MoveTowards(transform.position, _wanderDestination, _maxSpeed/4*Time.deltaTime);
      
      SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
      spriteRenderer.flipX = transform.position.x - _wanderDestination.x > 0;
      yield return null;
    }

    isWandering = false;
  }

  public float GetDistanceToPlayer() {
    return Vector2.Distance(transform.position, _playerTransform.position);
  }
  
  public virtual bool GetIsAttackReady() {
    return GetDistanceToPlayer() < maxDistanceToAttack && isAttackOffCooldown && !isInAttackAnimation;
  }

  public bool GetIsInAlertRange() {
    return GetDistanceToPlayer() < alertRange;
  }

  public void SetAlertTrigger() {
    alertAnimator?.Play("Active", 0, 0f);
  }

  protected virtual void OnCollisionEnter2D(Collision2D other) {
    if (!isWallCollisionOn && !isWandering && other.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle")) {
      Physics2D.IgnoreCollision(other.collider, GetComponent<Collider2D>());
    }
  }

  private void OnTriggerExit2D(Collider2D other) {
    // if (other.gameObject.layer == LayerMask.NameToLayer("Floor")) {
    //   StartCoroutine(FadeOutDeath());
    //   GameObject.FindGameObjectWithTag("LevelManager")
    //     .GetComponent<LevelManager>()
    //     .GetCurrentStage()
    //     .RemoveEnemy(this);
    // }
  }

  public virtual void Attack() {
    StartCoroutine(AttackPlayer());
  }

  protected abstract IEnumerator AttackPlayer();

  private void OnDestroy() {
    Health.OnPreDeath -= HandlePreDeath;
    Health.OnDeath -= HandleDeath;
  }
}
