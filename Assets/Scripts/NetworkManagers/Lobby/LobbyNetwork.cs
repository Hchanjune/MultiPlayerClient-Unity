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
        
        public ColyseusRoom<LobbyState> Lobby;
        public bool IsConnected = false;
        private TaskCompletionSource<bool> _connectionTask;
        private ColyseusClient _client;

        public void InitializeClient(ColyseusClient client)
        {
            _client = client;
        }

        public async Task<bool> Connect(string username, string password)
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
                Lobby.OnMessage<string>("DUPLICATED_CONNECTION", OnDuplicatedConnection);
                Lobby.OnMessage<string>("NOT_AUTHENTICATED", OnNotAuthenticated);
                Lobby.OnMessage<ColyseusMatchMakeResponse>("CHAT_ROOM_CREATED", OnChatRoomCreated);
                Lobby.OnMessage<string>("ERROR_MESSAGE", OnServerError);
                return await _connectionTask.Task;
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
                EntryPointManager.Instance.UpdateStatusMessage("서버와의 연결상태가 좋지 않습니다.");
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

        private void OnNotAuthenticated(string message)
        {
            IsConnected = false;
            EntryPointManager.Instance.UpdateStatusMessage(message);
            _connectionTask.SetResult(false);
        }
        
        private void OnDuplicatedConnection(string message)
        {
            IsConnected = false;
            EntryPointManager.Instance.UpdateStatusMessage(message);
            _connectionTask.SetResult(false);
        }
        
        public void CreateChatRoom(Dictionary<string, object> roomOption)
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
        
    }
}