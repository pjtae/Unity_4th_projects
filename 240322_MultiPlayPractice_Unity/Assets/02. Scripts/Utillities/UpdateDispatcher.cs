using MP.Singleton;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace MP.Utilities
{
    /// <summary>
    /// ��Ƽ������ ȯ�濡�� UI ���� ���� UI ������Ʈ �����忡�� ����Ǿ�߸� �ϴµ�,
    /// Uinty �� GameLogic �ֱ⿡�� UI ����ȣ���� �ؾ���. UI �Ӹ��ƴ϶� ���̾��Ű�� �ִ� ���� ������Ʈ��
    /// ������Ʈ�鿡 ���� ������ �Ҷ��� GameLogic ���� ó���ؾ��ϹǷ� Queue�� �Լ��� �����س��� Update �ֱ⿡ ������.
    /// </summary>
    public class UpdateDispatcher : SingletonMonoBase<UpdateDispatcher>
    {
        private ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

        private void Update()
        {
            while (_actions.TryDequeue(out Action action))
            {
                action?.Invoke();
            }
        }

        public void Enqueue(Action action)
        {
            _actions.Enqueue(action);
        }
    }
}
