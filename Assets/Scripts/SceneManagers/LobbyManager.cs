using NetworkManagers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace SceneManagers
{
    public class LobbyManager : MonoBehaviour
    {
        private NetworkManager _networkManager;

        [Inject]
        public void Constructor(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        // UI
        public Button CustomSessionListBtn;
        public Button LogoutBtn;
        public TMP_Text CurrentUserCount;

        private void Awake()
        {
            if (CustomSessionListBtn != null)
                CustomSessionListBtn.onClick.AddListener(OnCustomSessionListBtnClicked);
            else
                Debug.LogError("CustomSessionListBtn is not assigned.");

            if (LogoutBtn != null)
                LogoutBtn.onClick.AddListener(OnLogout);
            else
                Debug.LogError("LogoutBtn is not assigned.");
        }

        void Start()
        {
            // RegisterNetworkEvent
            RegisterNetworkEvent();
        }
        
        private void OnDestroy()
        {
            _networkManager.LobbyNetwork.Lobby.OnStateChange -= OnLobbyStateChange;
        }

        private void RegisterNetworkEvent()
        {
            _networkManager.LobbyNetwork.Lobby.OnStateChange += OnLobbyStateChange;

            // RegisterNetworkEvent 후 즉시 상태 갱신
            OnLobbyStateChange(_networkManager.LobbyNetwork.Lobby.State, true);
        }

        private void OnLobbyStateChange(LobbyState state, bool isFirstState)
        {
            Debug.Log($"OnStateChange called. Current scene: {SceneManager.GetActiveScene().name}, Manager scene: {gameObject.scene.name}");

            if (SceneManager.GetActiveScene().name.Equals(gameObject.scene.name))
            {
                Debug.Log("OnLobbyStateChange");
                OnCurrentUserCountChange(state.clients.Count);       
            }
        }

        private void OnCurrentUserCountChange(int count)
        {
            CurrentUserCount.text = $"현재 접속중인 사용자 : {count}";
        }

        private void OnLogout()
        {
            if (_networkManager != null)
            {
                _networkManager.Disconnect();
            }
            else
            {
                Debug.LogError("NetworkManager is null.");
            }
        }

        private void OnCustomSessionListBtnClicked()
        {
            SceneManager.LoadScene("CustomSessionList");
        }
    }
}
