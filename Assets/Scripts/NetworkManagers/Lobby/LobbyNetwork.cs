using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Colyseus;
using SceneManagers;
using UnityEngine;

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
        
        public ColyseusRoom<object> Lobby;
        public bool IsConnected = false;
        private ColyseusClient _client;
        private List<ColyseusRoomAvailable> allRooms = new List<ColyseusRoomAvailable>();

        public void InitializeClient(ColyseusClient client)
        {
            _client = client;
        }

        public async Task<bool> Connect(string username, string password)
        {
            try
            {
                Dictionary<string, object> options = new Dictionary<string, object>()
                {
                    ["username"] = username,
                    ["password"] = password
                };
                Lobby = await _client.JoinOrCreate("Lobby", options);
                Lobby.OnMessage<string>("DUPLICATED_CONNECTION", OnDuplicatedConnection);
                Lobby.OnMessage<string>("NOT_AUTHENTICATED", OnNotAuthenticated);
                Lobby.OnMessage<List<ColyseusRoomAvailable>>("rooms", OnRoomsUpdate);
                Lobby.OnMessage<ColyseusRoomAvailable>("+", OnRoomAdded);
                Lobby.OnMessage<string>("-", OnRoomRemoved);
                IsConnected = true;
                return IsConnected;
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
                EntryPointManager.Instance.UpdateStatusMessage("서버와의 연결상태가 좋지 않습니다.");
                IsConnected = false;
                return IsConnected;
            }
        }

        private void OnNotAuthenticated(string message)
        {
            EntryPointManager.Instance.UpdateStatusMessage(message);
        }
        
        private void OnDuplicatedConnection(string message)
        {
            EntryPointManager.Instance.UpdateStatusMessage(message);
        }
        
        private void OnRoomsUpdate(List<ColyseusRoomAvailable> rooms)
        {
            allRooms = rooms;
            foreach (var room in rooms)
            {
                Debug.Log(
                    $"Room ID: {room.roomId}, Clients: {room.clients}, Max Clients: {room.maxClients}");
            }
        }

        private void OnRoomAdded(ColyseusRoomAvailable room)
        {
            allRooms.Add(room);
            Debug.Log($"Room Added - ID: {room.roomId}, Clients: {room.clients}, Max Clients: {room.maxClients}");
        }

        private void OnRoomRemoved(string roomId)
        {
            allRooms.RemoveAll(room => room.roomId == roomId);
            Debug.Log($"Room Removed - ID: {roomId}");
        }
    }
}