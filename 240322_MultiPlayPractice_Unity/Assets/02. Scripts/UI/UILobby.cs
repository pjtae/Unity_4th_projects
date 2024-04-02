using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace MP.UI
{
    public class UILobby : UIScreenbase, ILobbyCallbacks, IMatchmakingCallbacks//받아오기 위해서는 포톤 네트워크를 받는 대상이라고 선정해줘야함
    {
        public const int NOT_SELECTED = -1;

        public int roomListSlotIndexSelected
        {
            get => _roomListSlotIndexSelected;
            set
            {
                _roomListSlotIndexSelected = value;
                _joinRoom.interactable = value > NOT_SELECTED;
            }
        }
        

        // Main panel
        private int _roomListSlotIndexSelected = NOT_SELECTED;
        private RoomListSlot _roomListSlot;
        private List<RoomListSlot> _roomListSlots = new List<RoomListSlot>(20); //리썰팅을 하는이유 / 가비지 컬렉션이 일어나는걸 방지하기 위해서 o/n 
        private RectTransform _roomListContent;
        private Button _joinRoom;
        private Button _createRoom;
        private List<RoomInfo> _localRoomList;

        // Roomoption panel
        private GameObject _roomOptionPanel;
        private TMP_InputField _roomName;
        private Scrollbar _maxPlayer;
        private TMP_Text _maxPlayerValue;
        private Button _confirmRoomOptions;
        private Button _cancelRoomOptions;



        //재사용풀 사용, 오브젝트 풀링을 사용해야함
        protected override void Awake()
        {
            base.Awake();
            _roomListSlot = Resources.Load<RoomListSlot>("UI/RoomListSlot");
            _roomListContent = transform.Find("Panel/Scroll View - RoomList/Viewport/Content").GetComponent<RectTransform>();
            _joinRoom = transform.Find("Panel/Button - Join").GetComponent<Button>();
            _createRoom = transform.Find("Panel/Button - Create").GetComponent<Button>();
            _joinRoom.interactable = false;
            _joinRoom.onClick.AddListener(() =>
            {
                if (PhotonNetwork.JoinRoom(_localRoomList[_roomListSlotIndexSelected].Name))
                {
                    //todo ==> 입장중.. 같은 메세지 띄우면서 유저한테 기다려달라고 하는 UI 띄우기
                    UIManager.instance.Get<UILodingPanel>()
                                        .Show();
                }
                else
                {
                    UIManager.instance.Get<UIWarningWindow>()
                                      .Show("Thre room is invalid");        
                }
            });
            _createRoom.onClick.AddListener(() =>
            {
                // todo => 방 생성 옵션 창 띄우기
                _roomName.text = string.Empty;
                _maxPlayer.value = 0f;
                _roomOptionPanel.gameObject.SetActive(true);
            });

            _roomOptionPanel = transform.Find("Panel - RoomOptions").gameObject;
            _roomName = transform.Find("Panel - RoomOptions/Panel/InputField (TMP) - RoomName").GetComponent<TMP_InputField>();
            _maxPlayer = transform.Find("Panel - RoomOptions/Panel/Scrollbar - MaxPlayer").GetComponent<Scrollbar>();
            _maxPlayerValue = transform.Find("Panel - RoomOptions/Panel/Text (TMP) - MaxPlayerValue").GetComponent<TMP_Text>();
            _confirmRoomOptions = transform.Find("Panel - RoomOptions/Panel/Button - ConformOptions").GetComponent<Button>();
            _cancelRoomOptions = transform.Find("Panel - RoomOptions/Panel/Button - CancelRoomOptions").GetComponent<Button>();

            _roomName.onValueChanged.AddListener(value => _confirmRoomOptions.interactable = value.Length > 1); // 방제목 두글자 이상일때만 확인버튼 누를수 있음
            _maxPlayer.onValueChanged.AddListener(value =>
            {
                _maxPlayerValue.text = Mathf.RoundToInt(value * _maxPlayer.numberOfSteps + 1).ToString();
            });
            _cancelRoomOptions.onClick.AddListener(()=> _roomOptionPanel.gameObject.SetActive(false));
            _confirmRoomOptions.interactable = false;
            _confirmRoomOptions.onClick.AddListener(() =>
            {
                if (PhotonNetwork.CreateRoom(_roomName.text,
                                        new RoomOptions
                                        {
                                            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
                                            {
                                                { "levelLimit", 10},
                                            },

                                            MaxPlayers = Mathf.RoundToInt(_maxPlayer.value * _maxPlayer.numberOfSteps + 1),
                                            PublishUserId = true
                                        }))
                {
                    // todo => 방생성중.. 같은 메세지 띄우면서 유저한테 기다려달라고 하는 UI 띄우기
                    UIManager.instance.Get<UILodingPanel>()
                                        .Show();
                }
            });
        }


        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
            StartCoroutine(C_JoinLobby());
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void OnJoinedLobby()
        {
            Debug.Log("[UILobby : Joined ]");
            //todo => join 되기 전에는 로딩패널가은거 띄어놓고 이 함수가 호출되면 로딩패널 치워주기
            UIManager.instance.Get<UILodingPanel>()
                                        .Hide();
        }

        public void OnLeftLobby()
        {
            throw new NotImplementedException();
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            _localRoomList = roomList;

            for (int i = 0; i < _roomListSlots.Count; i++)
                Destroy(_roomListSlots[i].gameObject);
            
            _roomListSlots.Clear();

            for (int i = 0; i < roomList.Count; i++)
            {
                RoomListSlot slot = Instantiate(_roomListSlot, _roomListContent);
                slot.roomIndex = i;
                slot.Refresh(roomList[i].Name, roomList[i].PlayerCount, roomList[i].MaxPlayers);
                slot.onSelected += (index) => roomListSlotIndexSelected = index;
                _roomListSlots.Add(slot);
            }
        }

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            throw new NotImplementedException();
        }

        IEnumerator C_JoinLobby()
        {
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);
            PhotonNetwork.JoinLobby();
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            throw new NotImplementedException();
        }

        public void OnCreatedRoom()
        {
            UIManager.instance.Get<UILodingPanel>()
                            .Hide();
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            throw new NotImplementedException();
        }

        public void OnJoinedRoom()
        {
            UIManager.instance.Get<UILodingPanel>()
                            .Hide();
            UIManager.instance.Get<UIReadyGamePlayReadyisRoom>()
                            .Show();
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
            UIManager.instance.Get<UILodingPanel>()
                            .Hide();
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
            UIManager.instance.Get<UILodingPanel>()
                            .Hide();
        }

        public void OnLeftRoom()
        {
            throw new NotImplementedException();
        }
    }
}
