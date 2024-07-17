using System;
using System.Collections.Generic;
using System.Linq;
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
        
        // Enter Modal
        public GameObject RoomEnterModal;
        public TMP_InputField RoomEnterPassword;
        public Button SubmitRoomPassword;
        public Button CancelRoomPassword;
        public Button CancelRoomPasswordIconBtn;
        public TMP_Text RoomEnterModalStatusMessage;
        
        // Deny Modal
        public GameObject RoomDeniedModal;
        public TMP_Text RoomDeniedModalStatusMessage;
        public Button ConfirmRoomDeniedModal;
        public Button ConfirmRoomDeniedModalIcon;
        
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
            
            // Enter Modal Events
            CloseRoomEnterModal();
            // Submit Event On Func
            CancelRoomPassword.onClick.AddListener(CloseRoomEnterModal);
            CancelRoomPasswordIconBtn.onClick.AddListener(CloseRoomEnterModal);
            
            // Denied Modal Events
            CloseDeniedModal();
            ConfirmRoomDeniedModal.onClick.AddListener(CloseDeniedModal);
            ConfirmRoomDeniedModalIcon.onClick.AddListener(CloseDeniedModal);
            
        }

        private void Start()
        {
            // Register Network Event
            RegisterNetworkEvent();
        }

        private void OnDestroy()
        {
            _networkManager.LobbyNetwork.Lobby.OnStateChange -= OnCustomSessionListStateChange;
            _networkManager.LobbyNetwork.OnChatRoomPasswordError -= OnPrivateRoomPasswordError;
            _networkManager.LobbyNetwork.OnChatRoomFull -= OnTargetRoomFull;
        }

        
        private void RegisterNetworkEvent()
        {
            OnCustomSessionListStateChange(_networkManager.LobbyNetwork.Lobby.State, true);
            _networkManager.LobbyNetwork.Lobby.OnStateChange += OnCustomSessionListStateChange;
            _networkManager.LobbyNetwork.OnChatRoomPasswordError += OnPrivateRoomPasswordError;
            _networkManager.LobbyNetwork.OnChatRoomFull += OnTargetRoomFull;
        }

        private void OnCustomSessionListStateChange(LobbyState state, bool isFirstState)
        {
                OnChatRoomCountChange(state.chatRooms.Count);
                OnChatRoomListChange(state.chatRooms);
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
                ["maxClients"] = int.Parse(new string(OptionMaxClient.options[OptionMaxClient.value].text.Where(char.IsDigit).ToArray())),
                ["isPrivate"] = OptionIsPrivate.isOn,
                ["password"] = OptionIsPrivate.isOn ? OptionPassword.text : ""
            };

            if (_networkManager != null) _networkManager.LobbyNetwork.CreateChatRoomRequest(options);
        }
        
        private void OnPublicRoomItemClicked(ChatRoomInfo chatRoomInfo)
        {
            Dictionary<string, object> options = new Dictionary<string, object>()
            {
                ["roomId"] = chatRoomInfo.roomId
            };
            _networkManager.LobbyNetwork.JoinChatRoomRequest(options);
        }

        private void OnPrivateRoomItemClicked(ChatRoomInfo chatRoomInfo)
        {
            if (chatRoomInfo.currentPlayers >= chatRoomInfo.maxClients)
            {
                OnTargetRoomFull("참여가능 인원 수를 초과 하였습니다.");                
            }
            else
            {
                OpenRoomEnterModal(chatRoomInfo);                
            }
        }

        private void OpenRoomEnterModal(ChatRoomInfo chatRoomInfo)
        {
            RoomEnterModal.SetActive(true);
            SetInteractable(false);
            SubmitRoomPassword.onClick.AddListener(()=>{ OnPrivateRoomJoinRequest(chatRoomInfo);});
        }

        private void CloseRoomEnterModal()
        {
            RoomEnterModal.SetActive(false);
            SetInteractable(true);
            RoomEnterPassword.text = "";
            RoomEnterModalStatusMessage.text = "";
            SubmitRoomPassword.onClick.RemoveAllListeners();
        }

        private void OnPrivateRoomJoinRequest(ChatRoomInfo chatRoomInfo)
        {
            Dictionary<string, object> options = new Dictionary<string, object>()
            {
                ["roomId"] = chatRoomInfo.roomId,
                ["password"] = RoomEnterPassword.text
            };
            _networkManager.LobbyNetwork.JoinChatRoomRequest(options);
        }

        private void OnPrivateRoomPasswordError(string message)
        {
            RoomEnterModalStatusMessage.text = message;
        }

        private void OnTargetRoomFull(string message)
        {
            if (RoomEnterModal.activeSelf)
            {
                RoomEnterModalStatusMessage.text = message;
            }
            else
            {
                OpenDeniedModal(message);
            }
            
        }

        private void OpenDeniedModal(string text = "")
        {
            RoomDeniedModal.SetActive(true);
            SetInteractable(false);
            RoomDeniedModalStatusMessage.text = text;
        }

        private void CloseDeniedModal()
        {
            RoomDeniedModal.SetActive(false);
            SetInteractable(true);
            RoomDeniedModalStatusMessage.text = "";
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
                if (chatRoom.isPlaying)
                {
                    continue;
                }
                
                GameObject chatRoomItem = Instantiate(ChatRoomItemPrefab, ChatRoomListScrollContent);
                Button chatRoomButton = chatRoomItem.GetComponent<Button>();
                TMP_Text chatRoomIsPrivate = chatRoomItem.transform.Find("Text-RoomIsPrivate").GetComponent<TMP_Text>();
                TMP_Text chatRoomId = chatRoomItem.transform.Find("Text-RoomId").GetComponent<TMP_Text>();
                TMP_Text chatRoomName = chatRoomItem.transform.Find("Text-RoomName").GetComponent<TMP_Text>();
                TMP_Text chatRoomOwner = chatRoomItem.transform.Find("Text-RoomOwner").GetComponent<TMP_Text>();
                TMP_Text chatRoomCurrentClient = chatRoomItem.transform.Find("Text-CurrentClient").GetComponent<TMP_Text>();
                chatRoomIsPrivate.text = chatRoom.isPrivate ? "Private" : "Public";
                chatRoomId.text = chatRoom.roomId.ToString();
                chatRoomName.text = chatRoom.roomName;
                chatRoomOwner.text = chatRoom.roomOwner;
                chatRoomCurrentClient.text = $"{chatRoom.currentPlayers}/{chatRoom.maxClients}";
                if (chatRoom.isPrivate)
                {
                    chatRoomButton.onClick.AddListener(() => { OnPrivateRoomItemClicked(chatRoom); });
                }
                else
                {
                    chatRoomButton.onClick.AddListener(() => { OnPublicRoomItemClicked(chatRoom); });
                }
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
            RoomEnterModal.GetComponent<GraphicRaycaster>().enabled = true;
            RoomDeniedModal.GetComponent<GraphicRaycaster>().enabled = true;
        }
        
    
    }
}
