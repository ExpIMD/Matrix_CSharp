namespace IMD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, world!");
            foreach (string arg in args) Console.WriteLine(arg);

            List<List<short>> llst = new List<List<short>>()
            {
                new List<short> { 1, 0, 7 },
                new List<short> {2, 0, 6 },
                new List<short>{3, 4, 5 },
                new List<short>{0, 3, 0 },
                new List<short>{9, 0, 20 }
            };

            Matrix<short> a = MatrixMethods.ToMatrix(llst);
            Console.WriteLine("Max sum: " + MatrixMethods.GetMaxGold(a));

            foreach (var x in MatrixMethods.GetMaxGoldPath(a))
                Console.Write(x + "-> ");

            Console.ReadLine();
        }
    }
}