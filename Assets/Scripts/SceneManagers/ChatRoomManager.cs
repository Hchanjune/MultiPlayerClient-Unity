using System;
using NetworkManagers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace SceneManagers
{
    public class ChatRoomManager: MonoBehaviour
    {
        // UI
        public TMP_Text roomTitleText;
        public Button ExitBtn;
        

        private NetworkManager _networkManager;
        [Inject]
        public void Constructor(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        private void Awake()
        {
            ExitBtn.onClick.AddListener(OnExitBtnClicked);
        }

        private void Start()
        {
            RegisterNetworkEvent();
        }

        private void OnDestroy()
        {
            _networkManager.ChatRoomNetwork.ChatRoom.OnStateChange -= OnChatRoomStateChange;
        }

        private void RegisterNetworkEvent()
        {
            _networkManager.ChatRoomNetwork.ChatRoom.OnStateChange += OnChatRoomStateChange;

            // RegisterNetworkEvent 후 즉시 상태 갱신
            OnChatRoomStateChange(_networkManager.ChatRoomNetwork.ChatRoom.State, true);
        }

        private void OnChatRoomStateChange(ChatRoomState state, bool isFirstState)
        {
            if (SceneManager.GetActiveScene().name.Equals(gameObject.scene.name))
            {
                OnRoomTitleChange(state);       
            }
        }

        private void OnRoomTitleChange(ChatRoomState state)
        {
            roomTitleText.text = $"{(state.isPrivate ? "[비공개]" : "[공개]")} - {state.roomName}";
        }


        private void OnExitBtnClicked()
        {
            _networkManager.ChatRoomNetwork.ChatRoom.Leave();
            SceneManager.LoadScene("CustomSessionList");
        }
        
        
        
        
    }
}