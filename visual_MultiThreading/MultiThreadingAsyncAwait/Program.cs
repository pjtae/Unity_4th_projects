namespace MultiThreadingAsyncAwait
{
    internal class Program
    {
        public readonly static object Lock = new object();// 
        public static int Ticks;
        
        // 어떤 Task 를 취소시키기 위해서 신호를 보내는 원본 객체.
        // Task 할당 시에. 이 Source를 통해 Token 을 발행해서 줄 수 있고,
        // 이 Source의 Cancel() 요청이 발생했을 때, Token을 발행받은 모든 Task를 취소시킬 수 있다.
        static CancellationTokenSource _cts;

        static void Main(string[] args)
        {
            Task t1 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Run dummy...");  
            }, _cts.Token, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

            
            _cts.Cancel();

            if (t1.IsCanceled)
            {
                //todo => Do exception handling
            }

            //쓰레드풀의 작업 할당을 위한 대기열에 등록
            Task.Run(() =>
            {
                Console.WriteLine("Run dummy...");
            });


            // _ 는 반환 내용에 대해 무시하겠다는 명시
            _ = HireBarista("J")
                            .GoToWork()
                            .MakeRandomBeverage();

            // c# 버전 등의 이슈로 async를 사용할 수 없는 함수에서는 직접 Task 참조를 통해 wait 등을 수행해야한다.
            Task<Beverage> task = HireBarista("J")
                            .GoToWork()
                            .MakeRandomBeverage();
            task.Wait();


            Task[] tasks = new Task[10];
            for (int i = 0; i < tasks.Length; i++)
            {
                int index = i;
                tasks[i] = HireBarista("PK")
                            .GoToWork()
                            .MakeRandomBeverage();
            }
            Task.WaitAll(tasks);


            //뒤에 내용이 있으면 그대로 실행하게 된다
            Console.WriteLine("안녕?");

            ++Ticks;
            Ticks = Ticks + 1;
            Ticks++;
            PPAF(ref Ticks);
        }

        public static int PPAF(ref int value)
        {
            int origin = value;
            ++value;
            return origin;
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

        public string Name { get; private set; }

        private static Dictionary<Beverage, int> s_delayTimes = new Dictionary<Beverage, int>()
        {
            { Beverage.Aspresso, 1000},
            { Beverage.Latte, 2000},
            { Beverage.Lemonade, 3000},

        };

        private Random _random;

        public Barista GoToWork()
        {
            Console.WriteLine($"바리스타 {Name} 이 출근했습니다...");
            return this;
        }

        public async Task<Beverage> MakeRandomBeverage()//반환할 타입이 있으면 제너릭 아니면 Task로 작성하면된다.
        {
            Beverage beverage = (Beverage)_random.Next(1, Enum.GetValues(typeof(Beverage)).Length);
            Console.WriteLine($"바리스타 {Name} 은 음료 {beverage} 를 제조를 시작했습니다.");
            //기다리는 로직작성

            //Thread.Sleep(s_delayTimes[beverage]); 차이 그냥 재우기

            /*Task delayTask = new Task(() =>//차이 새로운 스레드를 만들어서 재우기
            {
                Thread.Sleep(s_delayTimes[beverage]);
            });*/

            /*Task delayTask = Delay(s_delayTimes[beverage]);
            delayTask.Start();
            delayTask.Wait();*/

            


            await Task.Delay(s_delayTimes[beverage]);

            #region Lock 키워드
            //Lock 키워드 현재 어플리케이션 내에서 둘이상의 쓰레드 접근을 막기위한 키워드
            lock (Program.Lock)
            {
                for (int i = 0; i < 1999999; i++)
                {
                    Program.Ticks++;
                }
            }

            lock (Program.Lock)
            {
                for (int i = 0; i < 1999999; i++)
                {
                    Interlocked.Increment(ref Program.Ticks);
                }
            }
            #endregion


            // 감시 시작
            Monitor.Enter(Program.Lock);

            // Critical Section (임계영역) : 둘이상의 쓰레드가 접근하면 안되는 공유 자원에 접근하는 영역
            #region Critical Section 시작
            for (int i = 0; i < 100000; i++)
            {
                Program.Ticks++;

            } 
            #endregion CriticalSection 끝

            //await Delay(s_delayTimes[beverage]); //위 내용을 줄여주는 키워드 await를 사용하기 위해서는 async를 사용함
            Monitor.Exit(Program.Lock);//감시 끝

            Semaphore pool = new Semaphore(0, 3);

            pool.WaitOne();//한자리 날때까지 기다림
            //Critical Section 작성
            pool.Release();//해방 점유하고 이쓴거 움


            #region Mutex
            Mutex mutex = new Mutex();
            mutex.WaitOne();
            //크리티컬 섹션 작성
            mutex.ReleaseMutex();

            #endregion

            Console.WriteLine($"바리스타 {Name} 은 음료 {beverage} 를 제조를 완료했습니다.");
            return beverage;
        }

        private Task Delay(int milliseconds)
        {
            return new Task(() =>
            {
                Thread.Sleep(milliseconds);
            });
        }
    }
}