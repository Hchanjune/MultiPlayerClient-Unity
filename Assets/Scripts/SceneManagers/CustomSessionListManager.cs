using System.Collections.Generic;
using NetworkManagers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils.Extensions;
using Zenject;

namespace SceneManagers
{
    public class CustomSessionListManager : MonoBehaviour
    {
    
        public static CustomSessionListManager Instance { get; private set; }
        
        
        // UI
        public Button CreateSessionBtn;

        public Button BackToLobbyBtn;

        // Create Option Modal
        public GameObject RoomOptionModal;
        public Button CancelCreateRoomBtn;
        public Button CancelCreateRoomIconBtn;
        public Button SubmitCreateRoomBtn;
        public TMP_InputField OptionRoomName;
        public TMP_Dropdown OptionMaxClient;
        public Toggle OptionIsPrivate;
        public GameObject OptionPasswordArea;
        public TMP_InputField OptionPassword;
        
        
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

            // Back To Lobby EventListener
            BackToLobbyBtn.onClick.AddListener(OnBackToLobbyBtnClicked);
            // Create Session Btn
            CreateSessionBtn.onClick.AddListener(OpenRoomOptionModal);
            
            // Option Modal Events
            CloseRoomOptionModal();
            OptionPasswordArea.SetActive(false);
            OptionIsPrivate.onValueChanged.AddListener(delegate
            {
                OnOptionIsPrivateValueChanged(OptionIsPrivate);
            });
            SubmitCreateRoomBtn.onClick.AddListener(OnCreateRoomSubmit);
            CancelCreateRoomBtn.onClick.AddListener(CloseRoomOptionModal);
            CancelCreateRoomIconBtn.onClick.AddListener(CloseRoomOptionModal);
            
        }


        private void OnBackToLobbyBtnClicked()
        {
            SceneManager.LoadScene("Lobby");
        }

        private void OpenRoomOptionModal()
        {
            RoomOptionModal.SetActive(true);
            SetInteractable(false);
        }

        private void CloseRoomOptionModal()
        {
            RoomOptionModal.SetActive(false);
            SetInteractable(true);
        }

        private void OnOptionIsPrivateValueChanged(Toggle toggle)
        {
            OptionPasswordArea.SetActive(toggle.isOn);
        }

        private void OnCreateRoomSubmit()
        {
            Dictionary<string, object> options = new Dictionary<string, object>()
            {
                ["roomName"] = OptionRoomName.text,
                ["initialOwner"] = _networkManager?.ClientInfo?.id?? "DEBUG",
                ["maxClients"] = OptionMaxClient.value + 1,
                ["isPrivate"] = OptionIsPrivate.isOn,
                ["password"] = OptionIsPrivate.isOn ? OptionPassword.text : ""
            };

            if (_networkManager != null) _networkManager.LobbyNetwork.CreateChatRoom(options);
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
            RoomOptionModal.GetComponent<GraphicRaycaster>().enabled = true;
        }
        
    
    }
}
