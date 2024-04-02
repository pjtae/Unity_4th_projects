using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UI.Slot;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace MP.UI
{
    public class UIReadyGamePlayReadyisRoom : UIScreenbase, IInRoomCallbacks//��ȿ����� �ݹ�
    {
        private bool canStartGamePlay
        {
            get => _canStartGamePlay;
            set
            {
                _canStartGamePlay = value;

                // ���� �����̸� ���۹�ư Ȱ��ȭ
                if (PhotonNetwork.IsMasterClient)
                {
                    _start.interactable = value;
                }

            }
        }

        private bool _canStartGamePlay;
        private ReadyGamePlayInRoomPlayerSlot[] _playerSlots;
        private Button _start;
        private Toggle _ready;
        protected override void Awake()
        {
            base.Awake();

            _playerSlots =
                transform.Find("Panel/PlayerList").GetComponentsInChildren<ReadyGamePlayInRoomPlayerSlot>();
            _start = transform.Find("Panel/Button - Start").GetComponent<Button>();
            _ready = transform.Find("Panel/Button - Ready").GetComponent<Toggle>();

            _start.onClick.AddListener(() =>
            {
                if (PhotonNetwork.IsMasterClient == false)
                    return;

                if (canStartGamePlay == false)
                    return;

                PhotonNetwork.LoadLevel("GamePlay");
            });

            _ready.onValueChanged.AddListener(value =>
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable()
                {
                    { "isReady", value }
                });
            });
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
            StartCoroutine(C_Init());

        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private IEnumerator C_Init()
        {
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Joined);
            yield return StartCoroutine(C_RefreshPlayerSlots(PhotonNetwork.LocalPlayer));
            _start.gameObject.SetActive(PhotonNetwork.IsMasterClient == true);
            _ready.gameObject.SetActive(PhotonNetwork.IsMasterClient == false);
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            _start.gameObject.SetActive(newMasterClient.IsLocal == true); //���� �����̸� Start ��ư Ȱ��ȭ
            _ready.gameObject.SetActive(newMasterClient.IsLocal == false); //���� ����̸� Ready ��� Ȱ��ȭ
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            StartCoroutine(C_RefreshPlayerSlots(newPlayer));
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            RefreshPlayerSlots();
        }
        
        public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            
            for (int i = 0; i < _playerSlots.Length; i++)
            {
                if (_playerSlots[i].nickname.Equals(targetPlayer.NickName))
                {
                    if (changedProps.TryGetValue("isReady", out object value))
                    {
                        _playerSlots[i].isReady = (bool)value;
                    }
                }
            }

            RefreshCanStartGamePlay();
        }



        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
        }

        private void RefreshPlayerSlots()
        {
            for(int i = 0; i < _playerSlots.Length; i++)
            {
                if (i > PhotonNetwork.PlayerList.Length)
                {
                    _playerSlots[i].nickname = PhotonNetwork.PlayerList[i].NickName;
                    _playerSlots[i].isReady = (bool)PhotonNetwork.PlayerList[i].CustomProperties["isReady"];
                }
                else
                {
                    _playerSlots[i].nickname = string.Empty;
                    _playerSlots[i].isReady = false;
                }
            }
            /*IEnumerator e1 = _playerSlots.GetEnumerator();
            IEnumerator e2 = PhotonNetwork.PlayerList.GetEnumerator();

            while (e1.MoveNext())
            {
                if (e2.MoveNext())
                {
                    
                }
                else
                {

                }
            }*/
        }

        /// <summary>
        /// ���� ���� �÷��̾�� ���� isReady�� ���� CustomProperty ���Ⱑ ������ �ʾ��� �� �����Ƿ�,
        /// CustomProperty ���Ⱑ �Ϸ�Ǳ� ��ٷȴٰ� ���� ����.
        /// </summary>      
        private IEnumerator C_RefreshPlayerSlots(Player newPlayer)
        {
            yield return new WaitUntil(() => newPlayer.CustomProperties.ContainsKey("isReady"));
            RefreshPlayerSlots();
        }

        /// <summary>
        /// ���� ���� ���� ���� ����
        /// </summary>
        private void RefreshCanStartGamePlay()
        {
            //������ �ٸ� �÷��̾ ��� ���� �ߴ����� ���ؼ� üũ�ؾ���
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                //������ ���� �غ� ���ص� ��
                if (player.IsMasterClient)
                    continue;

                if (player.CustomProperties.TryGetValue("isReady", out object isReady))
                {
                    // �غ� ���� �� ã��
                    if ((bool)isReady == false)
                    {
                        canStartGamePlay = false;
                        return;
                    }
                }
                // ���� ���� �� ���� �غ� �ȵ�
                else
                {
                    canStartGamePlay = false;
                    return;
                }
            }

            canStartGamePlay = true;
        }
    }
}
