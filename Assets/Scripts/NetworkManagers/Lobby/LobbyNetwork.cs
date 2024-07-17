using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Colyseus;
using NetworkManagers.ChatRoom;
using SceneManagers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Extensions;

namespace NetworkManagers.Lobby
{
    public enum LobbyRequest
    {
        
    }
    public enum LobbyResponse
    {
        [Description("CONNECTED")]
        CONNECTED,
        [Description("NOT_AUTHORIZED")]
        NOT_AUTHORIZED,
        [Description("DUPLICATED_CONNECTION")]
        DUPLICATED_CONNECTION
    }

    public class LobbyNetwork
    {
        public event Action<ClientInfo> OnConnection;
        public event Action<ColyseusRoom<ChatRoomState>> OnChatRoomJoined;
        public event Action<int> OnDisconnection;
        public event Action<string> OnChatRoomPasswordError;
        public event Action<string> OnChatRoomFull; 
        
        public ColyseusRoom<LobbyState> Lobby;
        public bool IsConnected = false;
        private TaskCompletionSource<bool> _connectionTask;
        private ColyseusClient _client;

        public void InitializeClient(ColyseusClient client)
        {
            _client = client;
        }

        public async Task<bool> Connect(string username, string password, Action<string> updateStatusMessage)
        {
            try
            {
                _connectionTask = new TaskCompletionSource<bool>();
                Dictionary<string, object> options = new Dictionary<string, object>()
                {
                    ["id"] = username,
                    ["password"] = password
                };
                Lobby = await _client.JoinOrCreate<LobbyState>("Lobby", options);
                Lobby.OnMessage<ClientInfo>("CONNECTED", OnConnected);
                Lobby.OnMessage<string>("DUPLICATED_CONNECTION", message => OnDuplicatedConnection(message, updateStatusMessage));
                Lobby.OnMessage<string>("NOT_AUTHENTICATED", message => OnNotAuthenticated(message, updateStatusMessage));
                Lobby.OnMessage<ColyseusMatchMakeResponse>("CHAT_ROOM_CREATED", OnChatRoomCreated);
                Lobby.OnMessage<ColyseusMatchMakeResponse>("CHAT_ROOM_AUTHORIZED", OnJoinChatRoomAuthorized);
                Lobby.OnMessage<string>("CHAT_ROOM_PASSWORD_ERROR", message => OnChatRoomPasswordError?.Invoke(message));
                Lobby.OnMessage<string>("CHAT_ROOM_FULL", message => OnChatRoomFull?.Invoke(message));
                Lobby.OnMessage<string>("ERROR_MESSAGE", OnServerError);
                Lobby.colyseusConnection.OnClose += OnDisconnected;
                return await _connectionTask.Task;
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
                updateStatusMessage("서버와의 연결상태가 좋지 않습니다.");
                IsConnected = false;
                return false;
            }
        }

        private void OnServerError(string message)
        {
            Debug.LogError(message);
        }
        
        private void OnConnected(ClientInfo clientInfo)
        {
            Debug.Log(clientInfo.ToStringReflection());
            IsConnected = true;
            OnConnection?.Invoke(clientInfo);
            _connectionTask.SetResult(true);
        }

        private void OnNotAuthenticated(string message, Action<string> updateStatusMessage)
        {
            IsConnected = false;
            updateStatusMessage(message);
            _connectionTask.SetResult(false);
        }
        
        private void OnDuplicatedConnection(string message, Action<string> updateStatusMessage)
        {
            IsConnected = false;
            updateStatusMessage(message);
            _connectionTask.SetResult(false);
        }

        private void OnDisconnected(int closeCode)
        {
            OnDisconnection?.Invoke(closeCode);
        }
        
        public void CreateChatRoomRequest(Dictionary<string, object> roomOption)
        {
            Lobby.Send("CREATE_CUSTOM_CHAT_ROOM", roomOption);
        }

        private async void OnChatRoomCreated(ColyseusMatchMakeResponse seatReservation)
        {
            try
            {
                ColyseusRoom<ChatRoomState> chatRoom = await _client.ConsumeSeatReservation<ChatRoomState>(seatReservation);
                OnChatRoomJoined?.Invoke(chatRoom);
                Debug.Log($"Joined chat room with sessionId: {chatRoom.RoomId}");
                SceneManager.LoadScene("ChatRoom");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to Create ChatRoom: {e.Message}");
            }
        }

        public void JoinChatRoomRequest(Dictionary<string, object> joinOption)
        {
            Lobby.Send("JOIN_CUSTOM_CHAT_ROOM", joinOption);
        }

        private async void OnJoinChatRoomAuthorized(ColyseusMatchMakeResponse seatReservation)
        {
            try
            {
                ColyseusRoom<ChatRoomState> chatRoom = await _client.ConsumeSeatReservation<ChatRoomState>(seatReservation);
                OnChatRoomJoined?.Invoke(chatRoom);
                Debug.Log($"Joined chat room with sessionId: {chatRoom.RoomId}");
                SceneManager.LoadScene("ChatRoom");
            }
            catch (InvalidCastException e)
            {
                Debug.LogError($"InvalidCastException: {e.Message}");
                // 추가적인 디버깅 정보 출력
                Debug.LogError($"Seat Reservation: {seatReservation}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to Join ChatRoom: {e.Message}");
            }
        }
        
    }
}