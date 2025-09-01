namespace IMD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, world!");
            foreach (string arg in args) Console.WriteLine(arg);

            Matrix<int> a = new Matrix<int>(3, 4, (i, j) => (i + j*2));
            Matrix<int> b = (Matrix<int>)a.Clone();

            a *= 2;

            MatrixMethods.PrintLine(a, Console.Out);
            Console.WriteLine("---------------");
            MatrixMethods.PrintLine(b, Console.Out);
            Console.WriteLine("---------------");
            Console.WriteLine(a != b);

            Console.ReadLine();
        }
    }
}