using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDisplayPoint : MonoBehaviour {
    private Transform _player;
    private const float LeftX = -0.38f;
    private const float RightX = 0.38f;
    private const float Y = -0.55f;
    private bool _enableFlip;

    private void Awake() {
        _enableFlip = true;
        PlayerHealth.OnGameOver += DeathDisableFlip;
        PlayerHealth.OnReviveGameOver += DeathDisableFlip;
        PlayerDeath.OnReviveSequenceDone += DeathEnableFlip;
    }

    private void FixedUpdate() {
        _player = GameObject.Find("Player").transform;
        Vector3 playerPos = _player.position;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (!_enableFlip) return;
        if (mousePos.x < _player.position.x) {
            transform.position = new Vector3(LeftX + playerPos.x, Y + playerPos.y, ZcoordinateConsts.WeaponAttack);
        }
        else {
            transform.position = new Vector3(RightX + playerPos.x, Y + playerPos.y, ZcoordinateConsts.WeaponAttack);
        }
    }

    private void DeathDisableFlip(object sender, EventArgs e) {
        _enableFlip = false;
    }
    
    private void DeathEnableFlip(object sender, EventArgs e) {
        _enableFlip = true;
    }

    private void OnDestroy() {
        PlayerHealth.OnGameOver -= DeathDisableFlip;
        PlayerHealth.OnReviveGameOver -= DeathDisableFlip;
        PlayerDeath.OnReviveSequenceDone -= DeathEnableFlip;
    }
}
