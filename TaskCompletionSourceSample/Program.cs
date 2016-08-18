using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCompletionSourceSample
{
	public class ProducerConsumerPipeline
    {
        public Task StartConsumer()
        {
            var tcs = new TaskCompletionSource<object>();


            Task.Run(() =>
            {
                try
                {
                    Thread.Sleep(3000);
                    //throw new Exception("Consumer ex.");

                    Exception ex = new Exception("Consumer ex.");

                    tcs.SetException(ex);
                }
                catch (Exception e)
                {
                    Console.WriteLine(SynchronizationContext.Current?.ToString());
                    tcs.SetException(e);
                    Console.WriteLine(SynchronizationContext.Current?.ToString());
                }

            });

            return tcs.Task;
        }

        public Task StartProducer()
        {
            var tcs = new TaskCompletionSource<object>();


            Task.Run(() =>
            {
                try
                {
                    Thread.Sleep(6000);
                    throw new Exception("Producer ex.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(SynchronizationContext.Current?.ToString());
                    tcs.SetException(e);
                    Console.WriteLine(SynchronizationContext.Current?.ToString());
                }

            });

            return tcs.Task;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ProducerConsumerPipeline pipeline = new ProducerConsumerPipeline();

            Task producer = pipeline.StartProducer();
            Task consumer = pipeline.StartConsumer();

			Task<Task> resultedTask = Task.WhenAny(new[] { producer, consumer });
                        

            resultedTask.ContinueWith((task) =>
            {
				Console.WriteLine(SynchronizationContext.Current?.ToString());
				Task completedTask = task.Result;
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"Completed Task in State:{completedTask.Status}  with exception:");
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine($"{completedTask.Exception?.Flatten()?.InnerException}");

            });

			Console.WriteLine("Main thread waiting...");
			Console.ReadLine();
        }

 
    }
}
