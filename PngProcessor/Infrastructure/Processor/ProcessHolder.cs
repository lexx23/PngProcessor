using System;
using System.Threading;

namespace PngProcessor.Infrastructure.Processor
{
    internal class ProcessHolder
    {
        private ImageProcessor.PngProcessor _pngProcessor;
        private ProcessStatusInfo _statusInfo;
        private string _filePath;
        private Thread _thread;

        public event EventHandler OnWorkDone;

        public ProcessStatusInfoBase Status => _statusInfo;

        public ProcessHolder(string filePath)
        {
            _filePath = filePath;

            _statusInfo = new ProcessStatusInfo();
            _thread = new Thread(WorkThread);
            _pngProcessor = new ImageProcessor.PngProcessor();
// fix;
            _pngProcessor.ProgressChanged += _pngProcessor_ProgressChanged;
        }

        private void _pngProcessor_ProgressChanged(double obj)
        {
            _statusInfo.SetProgress(obj);
        }

        private void WorkThread()
        {
            try
            {
                _pngProcessor.Process(_filePath);
                _statusInfo.SetStatus(ProcessStatusEnum.Done);
            }
            catch
            {
                _statusInfo.SetStatus(ProcessStatusEnum.Error);
            }
            finally
            {
                OnWorkDone?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Запуск обработки
        /// </summary>
        public void Start()
        {
            _statusInfo.SetStatus(ProcessStatusEnum.Working);
            _thread.Start();
        }

        /// <summary>
        /// Прервать процесс обработки
        /// </summary>
        public void Abort()
        {
            if (_thread.IsAlive)
            {
                _statusInfo.SetStatus(ProcessStatusEnum.Deleting);
                _thread.Abort();
            }

            OnWorkDone?.Invoke(this, new EventArgs());
        }
    }
}
