namespace IMD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, world!");
            foreach (string arg in args) Console.WriteLine(arg);

            Matrix<double> A = new Matrix<double>(new double[,] {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 },
                { 13, 14, 15, 16 } });

            Matrix<double> b = new Matrix<double>(new double[,] {
                { 1 },
                { 0 },
                { -9 },
                { 5 } });

            IMD.MatrixMethods.PrintLine(A, Console.Out);

            Console.Write("Inner spiral order: ");

            IMD.MatrixMethods.InnerSpiralOrder(A, x => Console.Write(x + " "));

            Console.WriteLine();

            Console.Write("Outer spiral order: ");

            IMD.MatrixMethods.OuterSpiralOrder(A, x => Console.Write(x + " "));

            Console.WriteLine();

            Console.Write("Diagonal bottom up order: ");

            IMD.MatrixMethods.DiagonalOrderBottomUp(A, x => Console.Write(x + " "));

            Console.WriteLine();

            Console.Write("Diagonal top down order: ");

            IMD.MatrixMethods.DiagonalOrderTopDown(A, x => Console.Write(x + " "));

            Console.WriteLine();

            Console.Write("Diagonal with alternation order: ");

            IMD.MatrixMethods.DiagonalOrderWithAlternation(A, x => Console.Write(x + " "));

            Console.WriteLine();

            Console.Write("Gauss solution: ");

            IMD.MatrixMethods.GetGaussSolution(A, b).Show(Console.Out);
        }
    }
}