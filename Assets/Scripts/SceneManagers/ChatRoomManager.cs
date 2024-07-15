using System;
using Colyseus.Schema;
using NetworkManagers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
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

        public TMP_Text DebugMessage;

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
            OnChatRoomStateChange(_networkManager.ChatRoomNetwork.ChatRoom.State, true);
            _networkManager.ChatRoomNetwork.ChatRoom.OnStateChange += OnChatRoomStateChange;
            //Debug.Log("RegisterNetworkEvent: OnStateChange 이벤트가 등록되었습니다.");
        }

        private void OnChatRoomStateChange(ChatRoomState state, bool isFirstState)
        {
            Debug.Log($"OnChatRoomStateChange: State received. RoomName: {state.roomName}, IsPrivate: {state.isPrivate}");
            Debug.Log($"Players: {state.players.Count}");

            foreach (ChatRoomPlayer player in state.players.Values)
            {
                Debug.Log($"Player ID: {player.id}, Name: {player.name}");
            }
            OnRoomTitleChange(state.roomName, state.isPrivate);
            OnRoomPlayerChange(state.players);
        }

        private void OnRoomTitleChange(string roomName, bool isPrivate)
        {
            RoomTitleText.text = $"{(isPrivate ? "[비공개]" : "[공개]")} - {roomName}";
        }

        private void OnRoomPlayerChange(MapSchema<ChatRoomPlayer> players)
        {
            // RedTeamPlayerList와 BlackTeamPlayerList가 null인지 확인
            if (RedTeamPlayerList == null || BlackTeamPlayerList == null)
            {
                Debug.LogWarning("RedTeamPlayerList 또는 BlackTeamPlayerList가 null입니다.");
                return;
            }

            Debug.Log("OnRoomPlayerChange: 플레이어 목록이 변경되었습니다.");

            foreach (Transform child in RedTeamPlayerList)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in BlackTeamPlayerList)
            {
                Destroy(child.gameObject);
            }

            foreach (ChatRoomPlayer player in players.Values)
            {
                // 새로운 GameObject를 생성
                GameObject textObject = new GameObject(player.id);

                // 부모를 설정하여 계층 구조를 유지
                textObject.transform.SetParent(RedTeamPlayerList, false);

                // TextMeshProUGUI 컴포넌트를 추가
                TextMeshProUGUI tmpText = textObject.AddComponent<TextMeshProUGUI>();

                // 텍스트 설정
                tmpText.text = player.id;

                // 추가적인 설정 (선택 사항)
                tmpText.fontSize = 24;
                tmpText.color = Color.black;
                tmpText.alignment = TextAlignmentOptions.Center;
            }
        }

        private void OnExitBtnClicked()
        {
            _networkManager.ChatRoomNetwork.ChatRoom.Leave();
            SceneManager.LoadScene("CustomSessionList");
        }
    }
}
