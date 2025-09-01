using System.Collections;
using System.Numerics;

namespace IMD
{
    public class ArgumentEmptyException : ArgumentException
    {
        public ArgumentEmptyException(string message, string paramName) : base(message, paramName) { }

    }
    public class ArgumentUncorrectSizeException : ArgumentException
    {
        public ArgumentUncorrectSizeException(string message, string paramName) : base(message, paramName) { }

    }
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

            if (rows1 != rows2 || cols1 != cols2) throw new ArgumentUncorrectSizeException("The matrices have different sizes", nameof(a) + ", " + nameof(b));

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

            if (rows1 != rows2 || cols1 != cols2) throw new ArgumentUncorrectSizeException("The matrices have different sizes", nameof(a) + ", " + nameof(b));

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

            if (cols1 != rows2) throw new ArgumentUncorrectSizeException("The matrices have wrong sizes", nameof(a) + ", " + nameof(b));

            Matrix<T> result = new Matrix<T>(rows1, cols1);

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

            if (rows1 != rows2 || cols1 != cols2) throw new ArgumentUncorrectSizeException("The matrices have different sizes", nameof(a) + ", " + nameof(b));

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


        /// Conversion methods

        // Returns a matrix obtained from a rectangular full container of containers 'ccs'
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

        // Math methods

        // Возвращает подматрицу, полученную из матрицы 'mrx' путём исключения строки с индексом excludedRow и столбца с индексом excludedCol
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
        // Возвращает определитель квадратной матрицы 'mrx'
        public static T Determinant<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));
            if (!mrx.IsSquare()) throw new ArgumentUncorrectSizeException("The matrix isn't square", nameof(mrx));

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
        // Возвращает минор в квадратной матрице 'mrx' в строке с индексом row и в столбце с индексом col
        public static T Minor<T>(Matrix<T> mrx, int row, int col) where T : IComparable<T>, INumber<T>
        {
            return Determinant(GetSubMatrix(mrx, row, col));
        }
        // Возвращает алгебраическое дополнение в квадратной матрице 'mrx' в строке с индексом row и в столбце с индексом col
        public static T Cofactor<T>(Matrix<T> mrx, int row, int col) where T : IComparable<T>, INumber<T>
        {
            T result = Minor(mrx, row, col);
            return ((row + col) % 2 == 0) ? result : -result;
        }


        // Returns the Frobenius norm of the matrix 'mrx'
        public static T FrobeniusNorm<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentUncorrectSizeException("The matrix is empty", nameof(mrx));

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

        // Checks if matrix 'mrx' is a Toeplitz matrix
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
        // Transpose of square matrix 'mrx'
        public static void Transpose<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));
            if (!mrx.IsSquare()) throw new ArgumentUncorrectSizeException("The matrix isn't square", nameof(mrx));

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

        // Vertical reflection of matrix 'mrx'
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
        // Horizontal reflection of matrix 'mrx'
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
        // Returns a matrix of size rows x cols filled with elements from list 'lst' row by row
        public static Matrix<T> ConstructMatrix<T>(IList<T> lst, int rows, int cols) where T : IComparable<T>, INumber<T>
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
        // Rotate square matrix 'mrx' 90 degrees clockwise
        public static void ClockwiseRotate<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));
            if (!mrx.IsSquare()) throw new ArgumentUncorrectSizeException("The matrix isn't square", nameof(mrx));

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
        // Rotate square matrix 'mrx' 90 degrees counterclockwise
        public static void CounterclockwiseRotate<T>(Matrix<T> mrx) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (mrx.IsEmpty()) throw new ArgumentEmptyException("The matrix is empty", nameof(mrx));
            if (!mrx.IsSquare()) throw new ArgumentUncorrectSizeException("The matrix isn't square", nameof(mrx));

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
    }
}
