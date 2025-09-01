using System.Numerics;
using System;

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
    public class Matrix<T> : ICloneable where T : IComparable<T>, INumber<T>
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

        private void CheckBounds(int row, int col)
        {
            if (row < 0 || row >= this.Rows) throw new ArgumentOutOfRangeException("The row index is out of bounds", nameof(row));
            if (col < 0 || col >= this.Cols) throw new ArgumentOutOfRangeException("The col index is out of bounds", nameof(col));
        }
    }

    public static class MatrixMethods
    {
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
    }
}
