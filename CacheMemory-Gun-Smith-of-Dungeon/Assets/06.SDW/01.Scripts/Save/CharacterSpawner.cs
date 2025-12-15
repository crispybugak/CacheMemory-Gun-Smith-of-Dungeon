using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace _06.SDW._01.Scripts.Save
{
    [System.Serializable]
    public class CharacterData
    {
        public string id;
        public GameObject prefab;
    }

    public class CharacterSpawner : MonoSingleton<CharacterSpawner>
    {
        [SerializeField] private GameObject uiManager;
        [SerializeField] private GameObject _zoom;
        [SerializeField] private GameObject _gameUi;
        
        [SerializeField] private CharacterData[] characterDatabase;

        private Dictionary<string, GameObject> _map;
        public Action OnCharacterSpawned;

        private HealthUI _health;
        private StaminaUI _stamina;

        private void Awake()
        {
            _stamina = uiManager.GetComponent<StaminaUI>();
            _health = uiManager.GetComponent<HealthUI>();
            
            _map = new Dictionary<string, GameObject>();
            foreach (var data in characterDatabase)
            {
                _map[data.id] = data.prefab;
                Debug.Log($"CharacterSpawner: {data.id} -> {data.prefab}"); 
            }
        }

        private void Start()
        {
            string selected = PlayerPrefs.GetString("SelectedCharacter", "");
            Debug.Log($"[CharacterSpawner] SelectedCharacter = '{selected}'");
            if (_map.TryGetValue(selected, out var prefab))
            {
                var player = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                var cam = _zoom.GetComponent<CinemachineCamera>();
                cam.Target.TrackingTarget = player.transform;
                player.GetComponent<Stamina>()._staminaUI = _stamina;
                player.GetComponent<Dead>()._zoom = _zoom.GetComponent<CameraZoom>();
                _health.Health = player.GetComponent<Health>();
                _stamina.stamina = player.GetComponent<Stamina>();
                var dead = player.GetComponent<Dead>();
                dead.health = player.GetComponent<Health>();
                var gameui = player.GetComponent<Health>();
                gameui._gameUI = _gameUi.GetComponent<GameUI>();
                OnCharacterSpawned?.Invoke();
            }
            else
            {
                Debug.LogError($"매치되는 프리팹 없음: {selected}");
            }
        }
    }
}