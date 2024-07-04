using System;
using NetworkManagers;
using NetworkManagers.Lobby;
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
        public Button LogoutBtn;

        public TMP_Text CurrentUserCount;

        private void Awake()
        {
            LogoutBtn.onClick.AddListener(OnLogout);
            RegisterNetworkEvent();
        }

        void Start()
        {
            
        }
        
        private void RegisterNetworkEvent()
        {
            _networkManager.LobbyNetwork.Lobby.OnStateChange += OnStateChange;
        }

        private void OnStateChange(LobbyState state, bool isFirstState)
        {
            CurrentUserCount.text = $"현재 접속중인 사용자 : {state.clients.Count}";
        }


        private void OnLogout()
        {
            _networkManager.Disconnect();
        }
        
    }
}
