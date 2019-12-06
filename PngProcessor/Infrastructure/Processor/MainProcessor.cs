using System;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;

namespace PngProcessor.Infrastructure.Processor
{
    public class MainProcessor : IDisposable, IMainProcessor
    {
        private ConcurrentDictionary<string, ProcessHolder> _status;
        private ConcurrentQueue<string> _queue;
        private ManualResetEventSlim _event;
        private Thread _workThread;
        private int _processed;
        private string _path;

        /// <summary>
        /// Длина очереди
        /// </summary>
        public int QueueLength => _queue.Count;
        /// <summary>
        /// Количество отработанных задач
        /// </summary>
        public int Processed => _processed;

        public MainProcessor(string path = null)
        {
            if (string.IsNullOrEmpty(path))
                path = "upload";
            _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

            _processed = 0;

            _status = new ConcurrentDictionary<string, ProcessHolder>();
            _queue = new ConcurrentQueue<string>();
            _event = new ManualResetEventSlim(false);
            _workThread = new Thread(MainProcessorLoop);
            _workThread.Start();
        }

        /// <summary>
        /// Главный цикл обработчика, выполняется в отделном потоке.
        /// </summary>
        private void MainProcessorLoop()
        {
            string id;
            while (true)
            {
                _event.Wait();

                while (_queue.TryDequeue(out id))
                {
                    // если в списке статусов нет id, значит не обрабатываем
                    ProcessHolder result;
                    if (_status.TryGetValue(id, out result))
                    {
                        result.OnWorkDone += ProcessHolderOnWorkDone;
                        result.Start();
                    }
                }

                _event.Reset();
            }
        }

        /// <summary>
        /// Получение статуса обработчика по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProcessStatusInfoBase GetStatus(string id)
        {
            ProcessHolder result;
            if (_status.TryGetValue(id, out result))
                return result.Status;

            return null;
        }

        /// <summary>
        /// Добавить id в очередь обработки
        /// </summary>
        /// <param name="id"></param>
        public void Add(string id)
        {
            if (_status.ContainsKey(id))
                return;

            _queue.Enqueue(id);
            _status.TryAdd(id, new ProcessHolder(Path.Combine(_path,id)));
            _event.Set();
        }

        /// <summary>
        /// Удалить id из очереди на обработку
        /// </summary>
        /// <param name="id"></param>
        public void Remove(string id)
        {
            ProcessHolder result;
            if (_status.TryRemove(id, out result))
            {
                result.Abort();
                result.OnWorkDone -= ProcessHolderOnWorkDone;
            }
        }


        private void ProcessHolderOnWorkDone(object sender, EventArgs e)
        {
            Interlocked.Increment(ref _processed);
        }

        public void Dispose()
        {
            foreach(var current in _status)
                current.Value.Abort();
        }
    }
}