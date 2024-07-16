using System;
using System.Collections;
using Colyseus.Schema;
using NetworkManagers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils.Extensions;
using Zenject;

namespace SceneManagers
{
    public class ChatRoomManager : MonoBehaviour
    {
        // UI - Header
        public TMP_Text RoomTitleText;
        public Button ExitBtn;
        public Button RoomConfigBtn;

        // UI - ChatArea
        public Transform ChatMessageScrollContent;
        public TMP_InputField ChatMessageInput;
        public Button ChatMessageSubmitBtn;

        // UI - UserList Area
        public RectTransform RedTeamPlayerList;
        public RectTransform BlackTeamPlayerList;

        // UI - ReadyPanel
        public Button ReadyBtn;

        private NetworkManager _networkManager;

        [Inject]
        public void Constructor(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        private void Awake()
        {
            ChatMessageInput.onSubmit.AddListener(SendChatMessage);
            ChatMessageSubmitBtn.onClick.AddListener(SendChatMessage);
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

            OnChatRoomStateChange(_networkManager.ChatRoomNetwork.ChatRoom.State, true);
            _networkManager.ChatRoomNetwork.ChatRoom.OnStateChange += OnChatRoomStateChange;
            
            // OnMessages
            _networkManager.ChatRoomNetwork.ChatRoom.OnMessage<ChatMessage>("ECHO_CHAT_MESSAGE", OnChatMessageEcho);
            _networkManager.ChatRoomNetwork.ChatRoom.OnMessage<ChatMessage>("PLAYER_JOINED", OnSystemMessage);
            _networkManager.ChatRoomNetwork.ChatRoom.OnMessage<ChatMessage>("PLAYER_LEAVED", OnSystemMessage);
            _networkManager.ChatRoomNetwork.ChatRoom.OnMessage<ChatMessage>("OWNER_CHANGED", OnSystemMessage);
        }
        

        private void OnChatRoomStateChange(ChatRoomState state, bool isFirstState)
        {
            OnRoomTitleChange(state.roomName, state.isPrivate);
            OnRoomPlayerChange(state);
            SetReadyBtnState(state.roomOwner);
        }

        private void OnRoomTitleChange(string roomName, bool isPrivate)
        {
            RoomTitleText.text = $"{(isPrivate ? "[비공개]" : "[공개]")} - {roomName}";
        }

        private void SetReadyBtnState(string roomOwner)
        { 
            ReadyBtn.GetComponentInChildren<TMP_Text>().text = (roomOwner == _networkManager.ClientInfo.id) ? "시작" : "준비";                
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
            state.redTeam.ForEach((player) =>
            {
                GameObject textObject = new GameObject(player.id);
                textObject.transform.SetParent(RedTeamPlayerList, false);
                TextMeshProUGUI tmpText = textObject.AddComponent<TextMeshProUGUI>();
                tmpText.text = state.roomOwner.Equals(player.id) ? $"[*]{player.id}" : player.id;
                tmpText.fontSize = 24;
                tmpText.color = Color.black;
                tmpText.alignment = TextAlignmentOptions.Center;
            });
            
            // BLACK
            state.blackTeam.ForEach((player) =>
            {
                GameObject textObject = new GameObject(player.id);
                textObject.transform.SetParent(BlackTeamPlayerList, false);
                TextMeshProUGUI tmpText = textObject.AddComponent<TextMeshProUGUI>();
                tmpText.text = state.roomOwner.Equals(player.id) ? $"[*]{player.id}" : player.id;
                tmpText.fontSize = 24;
                tmpText.color = Color.black;
                tmpText.alignment = TextAlignmentOptions.Center;
            });
        }

        private void OnExitBtnClicked()
        {
            _networkManager.ChatRoomNetwork.ChatRoom.Leave();
            SceneManager.LoadScene("CustomSessionList");
        }
    }
}
