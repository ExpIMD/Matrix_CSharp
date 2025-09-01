namespace IMD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, world!");
            foreach (string arg in args) Console.WriteLine(arg);

            Matrix<double> A = new Matrix<double>(new double[,] {
                {1, 2, 3, 4 },
                {5, 6, 7, 8 },
                {9, 10, 11, 12 },
            {13, 14, 15, 16 } });
            Matrix<double> b = new Matrix<double>(new double[,] {
                {1 },
                {0 },
                {-9 }});
            Matrix<double> x = new Matrix<double>(new double[,] {
                { 0 },
                { 1 },
                { 0 }});

            IMD.MatrixMethods.InnerSpiralOrder(A, (x) => Console.Write(x + " "));
        }
    }
}