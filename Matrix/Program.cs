namespace IMD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, world!");
            foreach (string arg in args) Console.WriteLine(arg);

            Matrix<double> A = new Matrix<double>(new double[,] {
                {1, 0, 3 },
                {9, -1, 2 },
                {2, 1, 0 }});
            Matrix<double> b = new Matrix<double>(new double[,] {
                {1 },
                {0 },
                {-9 }});
            Matrix<double> x = new Matrix<double>(new double[,] {
                { 0 },
                {-9 },
                {  0} });

            MatrixMethods.PrintLine(A * MatrixMethods.GetGaussSolution(A, b).GetSolution(), Console.Out);

            Console.ReadLine();
        }
    }
}