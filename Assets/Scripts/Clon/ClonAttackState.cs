using UnityEngine;
using UnityEngine.AI;

namespace Main
{
    public class ClonAttackState : ClonBaseState
    {
        private Animator _animator;
        private Weapon _weapon;
        private Enemy _enemy;
        private NavMeshAgent _navMesh;

        private float _fireRate;
        private float _timer = 0;


        private Vector3 _direction;

        public ClonAttackState(Animator animator, Enemy enemy, Weapon weapon, NavMeshAgent navMeshAgent, PlayerController playerController)
        {
            _fireRate = playerController.PlayerStats.FireRate;
            _animator = animator;
            _enemy = enemy;
            _weapon = weapon;
            _navMesh = navMeshAgent;
        }

        public override void EntryState(PlayerClon playerClon)
        {
            _navMesh.velocity = Vector3.zero;
            _navMesh.speed = 0;


            _animator.SetBool(Animations.Idle, false);
            _animator.SetBool(Animations.Run, false);
            _animator.SetTrigger(Animations.Attack);
        }

        public override void UpdateState(PlayerClon playerClon)
        {
            _direction = _enemy.transform.position - playerClon.transform.position;
            playerClon.transform.rotation = Quaternion.LookRotation(_direction.normalized);

            _timer += Time.deltaTime;
            if (_timer > _fireRate)
            {
                _timer = 0;
                _weapon.Attack(_enemy.transform);
            }
        }
    }
}


