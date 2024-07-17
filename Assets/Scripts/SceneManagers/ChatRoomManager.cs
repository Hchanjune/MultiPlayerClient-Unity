using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Colyseus.Schema;
using NetworkManagers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils.Extensions;
using Zenject;

namespace SceneManagers
{
    public class ChatRoomManager : MonoBehaviour
    {
        private static class TeamColor
        {
            public const string RED = "RED";
            public const string BLACK = "BLACK";
        }
        
        // UI - Header
        public TMP_Text RoomTitleText;
        public Button ExitBtn;
        public Button RoomConfigBtn;

        // UI - ChatArea
        public Transform ChatMessageScrollContent;
        public TMP_InputField ChatMessageInput;
        public Button ChatMessageSubmitBtn;

        // UI - UserList Area
        public Button RedTeamTitle;
        public RectTransform RedTeamPlayerList;
        public Button BlackTeamTitle;
        public RectTransform BlackTeamPlayerList;

        // UI - ReadyPanel
        public Button ReadyBtn;
        
        // UI - ConfigModal
        public GameObject RoomConfigModal;
        public Button CancelRoomConfigModalBtn;
        public Button CancelRoomConfigModalIconBtn;
        public TMP_Text ConfigModalStatusMessage;
        public Button SubmitRoomConfigModalBtn;
        public TMP_InputField ConfigRoomName;
        public TMP_Dropdown ConfigMaxClients;
        public Toggle ConfigIsPrivate;
        public TMP_InputField ConfigPassword;

        private NetworkManager _networkManager;

        [Inject]
        public void Constructor(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        private void Awake()
        {
            RegisterUiEventHandlers();
        }

        private void Start()
        {
            RegisterNetworkEvent();
        }

        private void OnDestroy()
        {
            _networkManager.ChatRoomNetwork.ChatRoom.OnStateChange -= OnChatRoomStateChange;
        }

        private void RegisterUiEventHandlers()
        {
            // Team Btn
            RedTeamTitle.onClick.AddListener(()=>OnTeamChangeRequest(TeamColor.RED));
            BlackTeamTitle.onClick.AddListener(()=>OnTeamChangeRequest(TeamColor.BLACK));
            
            // Chat Message
            ChatMessageInput.onSubmit.AddListener(SendChatMessage);
            ChatMessageSubmitBtn.onClick.AddListener(SendChatMessage);
            
            //ExitBtn
            ExitBtn.onClick.AddListener(OnExitBtnClicked);
            
            // Config Modal
            CloseConfigModal();
            RoomConfigBtn.onClick.AddListener(OpenConfigModal);
            CancelRoomConfigModalBtn.onClick.AddListener(CloseConfigModal);
            CancelRoomConfigModalIconBtn.onClick.AddListener(CloseConfigModal);
            SubmitRoomConfigModalBtn.onClick.AddListener(OnRoomConfigRequest);
        }
        
        private void RegisterNetworkEvent()
        {

            OnChatRoomStateChange(_networkManager.ChatRoomNetwork.ChatRoom.State, true);
            _networkManager.ChatRoomNetwork.ChatRoom.OnStateChange += OnChatRoomStateChange;
            
            // OnMessages
            _networkManager.ChatRoomNetwork.ChatRoom.OnMessage<ChatMessage>("ECHO_CHAT_MESSAGE", OnChatMessageEcho);
            _networkManager.ChatRoomNetwork.ChatRoom.OnMessage<ChatMessage>("PLAYER_JOINED", OnSystemMessage);
            _networkManager.ChatRoomNetwork.ChatRoom.OnMessage<ChatMessage>("PLAYER_LEAVED", OnSystemMessage);
            _networkManager.ChatRoomNetwork.ChatRoom.OnMessage<ChatMessage>("OWNER_CHANGED", OnSystemMessage);
            _networkManager.ChatRoomNetwork.ChatRoom.OnMessage<string>("READY_TO_START", OnReadyToStart);
            _networkManager.ChatRoomNetwork.ChatRoom.OnMessage<string>("START_CANCELED", OnStartCanceled);
            _networkManager.ChatRoomNetwork.ChatRoom.OnMessage<string>("ROOM_CONFIG_SUCCESS", OnRoomConfigSuccess);
            _networkManager.ChatRoomNetwork.ChatRoom.OnMessage<string>("ROOM_CONFIG_FAILURE", OnRoomConfigFailure);
        }
        

        private void OnChatRoomStateChange(ChatRoomState state, bool isFirstState)
        {
            OnRoomTitleChange(state.roomName, state.isPrivate);
            OnRoomPlayerChange(state);
            if (state.chatRoomPlayers.Count > 0)
            {
                SetReadyBtnState(state.roomOwner);
            }
        }

        private void OnTeamChangeRequest(string color)
        {
            string currentTeamColor = _networkManager.ChatRoomNetwork.ChatRoom.State
                .chatRoomPlayers[_networkManager.ChatRoomNetwork.ChatRoom.SessionId].team;
            if (currentTeamColor == color) return;
            switch (color)
            {
                case TeamColor.RED:
                {
                    if (_networkManager.ChatRoomNetwork.ChatRoom.State.redTeam.Count == _networkManager.ChatRoomNetwork.ChatRoom.State.maxClients/2) return;
                    break;
                }
                case TeamColor.BLACK:
                {
                    if (_networkManager.ChatRoomNetwork.ChatRoom.State.blackTeam.Count == _networkManager.ChatRoomNetwork.ChatRoom.State.maxClients/2) return;
                    break;
                }
            }
            _networkManager.ChatRoomNetwork.ChatRoom.Send("TEAM_CHANGE_REQUEST", color);
        }

        private void OnRoomTitleChange(string roomName, bool isPrivate)
        {
            RoomTitleText.text = $"{(isPrivate ? "[비공개]" : "[공개]")} - {roomName}";
        }

        private void SetReadyBtnState(string roomOwner)
        {
            string sessionId = _networkManager.ChatRoomNetwork.ChatRoom.SessionId;
            ChatRoomPlayer currentPlayer = _networkManager.ChatRoomNetwork.ChatRoom.State.chatRoomPlayers[sessionId];
            TMP_Text btnTextComponent = ReadyBtn.GetComponentInChildren<TMP_Text>();
            ReadyBtn.onClick.RemoveAllListeners();
            if (roomOwner == currentPlayer.id)
            {
                RoomConfigBtn.gameObject.SetActive(true);
                btnTextComponent.text = "시작";
                ReadyBtn.onClick.AddListener(OnStartRequest);
            }
            else if (currentPlayer.isReady)
            {
                RoomConfigBtn.gameObject.SetActive(false);
                btnTextComponent.text = "취소";
                ReadyBtn.onClick.AddListener(OnReadyCancelRequest);
            }
            else
            {
                RoomConfigBtn.gameObject.SetActive(false);
                btnTextComponent.text = "준비";
                ReadyBtn.onClick.AddListener(OnReadyRequest);
            }
        }

        private void OnChatMessageEcho(ChatMessage message)
        {
            GameObject chatObject = new GameObject();
            chatObject.transform.SetParent(ChatMessageScrollContent);
            TextMeshProUGUI chat = chatObject.AddComponent<TextMeshProUGUI>();
            chat.text = $"{message.senderId}: {message.message}";
            chat.fontSize = 24;
            chat.color = Color.black;
            StartCoroutine(ChatAreaAutoScroll(ChatMessageScrollContent.GetComponentInParent<ScrollRect>()));
        }

        private void OnSystemMessage(ChatMessage message)
        {
            GameObject chatObject = new GameObject();
            chatObject.transform.SetParent(ChatMessageScrollContent);
            TextMeshProUGUI chat = chatObject.AddComponent<TextMeshProUGUI>();
            chat.text = $"{message.senderId}: {message.message}";
            chat.fontSize = 24;
            chat.color = Color.black;
            StartCoroutine(ChatAreaAutoScroll(ChatMessageScrollContent.GetComponentInParent<ScrollRect>()));
        }

        private IEnumerator ChatAreaAutoScroll(ScrollRect scrollRect)
        {
            yield return new WaitForEndOfFrame();
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0;
        }
        
        private void SendChatMessage(string message)
        {
            if (!message.IsNullOrBlank())
            {
                _networkManager.ChatRoomNetwork.ChatRoom.Send("CHAT_REQUEST", message);
                ChatMessageInput.text = "";
                ChatMessageInput.Select();
                ChatMessageInput.ActivateInputField();
            }
        }
    
        private void SendChatMessage()
        {
            string message = ChatMessageInput.text;
            if (!message.IsNullOrBlank())
            {
                _networkManager.ChatRoomNetwork.ChatRoom.Send("CHAT_REQUEST", message);
                ChatMessageInput.text = "";
                ChatMessageInput.Select();
                ChatMessageInput.ActivateInputField();
            }
        }

        private void OnRoomPlayerChange(ChatRoomState state)
        {
            // 기존 플레이어 리스트 정리
            foreach (Transform child in RedTeamPlayerList)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in BlackTeamPlayerList)
            {
                Destroy(child.gameObject);
            }

            // RED
            state.redTeam.ForEach(player =>
            {
                GameObject textObject = new GameObject(player.id);
                textObject.transform.SetParent(RedTeamPlayerList, false);
                TextMeshProUGUI tmpText = textObject.AddComponent<TextMeshProUGUI>();
                tmpText.text = state.roomOwner.Equals(player.id) ? $"[*]{player.id}" : player.id;
                tmpText.fontSize = 24;
                tmpText.color = player.isReady ? Color.green : Color.black;
                tmpText.alignment = TextAlignmentOptions.Center;
            });
            
            // BLACK
            state.blackTeam.ForEach(player =>
            {
                GameObject textObject = new GameObject(player.id);
                textObject.transform.SetParent(BlackTeamPlayerList, false);
                TextMeshProUGUI tmpText = textObject.AddComponent<TextMeshProUGUI>();
                tmpText.text = state.roomOwner.Equals(player.id) ? $"[*]{player.id}" : player.id;
                tmpText.fontSize = 24;
                tmpText.color = player.isReady ? Color.green : Color.black;
                tmpText.alignment = TextAlignmentOptions.Center;
            });
        }

        private void OnStartRequest()
        {
            _networkManager.ChatRoomNetwork.ChatRoom.Send("START");
        }

        private void OnReadyRequest()
        {
            _networkManager.ChatRoomNetwork.ChatRoom.Send("READY");
        }

        private void OnReadyCancelRequest()
        {
            _networkManager.ChatRoomNetwork.ChatRoom.Send("CANCEL_READY");
        }

        private void OnReadyToStart(string message)
        {
            RoomConfigBtn.interactable = false;
            ReadyBtn.interactable = false;
            ExitBtn.interactable = false;
        }

        private void OnStartCanceled(string message)
        {
            RoomConfigBtn.interactable = true;
            ReadyBtn.interactable = true;
            ExitBtn.interactable = true;
        }

        private void OpenConfigModal()
        {
            ChatRoomState roomState = _networkManager.ChatRoomNetwork.ChatRoom.State;
            ConfigModalStatusMessage.color = Color.black;
            ConfigModalStatusMessage.fontSize = 16;
            ConfigModalStatusMessage.text = "현재 세션을 설정하여 주세요";
            ConfigRoomName.text = roomState.roomName;
            switch (roomState.maxClients)
            {
                case 1:
                    ConfigMaxClients.value = 0;
                    break;
                case 2:
                    ConfigMaxClients.value = 1;
                    break;
                case 4:
                    ConfigMaxClients.value = 2;
                    break;
                case 6:
                    ConfigMaxClients.value = 3;
                    break;
                case 8:
                    ConfigMaxClients.value = 4;
                    break;
                case 10:
                    ConfigMaxClients.value = 5;
                    break;
            }
            ConfigIsPrivate.isOn = roomState.isPrivate;
            ConfigPassword.text = roomState.password;
            RoomConfigModal.SetActive(true);
            SetInteractable(false);
        }

        private void CloseConfigModal()
        {
            RoomConfigModal.SetActive(false);
            SetInteractable(true);
        }

        private void OnRoomConfigRequest()
        {
            Dictionary<string, object> options = new Dictionary<string, object>()
            {
                ["roomName"] = ConfigRoomName.text,
                ["maxClients"] = int.Parse(new string(ConfigMaxClients.options[ConfigMaxClients.value].text.Where(char.IsDigit).ToArray())),
                ["isPrivate"] = ConfigIsPrivate.isOn,
                ["password"] = ConfigPassword.text
            };

            _networkManager.ChatRoomNetwork.ChatRoom.Send("ROOM_CONFIG", options);
        }

        private void OnRoomConfigSuccess(string message)
        {
            CloseConfigModal();
        }

        private void OnRoomConfigFailure(string message)
        {
            ConfigModalStatusMessage.text = message;
            ConfigModalStatusMessage.fontSize = 14;
            ConfigModalStatusMessage.color = Color.red;
        }
        
        private void OnExitBtnClicked()
        {
            _networkManager.ChatRoomNetwork.ChatRoom.Leave();
            SceneManager.LoadScene("CustomSessionList");
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
            RoomConfigModal.GetComponent<GraphicRaycaster>().enabled = true;
        }
    }
}
