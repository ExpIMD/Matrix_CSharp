using System.Numerics;
using System;
using System.Collections;

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
            get => this.Rows * this.Cols;
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
        public static bool operator==(Matrix<T> a, Matrix<T> b)
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
        public static bool operator!=(Matrix<T> a, Matrix<T> b)
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
            this.__data = new T[rows, cols];

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    this.__data[i, j] = value;
        }
        public Matrix(int rows, int cols, Func<int, int, T> F)
        {
            if (rows < 0) throw new ArgumentOutOfRangeException("The number of rows is negative", nameof(rows));
            if (cols < 0) throw new ArgumentOutOfRangeException("The number of rows is negative", nameof(cols));
            if (F is null) throw new ArgumentNullException("The function is null", nameof(F));

            this.__data = new T[rows, cols];

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    this.__data[i, j] = F(i, j);
        }
        public Matrix(T[,] data)
        {
            if (data is null) throw new ArgumentNullException("The data is null", nameof(data));

            int rows = data.GetLength(0), cols = data.GetLength(1);
            this.__data = new T[rows, cols];

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    this.__data[i, j] = data[i, j];
        }

        public bool IsEmpty()
        {
            return this.__data is null;
        }
        public bool IsSquare()
        {
            if (IsEmpty()) return false;
            return this.Rows == this.Cols;
        }
        public int Count()
        {
            return this.Size;
        }

        public object Clone()
        {
            int rows = this.Rows, cols = this.Cols;
            Matrix<T> result = new Matrix<T>(rows, cols);

            for(int i = 0; i < rows; ++i)
                for(int j = 0; j < cols; ++j)
                    result[i, j] = this.__data[i, j];

            return result;
        }
        public override bool Equals(object? obj)
        {
            if (obj is null) throw new ArgumentNullException("The object is null", nameof(obj));
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not Matrix<T> temp) return false;

            int rows1 = this.Rows, cols1 = this.Cols, rows2 = temp.Rows, cols2 = temp.Cols;
            
            if (rows1 != rows2 || cols1 != cols2) return false;

            for(int i = 0; i < rows1; ++i)
                for(int j = 0; j < cols1; ++j)
                    if (!EqualityComparer<T>.Default.Equals(this.__data[i, j], temp.__data[i, j]))
                        return false;

            return true;
        }
        public override int GetHashCode()
        {
            int result = 17, rows = this.Rows, cols = this.Cols;

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    result = result * 31 + EqualityComparer<T>.Default.GetHashCode(this.__data[i, j]);

            return result;
        }
        public IEnumerator<T> GetEnumerator()
        {
            int rows = this.Rows, cols = this.Cols;

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    yield return this.__data[i, j];
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void CheckBounds(int row, int col)
        {
            if (row < 0 || row >= this.Rows) throw new ArgumentOutOfRangeException("The row index is out of bounds", nameof(row));
            if (col < 0 || col >= this.Cols) throw new ArgumentOutOfRangeException("The col index is out of bounds", nameof(col));
        }
    }

    public static class MatrixMethods
    {
        // Printing methods

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

        // Ordering methods

        // Traversal from the upper left corner of the matrix 'mrx' along the diagonals with alternating directions (top to bottom and bottom to top) applying action 'A' to each element
        public static void DiagonalOrderWithAlternation<T>(Matrix<T> mrx, Action<T> A) where T : IComparable<T>, INumber<T>
        {
            if (mrx is null) throw new ArgumentNullException("The matrix is null", nameof(mrx));
            if (A is null) throw new ArgumentNullException("The action is null", nameof(A));

            if (mrx.IsEmpty()) return;

            int rows = mrx.Rows, cols = mrx.Cols, currentRow = 0, currentCol = 0;
            
            while(currentRow < rows && currentCol < cols)
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

        // Conversion methods

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
    }
}
