using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MP.UI
{
    /// <summary>
    /// Canvas ������ �⺻ �������̽�
    /// </summary>
    public interface IUI
    {
        /// <summary>
        /// Canvas �� ���� ����
        /// </summary>
        int sortingOrder { get; set; }

        /// <summary>
        /// ������ �Է� ��ȣ�ۿ� ���� ����
        /// </summary>
        bool inputActionEnable { get; set; }


        /// <summary>
        /// ������ �Է� ��ȣ�ۿ� ���� ���ΰ� �ٲ������
        /// </summary>
        event Action<bool> onInputActionEnableChanged;


        /// <summary>
        /// Canvas Ȱ��ȭ
        /// </summary>
        void Show();

        /// <summary>
        /// Canvas ��Ȱ��ȭ
        /// </summary>
        void Hide();

        /// <summary>
        /// �������� ��ȣ�ۿ�
        /// </summary>
        void InputAction();

        /// <summary>
        /// �� Canvas ���� � RaycastTarget�� �����ϱ� ���� ���
        /// </summary>
        /// <param name="results">��� ��ȯ�� ����</param>
        /// <returns>������ Ÿ���� ������ true, ������ false </returns>
        bool Raycast(List<RaycastResult> results);

        //buffer ������ ���������� �������ֱ� ���� ��ü
    }
}
