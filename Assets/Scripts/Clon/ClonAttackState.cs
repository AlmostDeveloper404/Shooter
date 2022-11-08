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
        private LayerMask _enemyMask;

        public ClonAttackState(Animator animator, Enemy enemy, Weapon weapon, NavMeshAgent navMeshAgent, PlayerController playerController, LayerMask enemyMask)
        {
            _animator = animator;
            _enemy = enemy;
            _weapon = weapon;
            _navMesh = navMeshAgent;
            _fireRate = weapon.FireRate;
            _enemyMask = enemyMask;
        }

        public override void EntryState(PlayerClon playerClon)
        {
            _navMesh.velocity = Vector3.zero;
            _navMesh.speed = 0;


            _animator.SetBool(Animations.Idle, false);
            _animator.SetBool(Animations.Run, false);
            _animator.SetBool(Animations.Attack, true);
        }

        public override void UpdateState(PlayerClon playerClon)
        {
            if (!HasDirectView<Enemy>.HasView(playerClon.transform.position, _enemy.transform.position, _enemyMask))
            {
                
            }


            _direction = _enemy.transform.position - playerClon.transform.position;
            playerClon.transform.rotation = Quaternion.LookRotation(_direction.normalized);

            if (_enemy.IsDead)
            {
                _animator.SetBool(Animations.Attack, false);
                _animator.SetBool(Animations.Idle, true);
                playerClon.ChangeClonState(playerClon.ClonEscortState);
            }

            _timer += Time.deltaTime;

            if (_timer > _fireRate)
            {
                _timer = 0;
                _weapon.Attack(_enemy.transform);
            }
        }
    }
}


