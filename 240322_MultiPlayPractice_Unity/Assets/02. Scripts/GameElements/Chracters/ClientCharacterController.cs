using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MP.GameElements
{
    [RequireComponent(typeof(PhotonView), typeof(PhotonTransformView))]
    public class ClientCharacterController : MonoBehaviour
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
    }
}