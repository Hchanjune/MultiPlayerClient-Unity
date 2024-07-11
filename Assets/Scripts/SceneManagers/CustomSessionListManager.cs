using System;
using System.Collections.Generic;
using Colyseus.Schema;
using NetworkManagers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils.Extensions;
using Zenject;

namespace SceneManagers
{
    public class CustomSessionListManager : MonoBehaviour
    {
        // UI
        public Button CreateSessionBtn;

        public Button BackToLobbyBtn;

        public TMP_Text CurrentChatRoomCountText;
        
        public Transform ChatRoomListScrollContent;
        public GameObject ChatRoomItemPrefab;

        // Create Option Modal
        public GameObject RoomOptionModal;
        public Button CancelCreateRoomBtn;
        public Button CancelCreateRoomIconBtn;
        public Button SubmitCreateRoomBtn;
        public TMP_InputField OptionRoomName;
        public TMP_Dropdown OptionMaxClient;
        public Toggle OptionIsPrivate;
        public GameObject OptionPasswordArea;
        public TMP_InputField OptionPassword;
        
        
        private NetworkManager _networkManager;
        [Inject]
        public void Constructor(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }
    
        private void Awake()
        {
            // Back To Lobby EventListener
            BackToLobbyBtn.onClick.AddListener(OnBackToLobbyBtnClicked);
            // Create Session Btn
            CreateSessionBtn.onClick.AddListener(OpenRoomOptionModal);
            
            // Option Modal Events
            CloseRoomOptionModal();
            OptionPasswordArea.SetActive(false);
            OptionIsPrivate.onValueChanged.AddListener(delegate
            {
                OnOptionIsPrivateValueChanged(OptionIsPrivate);
            });
            SubmitCreateRoomBtn.onClick.AddListener(OnCreateRoomSubmit);
            CancelCreateRoomBtn.onClick.AddListener(CloseRoomOptionModal);
            CancelCreateRoomIconBtn.onClick.AddListener(CloseRoomOptionModal);
            
            
        }

        private void Start()
        {
            // Register Network Event
            RegisterNetworkEvent();
        }

        private void OnDestroy()
        {
            _networkManager.LobbyNetwork.Lobby.OnStateChange -= OnCustomSessionListStateChange;
        }

        
        private void RegisterNetworkEvent()
        {
            _networkManager.LobbyNetwork.Lobby.OnStateChange += OnCustomSessionListStateChange;

            // RegisterNetworkEvent 후 즉시 상태 갱신
            OnCustomSessionListStateChange(_networkManager.LobbyNetwork.Lobby.State, true);
        }

        private void OnCustomSessionListStateChange(LobbyState state, bool isFirstState)
        {
            Debug.Log($"OnStateChange called. Current scene: {SceneManager.GetActiveScene().name}, Manager scene: {gameObject.scene.name}");
            
            if (SceneManager.GetActiveScene().name.Equals(gameObject.scene.name))
            {
                Debug.Log("OnCustomSessionListStateChange");
                OnChatRoomCountChange(state.chatRooms.Count);
                OnChatRoomListChange(state.chatRooms);                
            }
        }
        

        private void OnBackToLobbyBtnClicked()
        {
            SceneManager.LoadScene("Lobby");
        }

        private void OpenRoomOptionModal()
        {
            RoomOptionModal.SetActive(true);
            SetInteractable(false);
        }

        private void CloseRoomOptionModal()
        {
            RoomOptionModal.SetActive(false);
            SetInteractable(true);
        }

        private void OnOptionIsPrivateValueChanged(Toggle toggle)
        {
            OptionPasswordArea.SetActive(toggle.isOn);
        }

        private void OnCreateRoomSubmit()
        {
            Dictionary<string, object> options = new Dictionary<string, object>()
            {
                ["roomName"] = OptionRoomName.text,
                ["initialOwner"] = _networkManager?.ClientInfo?.id?? "DEBUG",
                ["maxClients"] = OptionMaxClient.value + 1,
                ["isPrivate"] = OptionIsPrivate.isOn,
                ["password"] = OptionIsPrivate.isOn ? OptionPassword.text : ""
            };

            if (_networkManager != null) _networkManager.LobbyNetwork.CreateChatRoomRequest(options);
        }


        private void OnRoomItemClicked(ChatRoomInfo chatRoomInfo)
        {
            Dictionary<string, object> options = new Dictionary<string, object>()
            {
                ["roomId"] = chatRoomInfo.roomId
            };
            _networkManager.LobbyNetwork.JoinChatRoomRequest(options);
        }


        private void OnChatRoomCountChange(int count)
        {
            CurrentChatRoomCountText.text = $"{count}";
        }

        private void OnChatRoomListChange(MapSchema<ChatRoomInfo> chatRooms)
        {
            foreach (Transform child in ChatRoomListScrollContent)
            {
                Destroy(child.gameObject);
            }
            
            foreach (ChatRoomInfo chatRoom in chatRooms.Values)
            {
                // Prefab 인스턴스 생성
                GameObject chatRoomItem = Instantiate(ChatRoomItemPrefab, ChatRoomListScrollContent);

                // Button 컴포넌트 가져오기
                Button chatRoomButton = chatRoomItem.GetComponent<Button>();

                // TMP_Text 컴포넌트들 가져오기
                TMP_Text chatRoomIsPrivate = chatRoomItem.transform.Find("Text-RoomIsPrivate").GetComponent<TMP_Text>();
                TMP_Text chatRoomId = chatRoomItem.transform.Find("Text-RoomId").GetComponent<TMP_Text>();
                TMP_Text chatRoomName = chatRoomItem.transform.Find("Text-RoomName").GetComponent<TMP_Text>();
                TMP_Text chatRoomOwner = chatRoomItem.transform.Find("Text-RoomOwner").GetComponent<TMP_Text>();
                TMP_Text chatRoomCurrentClient = chatRoomItem.transform.Find("Text-CurrentClient").GetComponent<TMP_Text>();

                // TMP_Text 컴포넌트들에 데이터 설정
                chatRoomIsPrivate.text = chatRoom.isPrivate ? "Private" : "Public";
                chatRoomId.text = chatRoom.roomId.ToString();
                chatRoomName.text = chatRoom.roomName;
                chatRoomOwner.text = chatRoom.roomOwner;
                chatRoomCurrentClient.text = $"{chatRoom.players.Count}/{chatRoom.maxClients}";

                // Button 클릭 이벤트 추가
                chatRoomButton.onClick.AddListener(() =>
                {
                    Debug.Log($"Button for room {chatRoom.roomId} clicked");
                    OnRoomItemClicked(chatRoom);
                });
            }
        }

        private void SetInteractable(bool interactable)
        {
            // Canvas 하위의 모든 UI 요소들을 가져옵니다.
            Canvas canvas = FindObjectOfType<Canvas>();
            GraphicRaycaster[] raycasters = canvas.GetComponentsInChildren<GraphicRaycaster>();

            foreach (var raycaster in raycasters)
            {
                raycaster.enabled = interactable;
            }

            // Modal 창은 항상 상호작용 가능하게 유지
            RoomOptionModal.GetComponent<GraphicRaycaster>().enabled = true;
        }
        
    
    }
}
