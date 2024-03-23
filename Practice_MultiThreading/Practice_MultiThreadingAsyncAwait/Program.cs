namespace Practice_MultiThreadingAsyncAwait
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
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

        private static Dictionary<Beverage, int> s_delayTimes = new Dictionary<Beverage, int>()
        {
            {Beverage.Aspresso, 1000 },
            {Beverage.Latte, 2000 },
            {Beverage.Lemonade, 3000 },
        };

        public Barista GoToWork()
        {
            Console.WriteLine($"바리스타 {Name} 이 출근했습니다...");
            return this;
        }

        //반환 타입이 있으면 제너릭 생성자로 작성하고 아니면 Task로 작성하면 된다.
        public async Task<Beverage> MakeRandomBeverage()
        {
            Beverage beverage = (Beverage)_random.Next(1, Enum.GetValues(typeof(Beverage)).Length);
            Console.WriteLine($"바리스타 {Name}은 음료 {beverage}를 제조했습니다.");

            /*Thread.Sleep(s_delayTimes[beverage]);

            Task delayTasks = new Task(() =>
            {
                Thread.Sleep(s_delayTimes[beverage]);
            });


            Task delayTask = Delay(s_delayTimes[beverage]);
            delayTask.Start();
            delayTask.Wait();*/

            await Task.Delay(s_delayTimes[beverage]);

            Console.WriteLine($"바리스타 {Name}은 음료 {beverage}를 제조를 완료했습니다..");
            return beverage;
        }

        private Task Delay(int milliseconds)
        {
            return new Task(() =>
            {
                Thread.Sleep( milliseconds );
            });
        }
    }
}
