using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Main
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private List<Enemy> _allRoomEnemies = new List<Enemy>();

        [SerializeField] private Material _roomFloorClearedMat;
        [SerializeField] private Material _roomFloorUncleared;
        [SerializeField] private Material _roomWallCleared;
        [SerializeField] private Material _roomWallUncleared;

        [SerializeField] private MeshRenderer _floorRenderer;
        [SerializeField] private MeshRenderer _wallRenderer;

        [SerializeField] private GameObject[] _goToActivate;


        private void Start()
        {
            SetupRoom();
        }



        public void RemoveEnemy(Enemy enemy)
        {
            _allRoomEnemies.Remove(enemy);
            if (!HasEnemyInRoom()) RoomCompleted();
        }

        private void RoomCompleted()
        {
            SetupRoom();
            SpawnLoot();
        }

        private void SetupRoom()
        {

            bool enemiesInRoom = HasEnemyInRoom();

            _floorRenderer.material = enemiesInRoom ? _roomFloorUncleared : _roomFloorClearedMat;
            _wallRenderer.material = enemiesInRoom ? _roomWallUncleared : _roomWallCleared;
        }

        private void SpawnLoot()
        {
            for (int i = 0; i < _goToActivate.Length; i++)
            {
                _goToActivate[i].SetActive(true);
            }
        }

        private bool HasEnemyInRoom()
        {
            return _allRoomEnemies.Count == 0 ? false : true;
        }
    }
}

