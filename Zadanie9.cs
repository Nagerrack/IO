using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IO_final
{
    class MatMulCalculator
    {
        public delegate void MatMulCompletedEventHandler(object sender, MatMulCompletedEventArgs e);

        private delegate void WorkerEventHandler(double[] mat1, double[] mat2, int size, AsyncOperation asyncOp);

        public event MatMulCompletedEventHandler MatMulCompleted;
        private SendOrPostCallback onCompletedCallback;
        private HybridDictionary userState = new HybridDictionary();

        public MatMulCalculator()
        {
            onCompletedCallback = new SendOrPostCallback(CalculateCompleted);
        }

        public void CalculateCompleted(object state)
        {
            MatMulCompletedEventArgs e = state as MatMulCompletedEventArgs;
            if (MatMulCompleted != null)
            {
                MatMulCompleted(this, e);
            }
        }

        void Completion(int size, double[] mat, Exception ex, bool cancelled, AsyncOperation ao)
        {
        }

        bool TaskCancelled(object TaskID)
        {
            return userState[TaskID] == null;
        }

        void CalculateWorker(double[] mat1, double[] mat2, int size, AsyncOperation asyncOp)
        {
            double mat3 = new double(); //funkcja do mnozenia;
            lock (userState.SyncRoot)
            {
                userState.Remove(asyncOp.UserSuppliedState);
            }
            object[] tabs = {mat1, mat2, mat3};
            MatMulCompletedEventArgs e = new MatMulCompletedEventArgs(tabs, null, false, asyncOp.UserSuppliedState);
            asyncOp.PostOperationCompleted(onCompletedCallback, e);
        }

        double getVal(double[] mat, int column, int row, int size)
        {
        }

        double MatMul(double[] mat1, double[] mat2, int size)
        {
        }

        public virtual void MatMulAsync(double[] mat1, double[] mat2, int size, object TaskID)
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
            //!!!!!
            worker.BeginInvoke(mat1, mat2, 1, ao, null, null);
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
    }

    class MatMulCompletedEventArgs : AsyncCompletedEventArgs
    {
        public int[] Array1 { get; }
        public int[] Array2 { get; }
        public int[]Array3 { get; }

        public MatMulCompletedEventArgs(object Arrays, Exception e, bool cancelled, object state)
            : base(e, cancelled, state)
        {
            Array1 = ((int[][]) Arrays)[0];
            Array2 = ((int[][]) Arrays)[1];
            Array3 = ((int[][]) Arrays)[2];
        }
    }

    class Zadanie9
    {
    }

    //class MatMulCalculator
    //{
    //    private delegate void MatMulCompleted;
    //    private HybridDictionary operations = new HybridDictionary();
    //}
}