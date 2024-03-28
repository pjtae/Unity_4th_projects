using System;

namespace MP.Singleton
{
    internal abstract class SingletonBase<T>
        where T : SingletonBase<T> //제한을 걸어야한다
    {
        //사용할 때 : 필요할때 만들어서 제공하기 위해서
        public static T instance
        {
            get
            {
                if (s_instance == null)
                {
                    //ConstructorInfo consturcutorInfo = typeof(T).GetConstructor(new Type[] { });
                    //consturcutorInfo.Invoke(null);

                    s_instance = (T)Activator.CreateInstance(typeof(T));
                }

                return s_instance;
            }
        }

        private static T s_instance;
    }
}
