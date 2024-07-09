using NetworkManagers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace SceneManagers
{
    public class ChatRoomManager: MonoBehaviour
    {
        public static ChatRoomManager Instance { get; private set; }
        
        
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
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            ExitBtn.onClick.AddListener(OnExitBtnClicked);
        }


        private void OnExitBtnClicked()
        {
            _networkManager.ChatRoomNetwork.ChatRoom.Leave();
            SceneManager.LoadScene("CustomSessionList");
        }
        
        
        
        
    }
}