namespace MultiThreading
{
    internal class Program
    {
        const int MB = 1024;
        static void Main(string[] args)
        {
            #region 쓰레드 task 구현
            Barista barista1 = HireBarista("J");
            Barista barista2 = HireBarista("K");

            Thread t1 = new Thread(() =>
            {
                HireBarista("J")
                    .GoToWork()
                    .MakeRandomBeverage();

            }, 1 * MB);

            t1.Name = barista1.Name;
            t1.IsBackground = true;
            t1.Start();
            t1.Join();


            Thread.Sleep(1000);

            ThreadPool.SetMinThreads(1, 0);
            ThreadPool.SetMaxThreads(4, 4); 
            #endregion
            //요즘에는 이렇게 안씀 c#은 생산성

            Task task1 = new Task(() =>
            {
                HireBarista("J")
                    .GoToWork()
                    .MakeRandomBeverage();
            });
            task1.Start();
            task1.Wait(); //쓰레드를 기다리는 함수

            //10개를 만들고 동시에 관리할 수 있다
            Task[] tasks = new Task[10];


            //위 Task의 복잡한 과정을 간단하게 표현한방법은 async 및 await를 사용한 비동기 프로그램이 있다.



            // 멀티쓰레드 환경에서는 태스크를 할당하고 실행한 순서대로 진행된다는 보장이 없다..!
            for (int i = 0; i < tasks.Length; i++)
            {
                int index = i; //i 같은 숫자로 나온다
                tasks[i] = new Task(() =>
                {
                    HireBarista($"Barista{index}")
                        .GoToWork()
                        .MakeRandomBeverage();
                });
                tasks[i].Start();
            }
            
            Task.WaitAll(tasks);//모든 테스크들이 완료할때까지 기다림
        }

        static Barista HireBarista(string nickname)
        {
            return new Barista(nickname);
        }
    }

    public enum Beverage
    {
        None,
        Aspresso,
        Latte,
        Lemonade,
    }

    public class Barista
    {
        public Barista(string name)
        {
            Name = name;
            _random = new Random();
        }

        private Random _random;

        public string Name { get; private set; }

        public Barista GoToWork()
        {
            Console.WriteLine($"바리스타 {Name} 이 출근했습니다...");
            return this;
        }

        public Beverage MakeRandomBeverage()
        {
            Beverage beverage = (Beverage) _random.Next(1, Enum.GetValues(typeof(Beverage)).Length);
            Console.WriteLine($"바리스타 {Name} 은 음료 {beverage} 를 제조했습니다.");
            return beverage;
        }
    }
}