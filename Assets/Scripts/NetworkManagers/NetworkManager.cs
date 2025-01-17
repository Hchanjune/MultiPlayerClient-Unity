﻿using System;
using Colyseus;
using NetworkManagers.ChatRoom;
using NetworkManagers.Lobby;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace NetworkManagers
{
    public class NetworkManager
    {
        public ColyseusClient Client;

        public ClientInfo ClientInfo;
    
        public LobbyNetwork LobbyNetwork { get; private set; }
        public ChatRoomNetwork ChatRoomNetwork { get; private set; }

        [Inject]
        public void Construct(LobbyNetwork lobbyNetwork, ChatRoomNetwork chatRoomNetwork)
        {
            Client = new ColyseusClient("ws://localhost:3000");;
            InitializeLobbyNetwork(lobbyNetwork);
            InitializeChatRoomNetwork(chatRoomNetwork);
        }

        private void InitializeLobbyNetwork(LobbyNetwork lobbyNetwork)
        {
            LobbyNetwork = lobbyNetwork;
            LobbyNetwork.OnConnection += RegisterClientInfo;
            LobbyNetwork.InitializeClient(Client);
            LobbyNetwork.OnDisconnection += OnDisconnection;
        }
        
        private void InitializeChatRoomNetwork(ChatRoomNetwork chatRoomNetwork)
        {
            ChatRoomNetwork = chatRoomNetwork;
            LobbyNetwork.OnChatRoomJoined += SetCurrentChatRoom;
        }

        private void RegisterClientInfo(ClientInfo clientInfo)
        {
            ClientInfo = clientInfo;
        }

        private void SetCurrentChatRoom(ColyseusRoom<ChatRoomState> chatRoom)
        {
            ChatRoomNetwork.ChatRoom = chatRoom;
        }

        private void OnDisconnection(int closeCode)
        {
            Debug.Log(closeCode);
        }
        
        public void Disconnect()
        {
            if (LobbyNetwork.Lobby != null)
            {
                LobbyNetwork.Lobby.Leave();
            }
            Client = null;
            ClientInfo = null;
            SceneManager.LoadScene("EntryPoint");
        }
        
        
    
    }
}