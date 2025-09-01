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
                new List<int> { 1, 2, 3 },
                new List<int> { 4, 5, 6 },
                new List<int> { 7, 8, 9 },
            };

            Matrix<int> a = MatrixMethods.ToMatrix(llst);
            Matrix<int> b = MatrixMethods.GetTranspose(a);
            MatrixMethods.PrintLine(b, Console.Out);
            MatrixMethods.PrintLine(a, Console.Out);

            Console.WriteLine(MatrixMethods.Determinant(a));

            Console.ReadLine();
        }
    }
}