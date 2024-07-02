using System;
using NetworkManagers;
using NetworkManagers.Lobby;
using SceneManagers;
using UnityEngine;
using Zenject;

namespace DI
{
    public class NetworkManagerInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            //Debug.Log("DI Injection Initialized");
            Container.Bind<NetworkManager>().AsSingle();
            Container.Bind<LobbyNetwork>().AsSingle();
        }
    }
}