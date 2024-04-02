using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MP.GameElements
{
    [RequireComponent(typeof(PhotonView))]
    public class ClientCharacterControllerturnObsever : MonoBehaviour, IPunObservable
    {
        public Vector3 velocity;
        private PhotonView _photonview;

        private void Awake()
        {
            _photonview = GetComponent<PhotonView>();
        }

        private void Update()
        {
            velocity = new Vector3(Input.GetAxis("Vertical"), 0f, Input.GetAxis("Horizontal"));
        }

        private void FixedUpdate()
        {
            if (_photonview.IsMine)
            {
                transform.position += velocity * Time.fixedDeltaTime;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //��ü�� �����ͷ� �ٲٴ� Serialize

            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
            }
            else
            {
                transform.position = (Vector3)stream.ReceiveNext();
                transform.rotation = (Quaternion)stream.ReceiveNext();
            }
        }

        //���� ���α׷��� �����
    }
}