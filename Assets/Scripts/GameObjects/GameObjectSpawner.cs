using System.Collections.Generic;
using ECS;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameObjects
{

    public class GameObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _cubePrefab;
        [SerializeField] private bool _isActive = false;
        
        private int _size = 100;
        private bool _isDirty = true;
        private List<GameObject> _spawnedCubes = new List<GameObject>();

        private void Update()
        {
            if (!_isActive) return;

            if (SpawnerCommon.TryHandleSizeInput(ref _size))
            {
                _isDirty = true;
            }

            if (_isDirty)
            {
                RespawnCubes();
                _isDirty = false;
            }
        }
        
        private void RespawnCubes()
        {
            ClearCubes();
            SpawnCubes();
            SpawnerEventChannel.OnSpawn?.Invoke(_size * _size);
        }
        
        private void ClearCubes()
        {
            foreach (var cube in _spawnedCubes)
            {
                if (cube)
                {
                    Destroy(cube);
                }
            }
            _spawnedCubes.Clear();
        }
        
        private void SpawnCubes()
        {
            if (!_cubePrefab)
            {
                Debug.LogError("GameObjectSpawner: Cube prefab not assigned!");
                return;
            }
            
            _spawnedCubes.Capacity = _size * _size;

            for (int x = 0; x < _size; x++)
            {
                for (int z = 0; z < _size; z++)
                {
                    Vector3 position = SpawnerCommon.GridPosition(x, z, _size);

                    GameObject cube = Instantiate(_cubePrefab, position, Quaternion.identity);
                    SceneManager.MoveGameObjectToScene(cube, gameObject.scene);

                    var mover = cube.GetComponent<CubeMover>();
                    if (mover == null)
                    {
                        mover = cube.AddComponent<CubeMover>();
                    }

                    mover.Initialize(SpawnerCommon.ComputeRipplePhase(x, z, _size));

                    _spawnedCubes.Add(cube);
                }
            }
        }
        
        private void OnDestroy()
        {
            ClearCubes();
        }
    }
}
