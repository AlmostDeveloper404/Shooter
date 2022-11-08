using UnityEngine;
using System;

namespace Main
{
    public class PlayerStayingState : PlayerBaseState
    {
        private FloatingJoystick _joystick;
        private LayerMask _detectionMask;
        private float _detectionRadius;
        private Animator _animator;
        private Rigidbody _rigidBody;
        private Weapon _weapon;
        private LayerMask _rayMask;

        private PlayerAttackState _playerAttackState;

        public PlayerStayingState(FloatingJoystick floatingJoystick, LayerMask detectionMask, LayerMask rayMask, float detectionRadius, Animator animator, Rigidbody rigidbody, Weapon activeWeapon)
        {
            _joystick = floatingJoystick;
            _detectionMask = detectionMask;
            _detectionRadius = detectionRadius;
            _rayMask = rayMask;
            _animator = animator;
            _rigidBody = rigidbody;
            _weapon = activeWeapon;
        }

        public override void EntryState(PlayerController playerController)
        {
            _animator.SetBool(Animations.Idle, true);
            _animator.SetBool(Animations.Run, false);
            _animator.SetBool(Animations.Attack, false);
        }
        public override void Update(PlayerController playerController)
        {
            if (_joystick.Vertical != 0 || _joystick.Horizontal != 0)
            {
                playerController.ChangeState(playerController.PlayerRunningState);
                return;
            }
            TryShooting(playerController);
        }

        private void TryShooting(PlayerController playerController)
        {
            Collider[] _allDetectedColliders = Physics.OverlapSphere(playerController.transform.position, _detectionRadius, _detectionMask);
            if (_allDetectedColliders.Length == 0) return;


            float minDistance = Mathf.Infinity;
            Enemy nearestEnemy = null;
            for (int i = 0; i < _allDetectedColliders.Length; i++)
            {
                if (!HasDirectView(_allDetectedColliders[i], playerController)) continue;

                Enemy enemy = _allDetectedColliders[i].GetComponent<Enemy>();
                if (enemy.IsDead) continue;


                float distance = Vector3.Distance(playerController.transform.position, _allDetectedColliders[i].transform.position);
                if (distance < minDistance)
                {
                    nearestEnemy = _allDetectedColliders[i].GetComponent<Enemy>();
                    minDistance = distance;
                }
            }

            if (nearestEnemy)
            {
                Enemy targetEnemy = nearestEnemy;
                playerController.OnEnemyDetected?.Invoke(targetEnemy);
                _playerAttackState = new PlayerAttackState(targetEnemy, _animator, _rigidBody, _joystick, _detectionRadius, _weapon);
                playerController.ChangeState(_playerAttackState);
            }
        }

        private bool HasDirectView(Collider collider, PlayerController playerController)
        {
            Vector3 direction = collider.transform.position - playerController.transform.position;
            Ray ray = new Ray(playerController.transform.position + Vector3.up, direction);

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, _rayMask))
            {
                if (hitInfo.collider.GetComponent<Enemy>())
                {
                    return true;
                }
            }
            return false;
        }

        public override void FixedUpdate(PlayerController playerController)
        {
            _rigidBody.velocity = Vector3.zero;
        }
        public override void OnTriggerEnter(PlayerController playerController)
        {

        }

    }
}


