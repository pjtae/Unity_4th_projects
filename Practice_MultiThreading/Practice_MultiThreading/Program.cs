namespace Practice_MultiThreading
{
    internal class Program
    {
        const int MB = 1024 * 1024;
        static void Main(string[] args)
        {
            //쓰레드 구현하는 방법
            Barista barista1 = HireBarista("명진");
            Barista barista2 = HireBarista("진수");

            Thread t1 = new Thread(() =>
            {
                HireBarista("명진")
                .GoToWork()
                .MakeRandomBeverage();
            }, 1 * MB);

            t1.Name = barista1.Name; //쓰레드 이름 지정
            t1.IsBackground = true; // 백그라운드 스레드로 설정
            t1.Start(); //스레드 실행
            t1.Join(); //스레드가 종료 될때까지 기다림

            Thread.Sleep(1000);

            ThreadPool.SetMinThreads(1, 0); //스레드 구성 
            ThreadPool.SetMaxThreads(3, 3);

            Task task1 = new Task(() =>
            {
                HireBarista("명진")
                .GoToWork()
                .MakeRandomBeverage();
            });
            task1.Start();
            task1.Wait();//쓰레드를 기다리는 함수

            Task[] tasks = new Task[10];

            for ( int i = 0; i < tasks.Length; i++ )
            {
                int index = i;
                tasks[i] = new Task(() =>
                {
                    HireBarista($"Barista{index}")
                        .GoToWork()
                        .MakeRandomBeverage();
                });
                tasks[i].Start();
            }

            Task.WaitAll( tasks );
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
            Beverage beverage = (Beverage)_random.Next(1, Enum.GetValues(typeof(Beverage)).Length);
            Console.WriteLine($"바리스타 {Name}은 음료 {beverage}를 제조했습니다.");
            return beverage;
        }

    }
}



