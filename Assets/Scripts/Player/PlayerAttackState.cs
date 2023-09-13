using UnityEngine;

namespace Main
{
    public class PlayerAttackState : PlayerBaseState
    {
        private Enemy _targetEnemy;
        private Animator _animator;
        private Rigidbody _rigidBody;
        private FloatingJoystick _joystick;
        private float _detectionRadius;
        private Weapon _weapon;

        private Vector3 _shootingDirection;

        private float _timer;
        private Camera _camera;

        public PlayerAttackState(Enemy enemy, Animator animator, Rigidbody rigidbody, FloatingJoystick floatingJoystick, float detectionRadius, Weapon weapon, Camera camera)
        {
            _targetEnemy = enemy;
            _animator = animator;
            _rigidBody = rigidbody;
            _joystick = floatingJoystick;
            _detectionRadius = detectionRadius;
            _weapon = weapon;
            _camera = camera;
        }

        public override void EntryState(PlayerController playerController)
        {
            _timer = _weapon.FireRate * 0.7f;
            _animator.SetBool(Animations.Attack, true);
            _animator.SetBool(Animations.Run, false);
            _animator.SetBool(Animations.Idle, false);
        }
        public override void Update(PlayerController playerController)
        {
            if (_targetEnemy.IsDead)
            {
                _targetEnemy = null;
                playerController.ChangeState(playerController.PlayerStayingState);
                return;
            }

            if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
            {
                playerController.ChangeState(playerController.PlayerRunningState);
                return;
            }

            float distance = Vector3.Distance(playerController.transform.position, _targetEnemy.transform.position);
            if (distance > _detectionRadius + 1f)
            {
                playerController.ChangeState(playerController.PlayerStayingState);
                return;
            }

            _timer += Time.deltaTime;
            if (_timer > _weapon.FireRate)
            {
                _timer = 0;
                Shoot();
            }
        }
        public override void FixedUpdate(PlayerController playerController)
        {
            _shootingDirection = _targetEnemy.transform.position - playerController.transform.position;
            _rigidBody.MoveRotation(Quaternion.LookRotation(_shootingDirection * 2f));
            _rigidBody.velocity = Vector3.zero;
        }

        private void Shoot()
        {
            _weapon.Attack(_targetEnemy.transform);
        }
    }
}

