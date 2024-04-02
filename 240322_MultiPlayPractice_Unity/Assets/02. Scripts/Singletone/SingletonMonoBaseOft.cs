using System;
using UnityEngine;

namespace MP.Singleton
{
    public abstract class SingletonMonoBase<T> : MonoBehaviour
        where T : SingletonMonoBase<T> //제한을 걸어야한다
    {
        //빈깡통을 만들어서 게임오브젝트를 부착함
        public static T instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }

                return s_instance;
            }
        }

        private static T s_instance;


        protected virtual void Awake()
        {
            if (s_instance == null)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = (T)this;
        }
    }

    //인스펙터에서 추가하던 addcomponent 구현
}
