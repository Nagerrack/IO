using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IO_final
{
    class MatMulCalculator
    {
        public delegate void MatMulCompletedEventHandler(object sender, MatMulCompletedEventArgs e);

        private delegate void WorkerEventHandler(double[][] mat1, double[][] mat2, AsyncOperation asyncOp);

        public event MatMulCompletedEventHandler MatMulCompleted;
        private SendOrPostCallback onCompletedCallback;
        private HybridDictionary userState = new HybridDictionary();

        public MatMulCalculator()
        {
            onCompletedCallback = new SendOrPostCallback(CalculateCompleted);
            MatMulCompleted += onCompletion;
        }

        public void CalculateCompleted(object state)
        {
            MatMulCompletedEventArgs e = state as MatMulCompletedEventArgs;
            if (MatMulCompleted != null)
            {
                MatMulCompleted(this, e);
            }
        }

        public static double[][] RandomMat(int size)
        {
            Random r = new Random();
            double[][] mat = new double[size][];
            for (int i = 0; i < size; i++)
            {
                mat[i] = new double[size];
                for (int j = 0; j < size; j++)
                {
                    mat[i][j] = r.NextDouble() * 100;
                }
            }
            return mat;
        }

        public static void onCompletion(object sender, MatMulCompletedEventArgs e)
        {
            Console.WriteLine("Mat1:");
            MatMulCalculator.Print(e.Array1);
            Console.WriteLine("Mat2:");
            MatMulCalculator.Print(e.Array2);
            Console.WriteLine("Mat3:");
            MatMulCalculator.Print(e.Array3);
        }
        void Completion(int size, double[][] mat, Exception ex, bool cancelled, AsyncOperation ao)
        {
        }

        bool TaskCancelled(object TaskID)
        {
            return userState[TaskID] == null;
        }

        void CalculateWorker(double[][] mat1, double[][] mat2, AsyncOperation asyncOp)
        {
            double[][] mat3 = MatMul(mat1, mat2);
            lock (userState.SyncRoot)
            {
                userState.Remove(asyncOp.UserSuppliedState);
            }
            object[] mats = {mat1, mat2, mat3};
            MatMulCompletedEventArgs e = new MatMulCompletedEventArgs(mats, null, false, asyncOp.UserSuppliedState);
            asyncOp.PostOperationCompleted(onCompletedCallback, e);
        }

        double[][] MatMul(double[][] mat1, double[][] mat2)
        {
            double[][] mat3 = new double[mat1.Length][];
            for (int i = 0; i < mat1.Length; i++)
            {
                mat3[i] = new double[mat1.Length];
            }

            for (int i = 0; i < mat1.Length; i++)
            {
                for (int j = 0; j < mat1.Length; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < mat1.Length; k++)
                    {
                        sum += mat1[i][k] * mat2[k][j];
                    }
                    mat3[i][j] = sum;
                }
            }
            return mat3;
        }

        public virtual void MatMulAsync(double[][] mat1, double[][] mat2, object TaskID)
        {
            AsyncOperation ao = AsyncOperationManager.CreateOperation(TaskID);
            lock (userState.SyncRoot)
            {
                if (userState.Contains(TaskID))
                {
                    throw new ArgumentException("ArgumentError", "TaskID");
                }
                userState[TaskID] = ao;
            }
            WorkerEventHandler worker = new WorkerEventHandler(CalculateWorker);
            worker.BeginInvoke(mat1, mat2, ao, null, null);
        }

        public void CancelAsync(object TaskID)
        {
            AsyncOperation ao = userState[TaskID] as AsyncOperation;
            if (ao != null)
            {
                lock (userState.SyncRoot)
                {
                    userState.Remove(TaskID);
                }
            }
        }

        public static void Print(double[][] mat)
        {
            foreach (var i in mat)
            {
                foreach (var j in i)
                {
                    Console.Write(Math.Round(j,2) + " ");
                }
                Console.WriteLine();
            }
        }
    }

    class MatMulCompletedEventArgs : AsyncCompletedEventArgs
    {
        public double[][] Array1 { get; }
        public double[][] Array2 { get; }
        public double[][] Array3 { get; }

        public MatMulCompletedEventArgs(object Arrays, Exception e, bool cancelled, object state)
            : base(e, cancelled, state)
        {
            Array1 = (double[][]) ((object[]) Arrays)[0];
            Array2 = (double[][]) ((object[]) Arrays)[1];
            Array3 = (double[][]) ((object[]) Arrays)[2];
        }
    }

    class Zadanie9
    {
        public static void Start()
        {
            int TaskID = 1;
            MatMulCalculator mmc = new MatMulCalculator();
            

            for (int i = 0; i < 2; i++)
            {
                double[][] mat1 = MatMulCalculator.RandomMat(3);
                Thread.Sleep(50);
                double[][] mat2 = MatMulCalculator.RandomMat(3);
                mmc.MatMulAsync(mat1,mat2,TaskID);
                TaskID++;
                
            }
            Thread.Sleep(1000);
        }

        
    }
}