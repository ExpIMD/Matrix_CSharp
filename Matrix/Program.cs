namespace IMD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, world!");
            foreach (string arg in args) Console.WriteLine(arg);

            List<List<int>> llst = new List<List<int>>()
            {
                new List<int> { 1, 0, 7 },
                new List<int> {2, 0, 6 },
                new List<int>{3, 4, 5 },
                new List<int>{0, 3, 0 },
                new List<int>{9, 0, 20 }
            };

            Matrix<int> a = MatrixMethods.ToMatrix(llst);
            Console.WriteLine("Max sum: " + MatrixMethods.GetMaxGold(a));
            foreach (var x in MatrixMethods.GetMaxGoldPath(a))
                Console.Write(x + "-> ");

            Console.ReadLine();
        }
    }
}