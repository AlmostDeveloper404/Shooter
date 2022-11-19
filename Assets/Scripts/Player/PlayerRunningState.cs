using UnityEngine;

namespace Main
{
    public class PlayerRunningState : PlayerBaseState
    {
        private FloatingJoystick _floatingJoystick;
        private Animator _animator;
        private float _speed;
        private float _rotationSpeed;
        private Rigidbody _rigidBody;

        public PlayerRunningState(FloatingJoystick floatingJoystick, Animator animator, float speed, float rotationSpeed, Rigidbody rigidbody)
        {
            _floatingJoystick = floatingJoystick;
            _animator = animator;
            _speed = speed;
            _rotationSpeed = rotationSpeed;
            _rigidBody = rigidbody;

        }

        public override void EntryState(PlayerController playerController)
        {
            _animator.SetBool(Animations.Run, true);
            _animator.SetBool(Animations.Idle, false);
            _animator.SetBool(Animations.Attack, false);
        }
        public override void Update(PlayerController playerController)
        {
            if (_floatingJoystick.Horizontal == 0 && _floatingJoystick.Vertical == 0)
            {
                playerController.ChangeState(playerController.PlayerStayingState);
            }
        }

        public override void FixedUpdate(PlayerController playerController)
        {
            Vector3 direction = new Vector3(_floatingJoystick.Horizontal, 0f, _floatingJoystick.Vertical).normalized;
            _rigidBody.velocity = new Vector3(direction.x * _speed, _rigidBody.velocity.y, direction.z * _speed);
            _rigidBody.MoveRotation(Quaternion.LookRotation(new Vector3(_rigidBody.velocity.x * _rotationSpeed, 0f, _rigidBody.velocity.z * _rotationSpeed)));
        }
    }

}


