using Microsoft.VisualStudio.TestTools.UnitTesting;
using PngProcessor.Infrastructure.Processor;
using System.Threading;
using System.Diagnostics;

namespace PngProcessor.Tests
{
    [TestClass]
    public class MainProcessorIntegrationTest
    {
        /// <summary>
        /// Добавление задачи и проверка ее отработки
        /// </summary>
        [TestMethod]
        public void MainProcessor_Add()
        {
            IMainProcessor processor = new MainProcessor();
            Assert.AreEqual(0, processor.QueueLength);

            processor.Add("1");
            Assert.AreEqual(1, processor.QueueLength);
            while (processor.Processed == 0)
                Thread.Yield();

            Assert.AreEqual(0, processor.QueueLength);
            Assert.AreEqual(1, processor.Processed);
            Assert.AreEqual(ProcessStatusEnum.Done, processor.GetStatus("1").Status);
        }

        /// <summary>
        /// Проверка удаления работающей задачи
        /// </summary>
        [TestMethod]
        public void MainProcessor_Remove()
        {
            IMainProcessor processor = new MainProcessor();
            Assert.AreEqual(0, processor.QueueLength);

            processor.Add("1");
            Assert.AreEqual(1, processor.QueueLength);

            while (processor.QueueLength > 0)
                Thread.Yield();

            processor.Remove("1");


            Assert.AreEqual(0, processor.QueueLength);
            Assert.IsNull(processor.GetStatus("1"));
        }

        /// <summary>
        /// Проверка выдачи итоговых данных для нескольких задач
        /// </summary>
        [TestMethod]
        public void MainProcessor_Progress()
        {
            IMainProcessor processor = new MainProcessor();
            Assert.AreEqual(0, processor.QueueLength);

            processor.Add("1");
            processor.Add("2");
            processor.Add("3");
            processor.Add("4");
            processor.Add("5");

            bool exit;
            while (true)
            {
                exit = true;
                for (int i = 1; i < 6; i++)
                {
                    Thread.Yield();
                    if (processor.GetStatus(i.ToString()).Status != ProcessStatusEnum.Done)
                    {
                        exit = false;
                        break;
                    }
                }

                if (exit)
                    break;
            }


            for (int i = 1; i < 6; i++)
            {
                Assert.AreEqual(ProcessStatusEnum.Done, processor.GetStatus(i.ToString()).Status);
                Assert.AreNotEqual(0.0, processor.GetStatus(i.ToString()).Progress);
                Trace.WriteLine($"{i.ToString()} - {processor.GetStatus(i.ToString()).Progress.ToString()}");
            }

            Assert.AreEqual(5, processor.Processed);
        }
    }
}
