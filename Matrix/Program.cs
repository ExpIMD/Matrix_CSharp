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
                new List<int> { 1, 1, 1 },
                new List<int> { 1, 0, 1 },
                new List<int> { 1, 1, 1 },
            };

            Matrix<int> a = MatrixMethods.ToMatrix(llst);
            foreach (var x in MatrixMethods.DFSGetCycle(a))
                Console.Write(x + "-> ");
;
            Console.ReadLine();
        }
    }
}