using System.Collections;
using System.Numerics;
using System.Text;

namespace IMD
{
    public class ArgumentEmptyException : ArgumentException
    {
        public ArgumentEmptyException(string message, string paramName) : base(message, paramName) { }
    }
    public class ArgumentWrongSizeException : ArgumentException
    {
        public ArgumentWrongSizeException(string message, string paramName) : base(message, paramName) { }
    }
    public class ArgumentNoSolutionException : ArgumentException
    {
        public ArgumentNoSolutionException(string message) : base(message) { }
    }
    public class ArgumentSingularMatrixException : ArgumentException
    {
        public ArgumentSingularMatrixException(string message, string paramName) : base(message, paramName) { }
    }
    /// <summary>
    /// The class of numerical matrices
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Matrix<T> : ICloneable, IEnumerable<T> where T : IComparable<T>, INumber<T>
    {
        private T[,] __data;
        public int Rows
        {
            get => __data.GetLength(0);
        }
        public int Cols
        {
            get => __data.GetLength(1);
        }
        public Tuple<int, int> Dimenshion
        {
            get => new Tuple<int, int>(Rows, Cols);
        }
        public int Size
        {
            get => Rows * Cols;
        }

        public T this[int row, int col]
        {
            get
            {
                CheckBounds(row, col);
                return __data[row, col];
            }
            set
            {
                CheckBounds(row, col);
                __data[row, col] = value;
            }
        }
        public static Matrix<T> operator +(Matrix<T> a, Matrix<T> b)
        {
            if (a is null) throw new ArgumentNullException("The matrix is null", nameof(a));
            if (b is null) throw new ArgumentNullException("The matrix is null", nameof(b));

            int rows1 = a.Rows, cols1 = a.Cols, rows2 = b.Rows, cols2 = b.Cols;

            if (rows1 != rows2 || cols1 != cols2) throw new ArgumentWrongSizeException("The matrices have different sizes", nameof(a) + ", " + nameof(b));

            Matrix<T> result = new Matrix<T>(rows1, cols1);

            for (int i = 0; i < rows1; ++i)
                for (int j = 0; j < cols1; ++j)
                    result[i, j] = a[i, j] + b[i, j];

            return result;
        }
        public static Matrix<T> operator -(Matrix<T> a, Matrix<T> b)
        {
            if (a is null) throw new ArgumentNullException("The matrix is null", nameof(a));
            if (b is null) throw new ArgumentNullException("The matrix is null", nameof(b));

            int rows1 = a.Rows, cols1 = a.Cols, rows2 = b.Rows, cols2 = b.Cols;

            if (rows1 != rows2 || cols1 != cols2) throw new ArgumentWrongSizeException("The matrices have different sizes", nameof(a) + ", " + nameof(b));

            Matrix<T> result = new Matrix<T>(rows1, cols1);

            for (int i = 0; i < rows1; ++i)
                for (int j = 0; j < cols1; ++j)
                    result[i, j] = a[i, j] - b[i, j];

            return result;
        }
        public static Matrix<T> operator *(Matrix<T> a, T value)
        {
            if (a is null) throw new ArgumentNullException("The matrix is null", nameof(a));

            int rows = a.Rows, cols = a.Cols;

            Matrix<T> result = new Matrix<T>(rows, cols);

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    result[i, j] = a[i, j] * value;

            return result;
        }
        public static Matrix<T> operator *(Matrix<T> a, Matrix<T> b)
        {
            if (a is null) throw new ArgumentNullException("The matrix is null", nameof(a));
            if (b is null) throw new ArgumentNullException("The matrix is null", nameof(b));

            int rows1 = a.Rows, cols1 = a.Cols, rows2 = b.Rows, cols2 = b.Cols;

            if (cols1 != rows2) throw new ArgumentWrongSizeException("The matrices have wrong sizes", nameof(a) + ", " + nameof(b));

            Matrix<T> result = new Matrix<T>(rows1, cols2);

            for (int i = 0; i < rows1; ++i)
            {
                for (int j = 0; j < cols2; ++j)
                {
                    T temp = default(T);

                    for (int k = 0; k < cols1; ++k)
                        temp += a[i, k] * b[k, j];

                    result[i, j] = temp;
                }
            }

            return result;
        }
        public static bool operator ==(Matrix<T> a, Matrix<T> b)
        {
            if (a is null || b is null) return false;
            if (ReferenceEquals(a, b)) return true;

            int rows1 = a.Rows, cols1 = a.Cols, rows2 = b.Rows, cols2 = b.Cols;

            if (rows1 != rows2 || cols1 != cols2) throw new ArgumentWrongSizeException("The matrices have different sizes", nameof(a) + ", " + nameof(b));

            for (int i = 0; i < rows1; ++i)
                for (int j = 0; j < cols1; ++j)
                    if (a[i, j] != b[i, j])
                        return false;

            return true;
        }
        public static bool operator !=(Matrix<T> a, Matrix<T> b)
        {
            return !(a == b);
        }

        public static Matrix<T> Identity(int size)
        {
            Matrix<T> result = new Matrix<T>(size, size);

            for (int i = 0; i < size; ++i)
                for (int j = 0; j < size; ++j)
                    if (i == j)
                        result[i, j] = (T)Convert.ChangeType(1, typeof(T));

            return result;
        }
        public static Matrix<T> Zero(int rows, int cols)
        {
            return new Matrix<T>(rows, cols, default(T));
        }

        public Matrix(int rows, int cols, T value = default(T))
        {
            if (rows < 0) throw new ArgumentOutOfRangeException("The number of rows is negative", nameof(rows));
            if (cols < 0) throw new ArgumentOutOfRangeException("The number of rows is negative", nameof(cols));
            __data = new T[rows, cols];

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    __data[i, j] = value;
        }
        public Matrix(int rows, int cols, Func<int, int, T> F)
        {
            if (rows < 0) throw new ArgumentOutOfRangeException("The number of rows is negative", nameof(rows));
            if (cols < 0) throw new ArgumentOutOfRangeException("The number of rows is negative", nameof(cols));
            if (F is null) throw new ArgumentNullException("The function is null", nameof(F));

            __data = new T[rows, cols];

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    __data[i, j] = F(i, j);
        }
        public Matrix(T[,] data)
        {
            if (data is null) throw new ArgumentNullException("The data is null", nameof(data));

            int rows = data.GetLength(0), cols = data.GetLength(1);
            __data = new T[rows, cols];

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    __data[i, j] = data[i, j];
        }
        public Matrix(Matrix<T> other)
        {
            if (other is null) throw new ArgumentNullException("The matrix is null", nameof(other));

            int rows = other.Rows, cols = other.Cols;
            __data = new T[rows, cols];

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    __data[i, j] = other[i, j];
        }

        public bool IsEmpty()
        {
            return __data is null;
        }
        public bool IsSquare()
        {
            if (IsEmpty()) return false;
            return Rows == Cols;
        }
        public int Count()
        {
            return Size;
        }

        public object Clone()
        {
            int rows = Rows, cols = Cols;
            Matrix<T> result = new Matrix<T>(rows, cols);

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    result[i, j] = __data[i, j];

            return result;
        }
        public override bool Equals(object? obj)
        {
            if (obj is null) throw new ArgumentNullException("The object is null", nameof(obj));
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not Matrix<T> temp) return false;

            int rows1 = Rows, cols1 = Cols, rows2 = temp.Rows, cols2 = temp.Cols;

            if (rows1 != rows2 || cols1 != cols2) return false;

            for (int i = 0; i < rows1; ++i)
                for (int j = 0; j < cols1; ++j)
                    if (!EqualityComparer<T>.Default.Equals(__data[i, j], temp.__data[i, j]))
                        return false;

            return true;
        }
        public override int GetHashCode()
        {
            int result = 17, rows = Rows, cols = Cols;

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    result = result * 31 + EqualityComparer<T>.Default.GetHashCode(__data[i, j]);

            return result;
        }
        public IEnumerator<T> GetEnumerator()
        {
            int rows = Rows, cols = Cols;

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    yield return __data[i, j];
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void CheckBounds(int row, int col)
        {
            if (row < 0 || row >= Rows) throw new ArgumentOutOfRangeException("The row index is out of bounds", nameof(row));
            if (col < 0 || col >= Cols) throw new ArgumentOutOfRangeException("The col index is out of bounds", nameof(col));
        }
    }

    // The class describing the solution of a linear linear equation using the Gauss method
    public class GaussSolution<T>
    {
        private bool __hasSolution = false;
        private bool __hasUniqueSolution = false;

        private List<double> __particularSolution = new List<double>(); // Private solution
        private List<string> __expressions = new List<string>(); // Formulas for each variable

        public GaussSolution() { }

        // Printing the solution to the textwritter 'tw' without moving to a new line
        public void Print(TextWriter tw)
        {
            if (tw is null) throw new ArgumentNullException("The textwritter is null", nameof(tw));

            if (!this.__hasSolution)
            {
                tw.WriteLine("The system doesn't have any solution");
                return;
            }

            if (this.__hasUniqueSolution)
            {
                tw.WriteLine("The system has only one solution");
                tw.Write("Solution: (");

                for (int i = 0; i < this.__particularSolution.Count; ++i)
                {
                    tw.Write(this.__particularSolution[i]);

                    if (i < this.__particularSolution.Count - 1) tw.Write(", ");
                }

                tw.WriteLine(")");
            }
            else
            {
                tw.WriteLine("The system has infinitely number of solutions");
                tw.Write("Particular solution: (");

                for (int i = 0; i < this.__particularSolution.Count; ++i)
                {
                    tw.Write(this.__particularSolution[i]);

                    if (i < this.__particularSolution.Count - 1)  tw.Write(", ");
                }

                tw.WriteLine(")");
                tw.WriteLine("General solution:");

                foreach (var expr in this.__expressions)
                    tw.WriteLine(expr);
            }
        }
        // Printing the solution to the textwritter 'tw' with moving to a new line
        public void PrintLine(TextWriter stream)
        {
            Print(stream);
            stream.WriteLine();
        }
        // Installation solution
        public void SetSolution(bool hasSolution, bool hasUniqueSolution, List<double> particularSolution, List<string> expressions)
        {
            this.__hasSolution = hasSolution;
            this.__hasUniqueSolution = hasUniqueSolution;
            this.__particularSolution = particularSolution;
            this.__expressions = expressions;
        }
        public Matrix<double> GetSolution()
        {
            if (!this.__hasSolution) throw new ArgumentNoSolutionException("The system doesn't have any solution");
            return new Matrix<double>(this.__particularSolution.Count, 1, (i, j) => this.__particularSolution[i]);
        }
    }

    /// <summary>
    /// The static class that includes methods for numeric matrices
    /// </summary>
    public static class MatrixMethods
    {
        /// Printing methods

        public static void Print<T>(Matrix<T> mrx, TextWriter tw, string sep = " ") where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            if (tw is null) throw new ArgumentNullException("The textwriter is null", nameof(tw));

            if (sep is null) throw new ArgumentNullException("The separator is null", nameof(sep));

            int rows = mrx.Rows, cols = mrx.Cols;

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    tw.Write(mrx[i, j]);
                    if (j < cols - 1) tw.Write(sep);
                }
                if (i < rows - 1) tw.WriteLine();
            }
        }
        public static void PrintLine<T>(Matrix<T> mrx, TextWriter tw, string sep = " ") where T : IComparable<T>, INumber<T>
        {
            Print(mrx, tw, sep);
            tw.WriteLine();
        }

        /// Ordering methods

        // Traversal from the upper left corner of the matrix 'mrx' along the diagonals with alternating directions (top to bottom and bottom to top) applying action 'A' to each element
        public static void DiagonalOrderWithAlternation<T>(Matrix<T> mrx, Action<T> A) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (A is null) throw new ArgumentNullException("The action is null", nameof(A));

            if (mrx.IsEmpty()) return;

            int rows = mrx.Rows, cols = mrx.Cols, currentRow = 0, currentCol = 0;

            while (currentRow < rows && currentCol < cols)
            {
                A(mrx[currentRow, currentCol]);

                if ((currentRow + currentCol) % 2 == 0)
                {
                    if (currentCol == cols - 1) ++currentRow;
                    else if (currentRow == 0) ++currentCol;
                    else
                    {
                        --currentRow;
                        ++currentCol;
                    }
                }
                else
                {
                    if (currentRow == rows - 1) ++currentCol;
                    else if (currentCol == 0) ++currentRow;
                    else
                    {
                        ++currentRow;
                        --currentCol;
                    }
                }
            }
        }
        // Traversing the matrix 'mrx' from the top left corner along the diagonals from bottom to top, applying action 'A' to each element
        public static void DiagonalOrderBottomUp<T>(Matrix<T> mrx, Action<T> A) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (A is null) throw new ArgumentNullException("The action is null", nameof(A));

            if (mrx.IsEmpty()) return;

            int rows = mrx.Rows, cols = mrx.Cols;

            for (int d = 0; d < rows + cols - 1; ++d)
            {
                int currentRow = d < rows ? d : rows - 1;
                int currentCol = d - currentRow;

                while (currentRow >= 0 && currentCol < cols)
                {
                    A(mrx[currentRow, currentCol]);

                    if (currentRow == 0) break;

                    --currentRow;
                    ++currentCol;
                }
            }
        }
        // Traversing the matrix 'mrx' from the upper left corner along the diagonals from top to bottom, applying action 'A' to each element
        public static void DiagonalOrderTopDown<T>(Matrix<T> mrx, Action<T> A) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (A is null) throw new ArgumentNullException("The action is null", nameof(A));

            if (mrx.IsEmpty()) return;

            int rows = mrx.Rows, cols = mrx.Cols;

            for (int d = 0; d < rows + cols - 1; ++d)
            {
                int currentCol = d < cols ? d : cols - 1;
                int currentRow = d - currentCol;

                while (currentRow < rows && currentCol >= 0)
                {
                    A(mrx[currentRow, currentCol]);
                    ++currentRow;

                    if (currentCol == 0) break;

                    --currentCol;
                }
            }
        }
        // Traversing the matrix 'mrx' from the top left corner in a spiral from the outside in, applying action 'A' to each element
        public static void OuterSpiralOrder<T>(Matrix<T> mrx, Action<T> A) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (A is null) throw new ArgumentNullException("The action is null", nameof(A));

            if (mrx.IsEmpty()) return;

            int rows = mrx.Rows, cols = mrx.Cols;
            int iStart = 0, jStart = 0;
            int iEnd = rows, jEnd = cols;

            while (iStart < iEnd && jStart < jEnd)
            {
                for (int k = jStart; k < jEnd; ++k)
                    A(mrx[iStart, k]);
                ++iStart;

                for (int k = iStart; k < iEnd; ++k)
                    A(mrx[k, jEnd - 1]);
                --jEnd;

                if (iStart >= iEnd || jStart >= jEnd) break;

                for (int k = jEnd - 1; k >= jStart; --k)
                    A(mrx[iEnd - 1, k]);
                --iEnd;

                for (int k = iEnd - 1; k >= iStart; --k)
                    A(mrx[k, jStart]);
                ++jStart;
            }
        }
        // Traversing the matrix 'mrx' from the center in a spiral from the inside out, applying action 'A' to each element
        public static void InnerSpiralOrder<T>(Matrix<T> mrx, Action<T> A) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (A is null) throw new ArgumentNullException("The action is null", nameof(A));

            if (mrx.IsEmpty()) return;

            int rows = mrx.Rows, cols = mrx.Cols;

            int centerRow = (rows - 1) / 2, centerCol = (cols - 1) / 2;

            A(mrx[centerRow, centerCol]);

            int step = 1; // Step length in current direction
            int dirIndex = 0;
            int r = centerRow, c = centerCol;

            int totalElements = rows * cols, count = 1;

            while (count < totalElements)
            {
                // We go step by step twice in a row, changing direction
                for (int repeat = 0; repeat < 2; ++repeat)
                {
                    int dr = IMD.Constants.DIRECTIONS_WITHOUT_DIAGONAL[dirIndex][0], dc = IMD.Constants.DIRECTIONS_WITHOUT_DIAGONAL[dirIndex][1];

                    for (int i = 0; i < step; ++i)
                    {
                        r += dr;
                        c += dc;

                        if (r >= 0 && r < rows && c >= 0 && c < cols)
                        {
                            A(mrx[r, c]);
                            ++count;

                            if (count >= totalElements) return;
                        }
                    }

                    dirIndex = (dirIndex + 1) % 4;
                }

                ++step;
            }
        }

        /// Conversion methods

        // Returns a matrix obtained from the rectangular full container of containers 'ccs'
        public static Matrix<T> ToMatrix<T>(IEnumerable<IEnumerable<T>> ccs) where T : IComparable<T>, INumber<T>
        {
            if (ccs is null) throw new ArgumentNullException("The container of containers is null", nameof(ccs));

            if (ccs.Count() == 0) return new Matrix<T>(0, 0);

            int rows = ccs.Count(), cols = ccs.First().Count(), i = 0;
            Matrix<T> result = new Matrix<T>(rows, cols);

            foreach (var c in ccs)
            {
                if (c.Count() != cols) throw new ArgumentException("The container of containers isn't full", nameof(ccs));

                int j = 0;

                foreach (var x in c)
                {
                    result[i, j] = x;
                    ++j;
                }
                ++i;
            }

            return result;
        }
        // Returns a list of lists obtained from the matrix 'mrx'
        public static List<List<T>> ToList<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));

            if (mrx.IsEmpty()) return new List<List<T>>(0);

            int rows = mrx.Rows, cols = mrx.Cols;
            List<List<T>> result = new List<List<T>>(rows);

            for (int i = 0; i < rows; ++i)
            {
                var row = new List<T>(cols);

                for (int j = 0; j < cols; ++j)
                    row.Add(mrx[i, j]);

                result.Add(row);
            }

            return result;
        }

        /// Math methods

        // Returns a submatrix obtained from the matrix 'mrx' by excluding the row with index 'excludedRow' and the column with index 'excludedCol'
        public static Matrix<T> GetSubMatrix<T>(Matrix<T> mrx, int excludedRow, int excludedCol) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols, subRow = 0;

            if (excludedRow < 0 || excludedRow >= rows) throw new ArgumentOutOfRangeException("The excluded row index is out of bounds", nameof(excludedRow));
            if (excludedCol < 0 || excludedCol >= rows) throw new ArgumentOutOfRangeException("The excluded col index is out of bounds", nameof(excludedCol));


            Matrix<T> subMatrix = new Matrix<T>(rows - 1, cols - 1);

            for (int i = 0; i < rows; ++i)
            {
                if (i == excludedRow) continue;

                int subCol = 0;

                for (int j = 0; j < cols; ++j)
                {
                    if (j == excludedCol) continue;

                    subMatrix[subRow, subCol] = mrx[i, j];
                    ++subCol;
                }
                ++subRow;
            }

            return subMatrix;
        }

        // Returns the determinant of the square matrix 'mrx'
        public static T Determinant<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));
            if (!mrx.IsSquare()) throw new ArgumentWrongSizeException("The matrix isn't square", nameof(mrx));

            int size = mrx.Rows;

            if (size == 1)
                return mrx[0, 0];
            if (size == 2)
                return mrx[0, 0] * mrx[1, 1] - mrx[0, 1] * mrx[1, 0];
            if (size == 3)
                return mrx[0, 0] * (mrx[1, 1] * mrx[2, 2] - mrx[1, 2] * mrx[2, 1]) -
                       mrx[0, 1] * (mrx[1, 0] * mrx[2, 2] - mrx[1, 2] * mrx[2, 0]) +
                       mrx[0, 2] * (mrx[1, 0] * mrx[2, 1] - mrx[1, 1] * mrx[2, 0]);

            T result = default(T);
            for (int j = 0; j < size; ++j)
            {
                var subMatrix = GetSubMatrix(mrx, 0, j);
                var temp = mrx[0, j] * Determinant(subMatrix);
                result += (j % 2 == 0) ? temp : -temp;
            }

            return result;

        }
        // Returns the minor in the square matrix 'mrx' at the row with index 'row' and at the column with index 'col'
        public static T Minor<T>(Matrix<T> mrx, int row, int col) where T : IComparable<T>, INumber<T>
        {
            return Determinant(GetSubMatrix(mrx, row, col));
        }
        // Returns the algebraic complement of the square matrix 'mrx' at the row with index 'row' and at the column with index 'col'
        public static T Cofactor<T>(Matrix<T> mrx, int row, int col) where T : IComparable<T>, INumber<T>
        {
            T result = Minor(mrx, row, col);
            return ((row + col) % 2 == 0) ? result : -result;
        }
        // Returns the rank of the matrix 'mrx'
        public static int Rank<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols, result = 0;
            Matrix<double> temp = new Matrix<double>(rows, cols);

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    temp[i, j] = (dynamic)mrx[i, j];

            for (int row = 0, col = 0; row < rows && col < cols; ++col)
            {
                int pivotRow = row;

                while (pivotRow < rows && Math.Abs(temp[pivotRow, col]) < IMD.Constants.EPSILON) ++pivotRow; // Finding a non-zero element

                if (pivotRow == rows) continue;

                if (pivotRow != row) // We change the lines so that the main element is on the diagonal
                {
                    for (int j = col; j < cols; ++j)
                    {
                        var tempValue = temp[row, j];
                        temp[row, j] = temp[pivotRow, j];
                        temp[pivotRow, j] = tempValue;
                    }
                }

                var pivot = temp[row, col];

                if (Math.Abs(pivot) > IMD.Constants.EPSILON) // Normalizing the row
                    for (int j = col; j < cols; ++j)
                        temp[row, j] /= pivot;

                for (int i = row + 1; i < rows; ++i) // Subtract the current row from the rows below
                {
                    double factor = temp[i, col];

                    if (Math.Abs(factor) > IMD.Constants.EPSILON)
                        for (int j = col; j < cols; ++j)
                            temp[i, j] = temp[i, j] - factor * temp[row, j];
                }

                ++row;
                ++result;
            }

            return result;
        }

        // Returns the Frobenius norm of the matrix 'mrx'
        public static T FrobeniusNorm<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentWrongSizeException("The matrix is empty", nameof(mrx));

            T result = default(T);
            int rows = mrx.Rows, cols = mrx.Cols;

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    result += mrx[i, j] * mrx[i, j];

            return Math.Sqrt((dynamic)result);

        }
        // Returns the first order norm of the matrix 'mrx'
        public static T FirstNorm<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentException("The matrix is empty", nameof(mrx));

            T result = default(T);
            int rows = mrx.Rows, cols = mrx.Cols;

            for (int j = 0; j < rows; ++j)
            {
                T temp = default(T);

                for (int i = 0; i < cols; ++i)
                    temp += Math.Abs((dynamic)mrx[i, j]);

                if (temp > result) result = temp;
            }
            return result;
        }
        // Returns the infinite order norm of the matrix 'mrx'
        public static T InfinityNorm<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx == null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentException("The matrix is empty", nameof(mrx));

            T result = default(T);
            int rows = mrx.Rows, cols = mrx.Cols;

            for (int i = 0; i < rows; ++i)
            {
                T temp = default(T);

                for (int j = 0; j < cols; ++j)
                    temp += Math.Abs((dynamic)mrx[i, j]);

                if (temp > result) result = temp;
            }

            return result;
        }

        // Checks if the matrix 'mrx' is a Toeplitz matrix
        public static bool IsToeplitzMatrix<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;

            for (int i = 1; i < rows; ++i)
                for (int j = 1; j < cols; ++j)
                    if (!EqualityComparer<T>.Default.Equals(mrx[i, j], mrx[i - 1, j - 1]))
                        return false;

            return true;
        }

        // Returns the solution of the linear equation of the form 'Ax=b'
        public static GaussSolution<T> GetGaussSolution<T>(Matrix<T> A, Matrix<T> b) where T : IComparable<T>, INumber<T>
        {
            if (A is null) throw new ArgumentNullException("The matrix is null", nameof(A));
            if (b is null)  throw new ArgumentNullException("The matrix is null", nameof(b));
            if (A.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(A));
            if (b.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(b));

            int rows = A.Rows, cols = A.Cols;

            if (b.Rows != rows || b.Cols != 1) throw new ArgumentWrongSizeException("The dimensions of matrix 'A' and matrix 'b' are not consistent", nameof(A) + ", " + nameof(b));

            var result = new GaussSolution<T>();
            bool hasSolution = false, hasUniqueSolution = false;
            var aug = new Matrix<double>(rows, cols + 1); // The matrix of the form [A|b]

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                    aug[i, j] = (dynamic)A[i, j];

                aug[i, cols] = (dynamic)b[i, 0];
            }

            int rank = 0;
            var pivotCols = new List<int>(); // List of columns with pivots
            var freeCols = new List<int>();  // List of free columns (without pivot)
            var rowPivot = new int[cols]; // For each column, the index of the row with the pivot (-1 if none)

            for (int i = 0; i < cols; ++i)
                rowPivot[i] = -1;

            for (int col = 0; col < cols && rank < rows; ++col)
            {
                // Finding the row with the largest element in the current column (selecting the main element)

                int pivotRow = rank;
                double maxAbs = Math.Abs((dynamic)aug[pivotRow, col]);

                for (int i = rank + 1; i < rows; ++i)
                {
                    double value = Math.Abs((dynamic)aug[i, col]);

                    if (value > maxAbs)
                    {
                        maxAbs = value;
                        pivotRow = i;
                    }
                }

                if (maxAbs < IMD.Constants.EPSILON) continue; // If the maximum element is too small (almost zero), move to the next column

                for (int j = col; j <= cols; ++j) // Rearrange lines to move pivot to current position
                {
                    var tmp = aug[rank, j];
                    aug[rank, j] = aug[pivotRow, j];
                    aug[pivotRow, j] = tmp;
                }

                pivotCols.Add(col);
                rowPivot[col] = rank;
                double pivotValue = aug[rank, col];

                for (int j = col; j <= cols; ++j) // Normalizing the row
                    aug[rank, j] = aug[rank, j] / pivotValue;

                for (int i = 0; i < rows; ++i) // Zeroing out the elements in the current column in all other rows
                {
                    if (i != rank)
                    {
                        double factor = aug[i, col];

                        if (Math.Abs((dynamic)factor) > IMD.Constants.EPSILON)
                            for (int j = col; j <= cols; ++j)
                                aug[i, j] -= aug[rank, j] * factor;
                    }
                }

                ++rank;
            }

            for (int col = 0; col < cols; ++col)
                if (rowPivot[col] == -1)
                    freeCols.Add(col);

            for (int i = rank; i < rows; ++i) // Checking the system for incompatibility
            {
                if (Math.Abs((dynamic)aug[i, cols]) > IMD.Constants.EPSILON) // No solution
                {
                    result.SetSolution(false, false, new List<double>(), new List<string>()); 

                    return result;
                }
            }

            // Build expressions for pivot variables

            hasSolution = true;
            hasUniqueSolution = (freeCols.Count == 0);

            var particularSolution = new List<double>(new double[cols]);
            foreach (var col in pivotCols)
                particularSolution[col] = aug[rowPivot[col], cols];

            var expressions = new List<string>(new string[cols]);

            foreach (var col in pivotCols)
            {
                var sb = new StringBuilder();
                sb.Append($"x{col + 1} = {particularSolution[col]}");

                foreach (var freeCol in freeCols)
                {
                    var coefficient = aug[rowPivot[col], freeCol];

                    if (Math.Abs((dynamic)coefficient) > IMD.Constants.EPSILON)
                    {
                        if (coefficient < IMD.Constants.EPSILON) sb.Append($" + {-coefficient}*t{freeCol + 1}");
                        else sb.Append($" - {coefficient}*t{freeCol + 1}");
                    }
                }

                expressions[col] = sb.ToString();
            }

            foreach (var freeCol in freeCols)
                expressions[freeCol] = $"x{freeCol + 1} = t{freeCol + 1}";

            result.SetSolution(hasSolution, hasUniqueSolution, particularSolution, expressions);

            return result;
        }
        // Checks the compatibility of SLU's of the form Ax=b
        public static bool IsConsistent<T>(Matrix<T> A, Matrix<T> b) where T : IComparable<T>, INumber<T>
        {
            if (A is null) throw new ArgumentNullException("The matrix is null", nameof(A));
            if (b is null) throw new ArgumentNullException("The matrix is null", nameof(b));
            if (A.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(A));
            if (b.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(b));

            int rows = A.Rows, cols = A.Cols;

            if (b.Rows != rows || b.Cols != 1) throw new ArgumentWrongSizeException("The matrix have wrong size", nameof(A) + ", " + nameof(b));

            Matrix<T> aug = new Matrix<T>(rows, cols + 1);

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                    aug[i, j] = A[i, j];

                aug[i, cols] = b[i, 0];
            }

            return Rank(aug) == Rank(A);
        }

        // Returns the inverse of the given matrix 'mrx'
        public static Matrix<double> GetInverse<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));
            if (!mrx.IsSquare()) throw new ArgumentWrongSizeException("The matrix isn't square", nameof(mrx));

            int n = mrx.Rows;
            var A = new Matrix<double>(n, n);
            var result = Matrix<double>.Identity(n);

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    A[i, j] = (dynamic)mrx[i, j];

            for (int i = 0; i < n; ++i)
            {
                int pivot = i;

                for (int r = i + 1; r < n; ++r)
                    if (Math.Abs(A[r, i]) > Math.Abs(A[pivot, i]))
                        pivot = r;

                if (Math.Abs(A[pivot, i]) < IMD.Constants.EPSILON) throw new ArgumentSingularMatrixException("The matrix is singular", nameof(A));

                if (pivot != i)
                {
                    for (int col = 0; col < n; ++col)
                    {
                        var tempA = A[i, col];
                        A[i, col] = A[pivot, col];
                        A[pivot, col] = tempA;

                        var tempRes = result[i, col];
                        result[i, col] = result[pivot, col];
                        result[pivot, col] = tempRes;
                    }
                }

                double pivotVal = A[i, i];

                for (int col = 0; col < n; ++col)
                {
                    A[i, col] = (dynamic)A[i, col] / pivotVal;
                    result[i, col] = (dynamic)result[i, col] / pivotVal;
                }

                for (int r = 0; r < n; ++r)
                {
                    if (r != i)
                    {
                        double factor = A[r, i];

                        for (int col = 0; col < n; ++col)
                        {
                            A[r, col] -= factor * A[i, col];
                            result[r, col] -= factor * result[i, col];
                        }
                    }
                }
            }

            return result;
        }

        // Returns the kernel of the matrix 'mrx'
        public static List<Matrix<double>> GetKernel<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            var A = new Matrix<double>(rows, cols);

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    A[i, j] = Convert.ToDouble(mrx[i, j]);

            int rank = 0;
            var pivotCols = new List<int>();

            for (int col = 0; col < cols; ++col)
            {
                int pivotRow = rank;
                while (pivotRow < rows && Math.Abs(A[pivotRow, col]) < IMD.Constants.EPSILON) ++pivotRow;

                if (pivotRow == rows) continue;

                if (pivotRow != rank)
                {
                    for (int c = 0; c < cols; ++c)
                    {
                        var temp = A[rank, c];
                        A[rank, c] = A[pivotRow, c];
                        A[pivotRow, c] = temp;
                    }
                }

                double pivotValue = A[rank, col];

                for (int c = col; c < cols; ++c)
                    A[rank, c] /= pivotValue;

                for (int r = 0; r < rows; ++r)
                {
                    if (r != rank)
                    {
                        double factor = A[r, col];

                        for (int c = col; c < cols; ++c)
                            A[r, c] -= factor * A[rank, c];
                    }
                }

                pivotCols.Add(col);
                ++rank;

                if (rank == rows) break;
            }

            var isPivot = new bool[cols];
            foreach (var c in pivotCols)
                isPivot[c] = true;

            var freeVars = new List<int>();

            for (int c = 0; c < cols; ++c)
                if (!isPivot[c])
                    freeVars.Add(c);

            int kernelDim = freeVars.Count;
            var kernelBasis = new List<Matrix<double>>();

            for (int i = 0; i < kernelDim; ++i)
            {
                int freeVar = freeVars[i];
                var vec = new Matrix<double>(cols, 1);

                for (int j = 0; j < cols; ++j)
                    vec[j, 0] = (j == freeVar) ? 1.0 : 0.0;

                for (int j = pivotCols.Count - 1; j >= 0; --j)
                {
                    int pivotCol = pivotCols[j];
                    double sum = 0;

                    for (int c = pivotCol + 1; c < cols; ++c)
                        sum += A[j, c] * vec[c, 0];

                    vec[pivotCol, 0] = -sum;
                }

                kernelBasis.Add(vec);
            }


            return kernelBasis;
        }
        // Returns a list of coefficients of the characteristic polynomial of matrix 'mrx'
        public static List<double> GetCharacteristicPolynomial<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));
            if (!mrx.IsSquare()) throw new ArgumentWrongSizeException("The matrix isn't square", nameof(mrx));

            int n = mrx.Rows;
            var B = new Matrix<double>(n, n);
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    B[i, j] = Convert.ToDouble(mrx[i, j]);

            var traces = new List<double>(new double[n]);

            for (int k = 1; k <= n; ++k)
            {
                double trace = 0.0;

                if (k == 1)
                {
                    for (int i = 0; i < n; ++i)
                        trace += B[i, i];
                }
                else
                {
                    var newB = new Matrix<double>(n, n);

                    for (int i = 0; i < n; ++i)
                        for (int j = 0; j < n; ++j)
                        {
                            double sum = 0;

                            for (int l = 0; l < n; ++l)
                                sum += Convert.ToDouble(mrx[i, l]) * B[l, j];

                            newB[i, j] = (dynamic)sum;
                        }
                    B = newB;
                    for (int i = 0; i < n; ++i)
                        trace += B[i, i];
                }
                traces[k - 1] = trace;
            }

            var c = new List<double>(new double[n + 1]);
            c[0] = 1.0;

            for (int k = 1; k <= n; ++k)
            {
                double sum = 0.0;

                for (int j = 1; j < k; ++j)
                    sum += c[j] * traces[k - j - 1];
                c[k] = -(traces[k - 1] + sum) / k;
            }

            return c;
        }

        /// Operation methods

        // Returns the transposed matrix based on the matrix 'mrx'
        public static Matrix<T> GetTranspose<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            Matrix<T> result = new Matrix<T>(cols, rows, (i, j) => mrx[j, i]);

            return result;
        }
        // Transpose of the square matrix 'mrx'
        public static void Transpose<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));
            if (!mrx.IsSquare()) throw new ArgumentWrongSizeException("The matrix isn't square", nameof(mrx));

            int size = mrx.Rows;

            for (int i = 0; i < size; ++i)
            {
                for (int j = i + 1; j < size; ++j)
                {
                    T temp = mrx[i, j];
                    mrx[i, j] = mrx[j, i];
                    mrx[j, i] = temp;
                }
            }
        }

        // Vertical reflection of the matrix 'mrx'
        public static void FlipVertical<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols / 2; ++j)
                {
                    var temp = mrx[i, j];
                    mrx[i, j] = mrx[i, cols - 1 - j];
                    mrx[i, cols - 1 - j] = temp;
                }
            }
        }
        // Horizontal reflection of the matrix 'mrx'
        public static void FlipHorizontal<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;

            for (int i = 0; i < rows / 2; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    T temp = mrx[i, j];
                    mrx[i, j] = mrx[rows - 1 - i, j];
                    mrx[rows - 1 - i, j] = temp;
                }
            }
        }

        // Returns a matrix with the elements from the original matrix 'mrx' redistributed across the new sizes 'rows' and 'cols'
        public static Matrix<T> GetReshape<T>(Matrix<T> mrx, int rows, int cols) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentException("The matrix is empty", nameof(mrx));
            if (rows < 0) throw new ArgumentOutOfRangeException("The number of rows is negative", nameof(rows));
            if (cols < 0) throw new ArgumentOutOfRangeException("The number of cols is negative", nameof(cols));
            if (mrx.Rows * mrx.Cols != rows * cols) throw new ArgumentException("The total number of elements doesn't match", nameof(rows) + ", " + nameof(cols));

            var result = new Matrix<T>(rows, cols);

            int total = mrx.Rows * mrx.Cols;
            for (int i = 0; i < total; ++i)
            {
                int originalRow = i / mrx.Cols;
                int originalCol = i % mrx.Cols;

                int newRow = i / cols;
                int newCol = i % cols;

                result[newRow, newCol] = mrx[originalRow, originalCol];
            }

            return result;
        }
        // Returns a matrix of size 'rows' x 'cols' filled with elements from the list 'lst' row by row
        public static Matrix<T> ConstructMatrix<T>(List<T> lst, int rows, int cols) where T : IComparable<T>, INumber<T>
        {
            if (lst is null) throw new ArgumentNullException("The list is null", nameof(lst));
            if (lst.Count == 0) throw new ArgumentException("The list is empty", nameof(lst));
            if (rows < 0) throw new ArgumentOutOfRangeException("The number of rows is negative", nameof(rows));
            if (cols < 0) throw new ArgumentOutOfRangeException("The number of cols is negative", nameof(cols));
            if (rows * cols != lst.Count) throw new ArgumentException("The total number of elements does not match");

            var result = new Matrix<T>(rows, cols);
            int index = 0;

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    result[i, j] = lst[index++];

            return result;
        }

        // Returns a matrix obtained from the matrix 'mrx' by rotating it 90 degrees clockwise
        public static Matrix<T> GetClockwiseRotation<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            Matrix<T> result = GetTranspose(mrx);
            int rows = result.Rows, cols = result.Cols;

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols / 2; ++j)
                {
                    var temp = result[i, j];
                    result[i, j] = result[i, cols - 1 - j];
                    result[i, cols - 1 - j] = temp;
                }
            }

            return result;
        }
        // Rotate square the matrix 'mrx' 90 degrees clockwise
        public static void ClockwiseRotate<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));
            if (!mrx.IsSquare()) throw new ArgumentWrongSizeException("The matrix isn't square", nameof(mrx));

            Transpose(mrx);

            int n = mrx.Rows;

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n / 2; ++j)
                {
                    var temp = mrx[i, j];
                    mrx[i, j] = mrx[i, n - 1 - j];
                    mrx[i, n - 1 - j] = temp;
                }
            }
        }

        // Returns a matrix obtained from the matrix 'mrx' by rotating it 90 degrees counterclockwise
        public static Matrix<T> GetCounterclockwiseRotation<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            Matrix<T> result = GetTranspose(mrx);
            int rows = result.Rows, cols = result.Cols;

            for (int j = 0; j < cols; ++j)
            {
                for (int i = 0; i < rows / 2; ++i)
                {
                    var temp = result[i, j];
                    result[i, j] = result[rows - 1 - i, j];
                    result[rows - 1 - i, j] = temp;
                }
            }

            return result;
        }
        // Rotate square the matrix 'mrx' 90 degrees counterclockwise
        public static void CounterclockwiseRotate<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));
            if (!mrx.IsSquare()) throw new ArgumentWrongSizeException("The matrix isn't square", nameof(mrx));

            Transpose(mrx);

            int n = mrx.Rows;

            for (int j = 0; j < n; ++j)
            {
                for (int i = 0; i < n / 2; ++i)
                {
                    var temp = mrx[i, j];
                    mrx[i, j] = mrx[n - 1 - i, j];
                    mrx[n - 1 - i, j] = temp;
                }
            }
        }

        // Checks for cycle in the matrix 'mrx' using DFS
        public static bool DFSCheckCycle<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            if (mrx.Size == 1) return true;

            int rows = mrx.Rows, cols = mrx.Cols;
            var visited = new List<List<bool>>(rows);

            for (int i = 0; i < rows; ++i)
            {
                var row = new List<bool>(cols);

                for (int j = 0; j < cols; ++j)
                    row.Add(false);

                visited.Add(row);
            }

            bool DFS(int startX, int startY)
            {
                T targetChar = mrx[startX, startY];
                var stack = new Stack<(int currentX, int currentY, int prevX, int prevY)>();
                stack.Push((startX, startY, -1, -1));
                visited[startX][startY] = true;

                while (stack.Count > 0)
                {
                    var (currentX, currentY, previousX, previousY) = stack.Pop();

                    foreach (var dir in IMD.Constants.DIRECTIONS_WITHOUT_DIAGONAL)
                    {
                        int newX = currentX + dir[0];
                        int newY = currentY + dir[1];

                        if (newX == previousX && newY == previousY) continue;

                        if (newX < 0 || newY < 0 || newX >= rows || newY >= cols) continue;

                        if (!EqualityComparer<T>.Default.Equals(mrx[newX, newY], targetChar)) continue;

                        if (visited[newX][newY]) return true;

                        visited[newX][newY] = true;
                        stack.Push((newX, newY, currentX, currentY));
                    }
                }
                return false;
            }

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    if (!visited[i][j] && DFS(i, j))
                        return true;

            return false;
        }
        // Checks for cycle in the matrix 'mrx' using BFS
        public static bool BFSCheckCycle<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            if (mrx.Size == 1) return true;

            int rows = mrx.Rows, cols = mrx.Cols;
            var visited = new List<List<bool>>(rows);

            for (int i = 0; i < rows; ++i)
            {
                var row = new List<bool>(cols);

                for (int j = 0; j < cols; ++j)
                    row.Add(false);

                visited.Add(row);
            }

            bool BFS(int startX, int startY)
            {
                T targetChar = mrx[startX, startY];
                var queue = new Queue<(int currentX, int currentY, int prevX, int prevY)>();
                queue.Enqueue((startX, startY, -1, -1));
                visited[startX][startY] = true;

                while (queue.Count > 0)
                {
                    var (currentX, currentY, previousX, previousY) = queue.Dequeue();

                    foreach (var dir in IMD.Constants.DIRECTIONS_WITHOUT_DIAGONAL)
                    {
                        int newX = currentX + dir[0];
                        int newY = currentY + dir[1];

                        if (newX == previousX && newY == previousY) continue;

                        if (newX < 0 || newY < 0 || newX >= rows || newY >= cols) continue;

                        if (!EqualityComparer<T>.Default.Equals(mrx[newX, newY], targetChar)) continue;

                        if (visited[newX][newY]) return true;

                        visited[newX][newY] = true;
                        queue.Enqueue((newX, newY, currentX, currentY));
                    }
                }

                return false;
            }

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    if (!visited[i][j] && BFS(i, j))
                        return true;

            return false;
        }

        // Returns the cycle in the matrix 'mrx' using DFS
        public static List<(int, int)> DFSGetCycle<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));
            if (mrx.Size < 4) return new List<(int, int)>();

            int rows = mrx.Rows, cols = mrx.Cols;
            var visited = new bool[rows, cols];
            var parent = new (int, int)[rows, cols];

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    parent[i, j] = (-1, -1);

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    if (visited[i, j]) continue;

                    var stack = new Stack<(int, int)>();
                    stack.Push((i, j));
                    visited[i, j] = true;
                    parent[i, j] = (-1, -1);

                    while (stack.Count > 0)
                    {
                        var current = stack.Pop();
                        int x = current.Item1, y = current.Item2;

                        foreach (var dir in IMD.Constants.DIRECTIONS_WITHOUT_DIAGONAL)
                        {
                            int nx = x + dir[0];
                            int ny = y + dir[1];

                            if (nx < 0 || ny < 0 || nx >= rows || ny >= cols) continue;
                            if (!EqualityComparer<T>.Default.Equals(mrx[nx, ny], mrx[x, y])) continue;

                            if (!visited[nx, ny])
                            {
                                visited[nx, ny] = true;
                                parent[nx, ny] = current;
                                stack.Push((nx, ny));
                            }
                            else if (!parent[current.Item1, current.Item2].Equals((nx, ny)))
                            {
                                var cycle = ReconstructCycle(current, (nx, ny), parent);

                                if (cycle.Count >= 4) return cycle;
                            }
                        }
                    }
                }
            }

            return new List<(int, int)>();
        }
        // Returns the cycle in the matrix 'mrx' using BFS
        public static List<(int, int)> BFSGetCycle<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            var visited = new bool[rows, cols];
            var parent = new (int, int)[rows, cols];

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    parent[i, j] = (-1, -1);

            List<(int, int)> BFS(int startX, int startY)
            {
                var queue = new Queue<(int, int)>();
                visited[startX, startY] = true;
                parent[startX, startY] = (-1, -1);
                queue.Enqueue((startX, startY));

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();

                    foreach (var dir in IMD.Constants.DIRECTIONS_WITHOUT_DIAGONAL)
                    {
                        int nx = current.Item1 + dir[0];
                        int ny = current.Item2 + dir[1];

                        if (nx < 0 || ny < 0 || nx >= rows || ny >= cols) continue;
                        if (!EqualityComparer<T>.Default.Equals(mrx[nx, ny], mrx[current.Item1, current.Item2])) continue;

                        if (!visited[nx, ny])
                        {
                            visited[nx, ny] = true;
                            parent[nx, ny] = current;
                            queue.Enqueue((nx, ny));
                        }
                        else if (!parent[current.Item1, current.Item2].Equals((nx, ny)))
                        {
                            var candidateCycle = ReconstructCycle(current, (nx, ny), parent);

                            if (candidateCycle.Count >= 4) return candidateCycle;
                        }
                    }
                }

                return new List<(int, int)>();
            }

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    if (!visited[i, j])
                    {
                        var cycle = BFS(i, j);
                        if (cycle.Count > 0)
                            return cycle;
                    }
                }
            }

            return new List<(int, int)>();
        }

        // Returns the maximum sum along the path from the upper left to the lower right corner of the matrix 'mrx', moving only to the right or down (no diagonal transitions)
        public static T MaxPathSumWithoutDiagonal<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            var dp = new Matrix<T>(rows, cols);
            dynamic zero = T.Zero;

            dp[0, 0] = mrx[0, 0];

            for (int j = 1; j < cols; ++j)
                dp[0, j] = (dynamic)dp[0, j - 1] + (dynamic)mrx[0, j];

            for (int i = 1; i < rows; ++i)
                dp[i, 0] = (dynamic)dp[i - 1, 0] + (dynamic)mrx[i, 0];

            for (int i = 1; i < rows; ++i)
            {
                for (int j = 1; j < cols; ++j)
                {
                    dynamic left = dp[i, j - 1];
                    dynamic up = dp[i - 1, j];
                    dynamic max = left.CompareTo(up) > 0 ? left : up;
                    dp[i, j] = max + (dynamic)mrx[i, j];
                }
            }

            return dp[rows - 1, cols - 1];
        }
        // Returns the maximum sum along the path from the upper left to the lower right corner of the matrix 'mrx', moving only to the right, down, and diagonally down
        public static T MaxPathSumWithDiagonal<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            var dp = new Matrix<T>(rows, cols);

            dp[0, 0] = mrx[0, 0];

            for (int j = 1; j < cols; ++j)
                dp[0, j] = (dynamic)dp[0, j - 1] + (dynamic)mrx[0, j];

            for (int i = 1; i < rows; ++i)
                dp[i, 0] = (dynamic)dp[i - 1, 0] + (dynamic)mrx[i, 0];

            for (int i = 1; i < rows; ++i)
            {
                for (int j = 1; j < cols; ++j)
                {
                    dynamic left = dp[i, j - 1];
                    dynamic up = dp[i - 1, j];
                    dynamic diagonal = dp[i - 1, j - 1];

                    dynamic max = left;
                    if (up.CompareTo(max) > 0) max = up;
                    if (diagonal.CompareTo(max) > 0) max = diagonal;

                    dp[i, j] = max + (dynamic)mrx[i, j];
                }
            }

            return dp[rows - 1, cols - 1];
        }

        // Returns the minimum sum along the path from the upper left to the lower right corner of the matrix 'mrx', moving only to the right or down (no diagonal transitions)
        public static T MinPathSumWithoutDiagonal<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            var dp = new Matrix<T>(rows, cols);

            dp[0, 0] = mrx[0, 0];

            for (int j = 1; j < cols; ++j)
                dp[0, j] = (dynamic)dp[0, j - 1] + (dynamic)mrx[0, j];

            for (int i = 1; i < rows; ++i)
                dp[i, 0] = (dynamic)dp[i - 1, 0] + (dynamic)mrx[i, 0];

            for (int i = 1; i < rows; ++i)
            {
                for (int j = 1; j < cols; ++j)
                {
                    dynamic left = dp[i, j - 1];
                    dynamic up = dp[i - 1, j];
                    dynamic min = left.CompareTo(up) < 0 ? left : up;
                    dp[i, j] = min + (dynamic)mrx[i, j];
                }
            }

            return dp[rows - 1, cols - 1];
        }
        // Returns the minimum sum along the path from the top left to the bottom right corner of the matrix 'mrx', moving only to the right, down, and diagonally down
        public static T MinPathSumWithDiagonal<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            var dp = new Matrix<T>(rows, cols);

            dp[0, 0] = mrx[0, 0];

            for (int j = 1; j < cols; ++j)
                dp[0, j] = (dynamic)dp[0, j - 1] + (dynamic)mrx[0, j];

            for (int i = 1; i < rows; ++i)
                dp[i, 0] = (dynamic)dp[i - 1, 0] + (dynamic)mrx[i, 0];

            for (int i = 1; i < rows; ++i)
            {
                for (int j = 1; j < cols; ++j)
                {
                    dynamic left = dp[i, j - 1];
                    dynamic up = dp[i - 1, j];
                    dynamic diagonal = dp[i - 1, j - 1];

                    dynamic min = left;
                    if (up.CompareTo(min) < 0) min = up;
                    if (diagonal.CompareTo(min) < 0) min = diagonal;

                    dp[i, j] = min + (dynamic)mrx[i, j];
                }
            }

            return dp[rows - 1, cols - 1];
        }

        // Returns the path with the maximum sum of values ​​from the upper left to the lower right corner of the matrix 'mrx', moving only to the right or down (no diagonal transitions)
        public static List<(int, int)> MaxPathWithoutDiagonal<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            var dp = new Matrix<T>(rows, cols);
            var paths = new List<(int, int)>[rows, cols];

            dp[0, 0] = mrx[0, 0];
            paths[0, 0] = new List<(int, int)> { (0, 0) };

            for (int j = 1; j < cols; ++j)
            {
                dp[0, j] = (dynamic)dp[0, j - 1] + (dynamic)mrx[0, j];
                paths[0, j] = new List<(int, int)>(paths[0, j - 1]);
                paths[0, j].Add((0, j));
            }

            for (int i = 1; i < rows; ++i)
            {
                dp[i, 0] = (dynamic)dp[i - 1, 0] + (dynamic)mrx[i, 0];
                paths[i, 0] = new List<(int, int)>(paths[i - 1, 0]);
                paths[i, 0].Add((i, 0));
            }

            for (int i = 1; i < rows; ++i)
            {
                for (int j = 1; j < cols; ++j)
                {
                    dynamic up = dp[i - 1, j];
                    dynamic left = dp[i, j - 1];

                    if (up.CompareTo(left) > 0)
                    {
                        dp[i, j] = up + (dynamic)mrx[i, j];
                        paths[i, j] = new List<(int, int)>(paths[i - 1, j]);
                    }
                    else
                    {
                        dp[i, j] = left + (dynamic)mrx[i, j];
                        paths[i, j] = new List<(int, int)>(paths[i, j - 1]);
                    }
                    paths[i, j].Add((i, j));
                }
            }

            return paths[rows - 1, cols - 1];
        }
        // Returns the path with the maximum sum of values ​​from the top left to the bottom right corner of the matrix 'mrx', moving only to the right, down, and diagonally down
        public static List<(int, int)> MaxPathWithDiagonal<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            var dp = new Matrix<T>(rows, cols);
            var paths = new List<(int, int)>[rows, cols];

            dp[0, 0] = mrx[0, 0];
            paths[0, 0] = new List<(int, int)> { (0, 0) };

            for (int j = 1; j < cols; ++j)
            {
                dp[0, j] = (dynamic)dp[0, j - 1] + (dynamic)mrx[0, j];
                paths[0, j] = new List<(int, int)>(paths[0, j - 1]);
                paths[0, j].Add((0, j));
            }

            for (int i = 1; i < rows; ++i)
            {
                dp[i, 0] = (dynamic)dp[i - 1, 0] + (dynamic)mrx[i, 0];
                paths[i, 0] = new List<(int, int)>(paths[i - 1, 0]);
                paths[i, 0].Add((i, 0));
            }

            for (int i = 1; i < rows; ++i)
            {
                for (int j = 1; j < cols; ++j)
                {
                    dynamic up = dp[i - 1, j];
                    dynamic left = dp[i, j - 1];
                    dynamic diag = dp[i - 1, j - 1];

                    dynamic max = up;
                    List<(int, int)> maxPath = paths[i - 1, j];

                    if (left.CompareTo(max) > 0)
                    {
                        max = left;
                        maxPath = paths[i, j - 1];
                    }
                    if (diag.CompareTo(max) > 0)
                    {
                        max = diag;
                        maxPath = paths[i - 1, j - 1];
                    }

                    dp[i, j] = max + (dynamic)mrx[i, j];
                    paths[i, j] = new List<(int, int)>(maxPath);
                    paths[i, j].Add((i, j));
                }
            }

            return paths[rows - 1, cols - 1];
        }

        // Returns the path with the minimum sum of values ​​from the upper left to the lower right corner of the matrix 'mrx', moving only to the right or down (no diagonal transitions)
        public static List<(int, int)> MinPathWithoutDiagonal<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            var dp = new Matrix<T>(rows, cols);
            var paths = new List<(int, int)>[rows, cols];

            dp[0, 0] = mrx[0, 0];
            paths[0, 0] = new List<(int, int)> { (0, 0) };

            for (int j = 1; j < cols; ++j)
            {
                dp[0, j] = (dynamic)dp[0, j - 1] + (dynamic)mrx[0, j];
                paths[0, j] = new List<(int, int)>(paths[0, j - 1]);
                paths[0, j].Add((0, j));
            }

            for (int i = 1; i < rows; ++i)
            {
                dp[i, 0] = (dynamic)dp[i - 1, 0] + (dynamic)mrx[i, 0];
                paths[i, 0] = new List<(int, int)>(paths[i - 1, 0]);
                paths[i, 0].Add((i, 0));
            }

            for (int i = 1; i < rows; ++i)
            {
                for (int j = 1; j < cols; ++j)
                {
                    dynamic up = dp[i - 1, j];
                    dynamic left = dp[i, j - 1];

                    if (up.CompareTo(left) < 0)
                    {
                        dp[i, j] = up + (dynamic)mrx[i, j];
                        paths[i, j] = new List<(int, int)>(paths[i - 1, j]);
                    }
                    else
                    {
                        dp[i, j] = left + (dynamic)mrx[i, j];
                        paths[i, j] = new List<(int, int)>(paths[i, j - 1]);
                    }
                    paths[i, j].Add((i, j));
                }
            }

            return paths[rows - 1, cols - 1];
        }
        // Returns the path with the minimum sum of values ​​from the top left to the bottom right corner of the matrix 'mrx', moving only to the right, down, and diagonally down
        public static List<(int, int)> MinPathWithDiagonal<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            var dp = new Matrix<T>(rows, cols);
            var paths = new List<(int, int)>[rows, cols];

            dp[0, 0] = mrx[0, 0];
            paths[0, 0] = new List<(int, int)> { (0, 0) };

            for (int j = 1; j < cols; ++j)
            {
                dp[0, j] = (dynamic)dp[0, j - 1] + (dynamic)mrx[0, j];
                paths[0, j] = new List<(int, int)>(paths[0, j - 1]);
                paths[0, j].Add((0, j));
            }

            for (int i = 1; i < rows; ++i)
            {
                dp[i, 0] = (dynamic)dp[i - 1, 0] + (dynamic)mrx[i, 0];
                paths[i, 0] = new List<(int, int)>(paths[i - 1, 0]);
                paths[i, 0].Add((i, 0));
            }

            for (int i = 1; i < rows; ++i)
            {
                for (int j = 1; j < cols; ++j)
                {
                    dynamic up = dp[i - 1, j];
                    dynamic left = dp[i, j - 1];
                    dynamic diag = dp[i - 1, j - 1];

                    dynamic min = up;
                    List<(int, int)> minPath = paths[i - 1, j];

                    if (left.CompareTo(min) < 0)
                    {
                        min = left;
                        minPath = paths[i, j - 1];
                    }
                    if (diag.CompareTo(min) < 0)
                    {
                        min = diag;
                        minPath = paths[i - 1, j - 1];
                    }

                    dp[i, j] = min + (dynamic)mrx[i, j];
                    paths[i, j] = new List<(int, int)>(minPath);
                    paths[i, j].Add((i, j));
                }
            }

            return paths[rows - 1, cols - 1];
        }

        // Returns the length of the longest ascending path in the matrix 'mrx'. The severity of the inequality is determined by the 'isSeverity' argument
        public static int LongestIncreasingPathLength<T>(Matrix<T> mrx, bool isSeverity = true) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            var dp = new int[rows, cols];
            var visited = new bool[rows, cols];

            int DFS(int r, int c)
            {
                if (dp[r, c] != 0) return dp[r, c];
                if (visited[r, c]) return 0;

                visited[r, c] = true;
                int maxLen = 1;

                foreach (var dir in IMD.Constants.DIRECTIONS_WITHOUT_DIAGONAL)
                {
                    int nr = r + dir[0], nc = c + dir[1];

                    if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
                    {
                        bool condition = isSeverity ? mrx[nr, nc].CompareTo(mrx[r, c]) > 0 : mrx[nr, nc].CompareTo(mrx[r, c]) >= 0;

                        if (condition)
                        {
                            int len = 1 + DFS(nr, nc);

                            if (len > maxLen) maxLen = len;
                        }
                    }
                }

                visited[r, c] = false;
                dp[r, c] = maxLen;
                return maxLen;
            }

            int result = 0;

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    int len = DFS(i, j);

                    if (len > result) result = len;
                }
            }

            return result;
        }
        // Returns the length of the longest descending path in the matrix 'mrx'. The severity of the inequality is determined by the 'isSeverity' argument
        public static int LongestDecreasingPathLength<T>(Matrix<T> mrx, bool isSeverity = true) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            var dp = new int[rows, cols];
            var visited = new bool[rows, cols];

            int DFS(int r, int c)
            {
                if (dp[r, c] != 0) return dp[r, c];
                if (visited[r, c]) return 0;

                visited[r, c] = true;
                int maxLen = 1;

                foreach (var dir in IMD.Constants.DIRECTIONS_WITHOUT_DIAGONAL)
                {
                    int nr = r + dir[0], nc = c + dir[1];

                    if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
                    {
                        bool condition = isSeverity ? mrx[nr, nc].CompareTo(mrx[r, c]) < 0 : mrx[nr, nc].CompareTo(mrx[r, c]) <= 0;

                        if (condition)
                        {
                            int len = 1 + DFS(nr, nc);

                            if (len > maxLen) maxLen = len;
                        }
                    }
                }

                visited[r, c] = false;
                dp[r, c] = maxLen;

                return maxLen;
            }

            int result = 0;

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    int len = DFS(i, j);

                    if (len > result) result = len;
                }
            }

            return result;
        }


        // Returns the longest ascending path in the matrix 'mrx'. The severity of the inequality is determined by the 'isSeverity' argument
        public static List<(int r, int c)> LongestIncreasingPath<T>(Matrix<T> mrx, bool isSeverity = true) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            var dp = new int[rows, cols];
            var paths = new List<(int, int)>[rows, cols];
            var visited = new bool[rows, cols];

            List<(int, int)> DFS(int r, int c)
            {
                if (paths[r, c] != null) return paths[r, c];
                if (visited[r, c]) return new List<(int, int)>();

                visited[r, c] = true;

                var bestPath = new List<(int, int)> { (r, c) };
                int maxLen = 1;

                foreach (var dir in IMD.Constants.DIRECTIONS_WITHOUT_DIAGONAL)
                {
                    int nr = r + dir[0], nc = c + dir[1];

                    if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
                    {
                        bool condition = isSeverity ? mrx[nr, nc].CompareTo(mrx[r, c]) > 0 : mrx[nr, nc].CompareTo(mrx[r, c]) >= 0;
                        if (condition)
                        {
                            var neighborPath = DFS(nr, nc);

                            if (neighborPath.Count + 1 > maxLen)
                            {
                                maxLen = neighborPath.Count + 1;
                                bestPath = new List<(int, int)> { (r, c) };
                                bestPath.AddRange(neighborPath);
                            }
                        }
                    }
                }

                visited[r, c] = false;
                dp[r, c] = maxLen;
                paths[r, c] = bestPath;

                return bestPath;
            }

            List<(int, int)> result = new List<(int, int)>();

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    var path = DFS(i, j);

                    if (path.Count > result.Count) result = path;
                }
            }

            return result;
        }
        // Returns the longest descending path in the matrix 'mrx'. The severity of the inequality is determined by the 'isSeverity' argument
        public static List<(int r, int c)> LongestDecreasingPath<T>(Matrix<T> mrx, bool isSeverity = true) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            var dp = new int[rows, cols];
            var paths = new List<(int, int)>[rows, cols];
            var visited = new bool[rows, cols];

            List<(int, int)> DFS(int r, int c)
            {
                if (paths[r, c] != null) return paths[r, c];
                if (visited[r, c]) return new List<(int, int)>();

                visited[r, c] = true;

                var bestPath = new List<(int, int)> { (r, c) };
                int maxLen = 1;

                foreach (var dir in IMD.Constants.DIRECTIONS_WITHOUT_DIAGONAL)
                {
                    int nr = r + dir[0], nc = c + dir[1];

                    if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
                    {
                        bool condition = isSeverity ? mrx[nr, nc].CompareTo(mrx[r, c]) < 0 : mrx[nr, nc].CompareTo(mrx[r, c]) <= 0;
                        if (condition)
                        {
                            var neighborPath = DFS(nr, nc);

                            if (neighborPath.Count + 1 > maxLen)
                            {
                                maxLen = neighborPath.Count + 1;
                                bestPath = new List<(int, int)> { (r, c) };
                                bestPath.AddRange(neighborPath);
                            }
                        }
                    }
                }

                visited[r, c] = false;
                dp[r, c] = maxLen;
                paths[r, c] = bestPath;

                return bestPath;
            }

            List<(int, int)> result = new List<(int, int)>();

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    var path = DFS(i, j);

                    if (path.Count > result.Count) result = path;
                }
            }

            return result;
        }

        // Returns the number of increasing paths in the matrix 'mrx'. The severity of the inequality is determined by the 'isSeverity' argument
        public static int CountIncreasingPaths<T>(Matrix<T> mrx, bool isSeverity = true) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            var dp = new int[rows, cols];
            var visited = new bool[rows, cols];

            int DFS(int r, int c)
            {
                if (dp[r, c] != 0) return dp[r, c];
                if (visited[r, c]) return 0;

                visited[r, c] = true;
                int count = 1;

                foreach (var dir in IMD.Constants.DIRECTIONS_WITHOUT_DIAGONAL)
                {
                    int nr = r + dir[0], nc = c + dir[1];

                    if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
                    {
                        bool condition = isSeverity ? mrx[nr, nc].CompareTo(mrx[r, c]) > 0 : mrx[nr, nc].CompareTo(mrx[r, c]) >= 0;
                        if (condition) count += DFS(nr, nc);
                    }
                }

                visited[r, c] = false;
                dp[r, c] = count;

                return count;
            }

            int result = 0;

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    result += DFS(i, j);

            return result;
        }
        // Returns the number of decreasing paths in the matrix 'mrx'. The severity of the inequality is determined by the 'isSeverity' argument
        public static int CountDecreasingPaths<T>(Matrix<T> mrx, bool isSeverity = true) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));

            int rows = mrx.Rows, cols = mrx.Cols;
            var dp = new int[rows, cols];
            var visited = new bool[rows, cols];

            int DFS(int r, int c)
            {
                if (dp[r, c] != 0) return dp[r, c];
                if (visited[r, c]) return 0;

                visited[r, c] = true;
                int count = 1;

                foreach (var dir in IMD.Constants.DIRECTIONS_WITHOUT_DIAGONAL)
                {
                    int nr = r + dir[0], nc = c + dir[1];

                    if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
                    {
                        bool condition = isSeverity ? mrx[nr, nc].CompareTo(mrx[r, c]) < 0 : mrx[nr, nc].CompareTo(mrx[r, c]) <= 0;

                        if (condition) count += DFS(nr, nc);
                    }
                }

                visited[r, c] = false;
                dp[r, c] = count;

                return count;
            }

            int result = 0;

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    result += DFS(i, j);

            return result;
        }

        /// Special methods

        // Returns the maximum amount of gold that can be collected in the matrix 'grid' under the following conditions:
        // - Start from any cell containing gold (> 0).
        // - Move only up, down, left, or right (no diagonals).
        // - Do not visit the same cell more than once.
        // - Never visit cells with 0 gold.
        // - Collect all gold from each visited cell.
        public static T GetMaxGold<T>(Matrix<T> grid) where T : IComparable<T>, INumber<T>
        {
            if (grid is null) throw new ArgumentNullException("The matrix is null", nameof(grid));
            if (grid.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(grid));

            int rows = grid.Rows, cols = grid.Cols;
            T result = default(T);
            bool maxGoldInitialized = false;
            var visited = new bool[rows, cols];

            void DFS(int r, int c, T currentSum)
            {
                if (!maxGoldInitialized || currentSum.CompareTo(result) > 0)
                {
                    result = currentSum;
                    maxGoldInitialized = true;
                }

                foreach (var dir in IMD.Constants.DIRECTIONS_WITHOUT_DIAGONAL)
                {
                    int nr = r + dir[0];
                    int nc = c + dir[1];

                    if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
                    {
                        if (!visited[nr, nc] && Comparer<T>.Default.Compare(grid[nr, nc], default(T)) > 0)
                        {
                            visited[nr, nc] = true;
                            T sum = currentSum;
                            sum += (dynamic)grid[nr, nc];

                            DFS(nr, nc, sum);

                            visited[nr, nc] = false;
                        }
                    }
                }
            }

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    if (Comparer<T>.Default.Compare(grid[i, j], default(T)) > 0)
                    {
                        visited[i, j] = true;
                        DFS(i, j, grid[i, j]);
                        visited[i, j] = false;
                    }
                }
            }

            return result;
        }
        // Returns a path with the maximum amount of gold that can be collected in the matrix 'grid' under the following conditions:
        // - Start from any cell containing gold (> 0).
        // - Move only up, down, left, or right (no diagonals).
        // - Do not visit the same cell more than once.
        // - Never visit cells with 0 gold.
        // - Collect all gold from each visited cell.
        public static List<(int r, int c)> GetMaxGoldPath<T>(Matrix<T> grid) where T : IComparable<T>, INumber<T>
        {
            if (grid is null) throw new ArgumentNullException("The matrix is null", nameof(grid));
            if (grid.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(grid));

            int rows = grid.Rows, cols = grid.Cols;
            T maxGold = default(T);
            bool maxGoldInitialized = false;
            var visited = new bool[rows, cols];
            var currentPath = new List<(int, int)>();
            var result = new List<(int, int)>();

            void DFS(int r, int c, T currentSum)
            {
                currentPath.Add((r, c));

                if (!maxGoldInitialized || currentSum.CompareTo(maxGold) > 0)
                {
                    maxGold = currentSum;
                    maxGoldInitialized = true;
                    result = new List<(int, int)>(currentPath);
                }

                foreach (var dir in IMD.Constants.DIRECTIONS_WITHOUT_DIAGONAL)
                {
                    int nr = r + dir[0], nc = c + dir[1];

                    if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
                    {
                        if (!visited[nr, nc] && Comparer<T>.Default.Compare(grid[nr, nc], default(T)) > 0)
                        {
                            visited[nr, nc] = true;
                            T sum = currentSum;
                            sum += grid[nr, nc];

                            DFS(nr, nc, sum);

                            visited[nr, nc] = false;
                        }
                    }
                }

                currentPath.RemoveAt(currentPath.Count - 1);
            }

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    if (Comparer<T>.Default.Compare(grid[i, j], default(T)) > 0)
                    {
                        visited[i, j] = true;
                        DFS(i, j, grid[i, j]);
                        visited[i, j] = false;
                    }
                }
            }

            return result;
        }
        // Given the server center map, represented as the integer matrix 'grid' of arbitrary size, where 1 means there is a server in that cell and 0 means there is no server.
        // Two servers are considered communicating if they are in the same row or in the same column.
        // Returns the number of servers communicating with each other.
        public static int CountCommunicateServers<T>(Matrix<T> grid) where T : IComparable<T>, INumber<T>
        {
            if (grid is null) throw new ArgumentNullException("The matrix is null", nameof(grid));
            if (grid.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(grid));

            int rows = grid.Rows, cols = grid.Cols;
            int[] rowCount = new int[rows], colCount = new int[cols];
            int totalServers = 0, isolated = 0;

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    if (grid[i, j] == T.One)
                    {
                        ++rowCount[i];
                        ++colCount[j];
                        ++totalServers;
                    }
                }
            }

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    if (grid[i, j] == T.One && rowCount[i] == 1 && colCount[j] == 1)
                        ++isolated;

            return totalServers - isolated;
        }
        // Given the matrix 'board' of symbols board, where 'X' denotes a part of a ship, '.' denotes an empty cell.
        // Ships do not touch each other horizontally or vertically.
        // Only the beginning of each ship is counted - the cell 'X' that does not have an 'X' above or to the left.
        // Returns the number of warships on the board
        public static int CountBattleships(Matrix<char> board)
        {
            if (board is null) throw new ArgumentNullException("The matrix is null", nameof(board));
            if (board.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(board));

            int result = 0, rows = board.Rows, cols = board.Cols;

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    if (board[i, j] == 'X')
                    {
                        bool isStartOfShip = (i == 0 || board[i - 1, j] != 'X') && (j == 0 || board[i, j - 1] != 'X');

                        if (isStartOfShip) ++result;
                    }
                }
            }
            return result;
        }

        /// Helper methods

        private static List<(int, int)> ReconstructCycle((int, int) start, (int, int) end, (int, int)[,] parent)
        {
            var pathStart = new List<(int, int)>();
            var pathEnd = new List<(int, int)>();

            for (var cur = start; cur.Item1 != -1; cur = parent[cur.Item1, cur.Item2])
                pathStart.Add(cur);
            for (var cur = end; cur.Item1 != -1; cur = parent[cur.Item1, cur.Item2])
                pathEnd.Add(cur);

            pathStart.Reverse();
            pathEnd.Reverse();

            int lcaIndex = 0;
            int minLen = Math.Min(pathStart.Count, pathEnd.Count);

            for (int i = 0; i < minLen; ++i)
            {
                if (!pathStart[i].Equals(pathEnd[i])) break;

                lcaIndex = i;
            }

            var cycle = new List<(int, int)>();

            for (int i = pathStart.Count - 1; i > lcaIndex; --i)
                cycle.Add(pathStart[i]);

            for (int i = lcaIndex; i < pathEnd.Count; ++i)
                cycle.Add(pathEnd[i]);

            cycle.Add(cycle[0]);

            for (int i = 0; i < cycle.Count - 1; ++i)
            {
                var a = cycle[i];
                var b = cycle[i + 1];
                int dx = Math.Abs(a.Item1 - b.Item1), dy = Math.Abs(a.Item2 - b.Item2);

                if (!((dx == 1 && dy == 0) || (dx == 0 && dy == 1))) return new List<(int, int)>();
            }

            return cycle;
        }
    }
}
