using Stage;
using System;
using UnityEngine;

public class EggGuardWeapons : MonoBehaviour {
  [SerializeField] private GameObject _spear;
  private GameObject _currentSpear;

  private Animator _anim;

  private GameObject _playerObject;

  [SerializeField] private GameObject _heldSpear;

  private float _attackSpeed = 1;

  private void Awake() {
    _anim = gameObject.GetComponent<Animator>();

    _playerObject = GameObject.FindGameObjectWithTag("Player");

    EggGuardAttack[] attackBehaviours = _anim.GetBehaviours<EggGuardAttack>();

    int level = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().level;
    if (level > 1) {
      _attackSpeed = 1.25f;
    }
    if (level > 2) {
      _attackSpeed = 1.5f;
    }

    if (attackBehaviours.Length > 0) {
      EggGuardAttack attackBehaviour = attackBehaviours[0];

      attackBehaviour.OnEggGuardAttackBegin += HandleOnAttackBegin;
      attackBehaviour.OnEggGuardAttackEnd += HandleOnAttackEnd;
    }
  }

  private void HandleOnAttackBegin(object sender, EventArgs e) {
    if (_currentSpear != null) {
      return;
    }

    HideHeldSpear();

    _currentSpear = Instantiate(_spear, transform.position, Quaternion.identity);
    _currentSpear.transform.SetParent(gameObject.transform);

    EggGuardSpear spear = _currentSpear.GetComponent<EggGuardSpear>();
    spear.SetSpeed(_attackSpeed);
    spear.RotateToPlayer();
  }

  private void HandleOnAttackEnd(object sender, EventArgs e) {
    if (_currentSpear != null) {
      ShowHeldSpear();

      Destroy(_currentSpear);
    }
  }

  private void ShowHeldSpear() {
    if (_heldSpear != null) {
      _heldSpear.SetActive(true);
    }
  }

  private void HideHeldSpear() {
    if (_heldSpear != null) {
      _heldSpear.SetActive(false);
    }
  }
}
