using System.Collections.Generic;
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
            SpawnKey();
        }

        private void SetupRoom()
        {
            bool enemiesInRoom = HasEnemyInRoom();

            _floorRenderer.material = enemiesInRoom ? _roomFloorUncleared : _roomFloorClearedMat;
            _wallRenderer.material = enemiesInRoom ? _roomWallUncleared : _roomWallCleared;
        }

        private void SpawnKey()
        {

        }

        private bool HasEnemyInRoom()
        {
            return _allRoomEnemies.Count == 0 ? false : true;
        }
    }
}

