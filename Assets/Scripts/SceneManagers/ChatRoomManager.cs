using NetworkManagers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace SceneManagers
{
    public class ChatRoomManager: MonoBehaviour
    {
        // UI
        public Button ExitBtn;
        

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


        private void OnExitBtnClicked()
        {
            _networkManager.ChatRoomNetwork.ChatRoom.Leave();
            SceneManager.LoadScene("CustomSessionList");
        }
        
        
        
        
    }
}