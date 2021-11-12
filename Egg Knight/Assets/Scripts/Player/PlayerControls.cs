using Stage;
using System;
using Fungus;
using UnityEngine;
using EventHandler = System.EventHandler;

public class PlayerControls : MonoBehaviour {
	public static event EventHandler<MovementVectorEventArgs> OnMovement;
	public static event EventHandler OnMovementKeysPressed;
	public static event EventHandler OnRightClick;

	public static event EventHandler OnSpaceBarPressed;
	public static event EventHandler OnLeftClick;
	public static event EventHandler OnQPress;
	public static event EventHandler OnEPress;

	public static event EventHandler On1Press;
	public static event EventHandler On2Press;
	public static event EventHandler On3Press;

	private bool _notRolling = true;
	private bool _dialogueDisabled = true;
	private bool _gameRunning = true;

	private bool _weaponSwitchingEnabled = true;
	private bool _forkEnabled = false;
	private bool _spoonEnabled = false;

	private bool _movementKeysDown = false;

	private void Awake() {
		PlayerMovement.OnRollBegin += HandleRollBegin;
		PlayerMovement.OnRollEnd += HandleRollEnd;

		PlayerWeapons.OnWeaponAnimationBegin += HandleWeaponAnimBegin;
		BasePlayerWeapon.OnWeaponAnimationEnd += HandleWeaponAnimEnd;

		LevelManager.OnDialogueStart += HandleDialogueBegin;
		LevelManager.OnDialogueEnd += HandleDialogueEnd;

		PauseScreen.OnGamePaused += HandleGamePaused;
		PauseScreen.OnGameResumed += HandleGameResumed;

		UnlockWeaponItem.OnPickup += UnlockWeapon;
		
	}

	private void Update() {
		if (_dialogueDisabled && _gameRunning) {
			AttackControls();
			ShootControls();
		}

		if (ControlsEnabled()) {
			MovementControls();

			RollControls();

			AttackSwitchControls();
			UnlockAllWeapons();
			TeleportToExit();
			KillAllEnemies();
		}
	}

	private void KillAllEnemies() {
		if (Input.GetKey(KeyCode.K)) {
			GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>()
				.GetCurrentStage().KillAllEnemies();
		}
	}

	private void TeleportToExit() {
		if (Input.GetKey(KeyCode.O)) {
			GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("LevelManager")
				.GetComponent<LevelManager>().GetCurrentStage().stageExits[0].transform.position;
		}
	}

	private void UnlockAllWeapons() {
		if (Input.GetKey(KeyCode.L)) {
			_forkEnabled = true;
			_spoonEnabled = true;
		}
	}

	private bool ControlsEnabled() {
		return (_notRolling && _dialogueDisabled && _gameRunning);
	}

	private void MovementControls() {
		if (_movementKeysDown != MovementKeysPressed()) {
			OnMovementKeysPressed?.Invoke(this, EventArgs.Empty);
		}

		_movementKeysDown = MovementKeysPressed();

		Vector2 movementVector = new Vector2(
			0 + (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0),
			0 + (Input.GetKey(KeyCode.S) ? -1 : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0)
		);

		if (movementVector.x != 0 && movementVector.y != 0) {
			movementVector *= 0.8f;
		}

		OnMovement?.Invoke(this, new MovementVectorEventArgs(movementVector));
	}

	private bool MovementKeysPressed() {
		return (
			Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)
		);
	}

	private void RollControls() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			OnSpaceBarPressed?.Invoke(this, EventArgs.Empty);
		}
	}

	private void ShootControls() {
		if (Input.GetMouseButton((int)MouseInput.RightClick)) {
			OnRightClick?.Invoke(this, EventArgs.Empty);
		}
	}

	private void AttackControls() {
		if (Input.GetMouseButtonDown((int)MouseInput.LeftClick)) {
			OnLeftClick?.Invoke(this, EventArgs.Empty);
		}
	}

	private void AttackSwitchControls() {
		if (_weaponSwitchingEnabled == false) {
			return;
		}
		
		if (Input.GetKeyDown(KeyCode.Alpha1)) On1Press?.Invoke(this, EventArgs.Empty);
		if (Input.GetKeyDown(KeyCode.Alpha2) && _forkEnabled) On2Press?.Invoke(this, EventArgs.Empty);
		if (Input.GetKeyDown(KeyCode.Alpha3) && _spoonEnabled) On3Press?.Invoke(this, EventArgs.Empty);

		// if (Input.GetKeyDown(KeyCode.Q)) {
		// 	OnQPress?.Invoke(this, EventArgs.Empty);
		// } else if (Input.GetKeyDown(KeyCode.E)) {
		// 	OnEPress?.Invoke(this, EventArgs.Empty);
		// }
	}

	private void UnlockWeapon(object sender, EventArgs e) {
		if (!_forkEnabled) {
			_forkEnabled = true;
		} else if (!_spoonEnabled) {
			_spoonEnabled = true;
		}
	}

	private void HandleRollBegin(object sender, RollEventArgs e) {
		_notRolling = false;
	}

	private void HandleRollEnd(object sender, EventArgs e) {
		_notRolling = true;
	}

	private void HandleWeaponAnimBegin(object sender, EventArgs e) {
		_weaponSwitchingEnabled = false;
	}

	private void HandleWeaponAnimEnd(object sender, EventArgs e) {
		_weaponSwitchingEnabled = true;
	}

	private void HandleDialogueBegin(object sender, EventArgs e) {
		_dialogueDisabled = false;
	}

	private void HandleDialogueEnd(object sender, EventArgs e) {
		_dialogueDisabled = true;
	}

	private void HandleGamePaused(object sender, EventArgs e) {
		_gameRunning = false;
	}
	private void HandleGameResumed(object sender, EventArgs e) {
		_gameRunning = true;
	}

	private void OnDestroy() {
		PlayerMovement.OnRollBegin -= HandleRollBegin;
		PlayerMovement.OnRollEnd -= HandleRollEnd;

		PlayerWeapons.OnWeaponAnimationBegin -= HandleWeaponAnimBegin;
		BasePlayerWeapon.OnWeaponAnimationEnd -= HandleWeaponAnimEnd;

		LevelManager.OnDialogueStart -= HandleDialogueBegin;
		LevelManager.OnDialogueEnd -= HandleDialogueEnd;

		PauseScreen.OnGamePaused -= HandleGamePaused;
		PauseScreen.OnGameResumed -= HandleGameResumed;
	}
}
