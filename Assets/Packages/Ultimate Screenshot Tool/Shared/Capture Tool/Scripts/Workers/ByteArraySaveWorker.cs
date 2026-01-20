using UnityEngine;
using System.Collections;
using System.Threading;
using ThreadPriority = System.Threading.ThreadPriority;

namespace TRS.CaptureTool
{
    internal sealed class ByteArraySaveWorker
    {
        static int workerId = 1;

        Thread thread;
        public int id;

        internal byte[] bytes;
        internal string filePath;
        internal System.Action<int, string, bool> OnFileSaved;

        internal ByteArraySaveWorker(ThreadPriority priority)
        {
            id = workerId++;
            thread = new Thread(Run);
            thread.Priority = priority;
        }

        internal void Start()
        {
            thread.Start();
        }

        void Run()
        {
            IEnumerator saveBytes = SaveBytes();
            while (saveBytes.MoveNext()) { }
        }

        internal IEnumerator SaveBytes()
        {
            bool savedSuccessfuly = false;
            try
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
                System.IO.File.WriteAllBytes(filePath, bytes);
                savedSuccessfuly = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Exception attempting to save texture: " + e);
            }

            if (OnFileSaved != null)
            {
                UnityToolbag.Dispatcher.Invoke(() =>
                {
                    OnFileSaved(id, filePath, savedSuccessfuly);
                });
            }

            FlushMemory();
            yield break;
        }

        internal void FlushMemory()
        {
            bytes = null;
            filePath = null;
            OnFileSaved = null;
            thread = null;

            //System.GC.Collect();
            //System.GC.WaitForPendingFinalizers();
            //System.GC.Collect();
        }
    }
}