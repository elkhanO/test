//using Microsoft.Extensions.Caching.Distributed;
using BenchmarkDotNet.Running;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace TestNet5_Redis
{
    public class BankAccount
    {

        object _lock = new object();
        Mutex mutex = new Mutex();
        // SpinLock spinLock = new SpinLock();

        //  public bool _lockSpin;

        public int Balance { get; set; }

        public void Deposit(int amount)
        {
            //lock(_lock)
            //var _lockSpin = false;

            try
            {
                //    spinLock.Enter(ref _lockSpin);
                mutex.WaitOne();
                Balance += amount;
            }
            finally
            {
                mutex.ReleaseMutex();
                //if (_lockSpin)
                //    spinLock.Exit(false);
            }

        }

        public void WithDraw(int amount)
        {
            try
            {
                //    spinLock.Enter(ref _lockSpin);
                mutex.WaitOne();
                Balance -= amount;
            }
            finally
            {
                mutex.ReleaseMutex();
                //if (_lockSpin)
                //    spinLock.Exit(false);
            }

        }
    }

    public class Program
    {

        public static string Print(string p, CancellationToken cancellationToken)
        {

            int s = 5000;

            while (s-- > 0)
            {
                //if(cancellationToken.IsCancellationRequested)
                //    break;
                // if (cancellationToken.WaitHandle.WaitOne())
                //    Console.WriteLine(cancellationToken.WaitHandle.WaitOne());

                //cancellationToken.WaitHandle.WaitOne(8000);
                Thread.Sleep(8000);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                else
                {
                    Console.WriteLine(s + $"---------- {p} ----------------------" + Thread.CurrentThread.ManagedThreadId);

                }
                p += p + "---------" + s;
            }
            return p;
        }

        public static IEnumerable<int> Range(int from, int to, int step)
        {
            for (int i = from; i < to; i += step)
            {
                yield return i;
            }
        }
        public static void Demo()
        {

            var cts = new CancellationTokenSource();

            var parOptions = new ParallelOptions()
            {
                CancellationToken = cts.Token,
            };

            Parallel.For(1, 20, parOptions, (x, state) =>
            {
                Console.WriteLine(x);
                if (x == 10)
                {
                    // state.Stop();
                    //state.Break();
                    cts.Cancel();
                }
            });
        }

        public static Semaphore semaphore = null;


        [BenchmarkDotNet.Attributes.Benchmark]
        public void SquareEachValue()
        {
            int sum = 0;
            var count = Enumerable.Range(0, 10);

            Parallel.For(0, 1001,
            partialSum =>
            {
                Interlocked.Add(ref sum, partialSum);
            }
            );

            //const int count = 100000;
            //var values = Enumerable.Range(0, count);
            //var results = new int[count];
            //Parallel.ForEach(values, (value, index) => { results[value] = (int)Math.Pow(value, 2); });
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void SquareEachValuePart()
        {
            int sum = 0;
            var count = Enumerable.Range(0, 10);

            Parallel.For(0, 1001, () => 0, (x, state, tls) =>
            {
                tls += x;
                return tls;
            },
            partialSum =>
            {
                Interlocked.Add(ref sum, partialSum);
            }
            );

            //const int count = 100000;
            //var values = Enumerable.Range(0, count);
            //var results = new int[count];

            //var nese = Partitioner.Create(0, 100000, 10000);

            //Parallel.ForEach(nese, x =>
            //{
            //    for (int i = x.Item1; i < x.Item2; i++)
            //    {
            //        results[i] = (int)Math.Pow(i, 2);
            //    }
            //});
        }


        static void Main(string[] args)
        {
            //var res = BenchmarkRunner.Run<Program>();
            //Console.WriteLine(res);

            var items = Enumerable.Range(1, 45);

            var result = items.AsParallel().Select(x=>x*x*x);

            foreach (var item in result)
            {
                Console.WriteLine(item);
            }



            //Console.WriteLine(sum);





            //var a = new Action(() => Console.WriteLine($"a - {Task.CurrentId} - {Thread.CurrentThread.ManagedThreadId}"));
            //var b = new Action(() => Console.WriteLine($"b - {Task.CurrentId} - {Thread.CurrentThread.ManagedThreadId}"));
            //var c = new Action(() => Console.WriteLine($"c - {Task.CurrentId} - {Thread.CurrentThread.ManagedThreadId}"));


            //Parallel.Invoke(a, b, c);


            //Parallel.For(1, 6, (i) =>
            //{
            //    Console.WriteLine($"{i * i}");
            //});

            //var ss = new ParallelOptions()
            //{

            //};

            //Parallel.ForEach(Range(1, 20, 3), Console.WriteLine);



            //Console.WriteLine("d");

            //try
            //{
            //    semaphore = Semaphore.OpenExisting("semaphore"); 
            //}
            //catch (Exception)
            //{
            //    semaphore = new Semaphore(2,2,"semaphore");
            //}

            //Console.WriteLine("1");
            //semaphore.WaitOne();
            //Console.WriteLine("2");

            //Console.ReadKey();

            //semaphore.Release();
            //int count = 5;
            //CountdownEvent countEvent = new CountdownEvent(count);


            //for (int i = 0; i < count; i++)
            //{
            //    Task.Run(() =>
            //  {

            //      Console.WriteLine($"Enter {Thread.CurrentThread} and ProsessorID = {Thread.GetCurrentProcessorId()}");
            //      Thread.Sleep(3000);

            //      countEvent.Signal();

            //      Console.WriteLine($"Exiting {i}");


            //  });
            //}

            //Task task = Task.Run(() =>
            //{
            //    for (int i = 0; i < count; i++)
            //    {
            //        Console.WriteLine($"Enter {i}");
            //        Thread.Sleep(3000);
            //    }
            //    countEvent.Signal();

            //    for (int i = 0; i < count; i++)
            //    {
            //        Console.WriteLine($"Exiting {i}");
            //        Thread.Sleep(3000);
            //    }

            //});


            //Task task2 = Task.Run(() =>
            //{

            //    Console.WriteLine("New task");
            //    countEvent.Wait();
            //    Console.WriteLine("New task2222");

            //});


            //task2.Wait();

            //Task t1 = new Task(() =>
            //{
            //   Task t2 =new  Task(() =>
            //    {

            //        Console.WriteLine("Task1 started");
            //        throw new Exception();

            //    });

            //    t2.ContinueWith((x) =>
            //    {
            //        Console.WriteLine("Task1 Completed");
            //    }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnRanToCompletion);

            //    t2.ContinueWith((y) =>
            //    {

            //        Console.WriteLine("Error occured");
            //    }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnFaulted);


            //    t2.Start();
            //},TaskCreationOptions.AttachedToParent);

            //t1.Start();

            //try
            //{
            //    t1.Wait();

            //}
            //catch (AggregateException e)
            //{
            //    e.Handle(x => true);
            //}

            Console.ReadLine();




            //var tasks = new List<Task>();
            //BankAccount bankAccount = new BankAccount();    


            ////for (int i = 0; i < 5; i++)
            ////{

            //    tasks.Add(Task.Factory.StartNew(() =>
            //    {
            //        //for (int i = 0; i < 1000; i++)
            //        //{
            //                // Console.WriteLine(bankAccount.Balance + "+" + 1);
            //                bankAccount.WithDraw(100);
            //        //}
            //    }));


            //    tasks.Add(Task.Factory.StartNew(() =>
            //    {
            //        //for (int i = 0; i < 1000; i++)
            //        //{
            //            //Console.WriteLine(bankAccount.Balance + "-" + 1);
            //            bankAccount.WithDraw(100);
            //        //}
            //    }));
            ////}

            //Task.WaitAll(tasks.ToArray());

            //Console.WriteLine(bankAccount.Balance);
            //Console.ReadLine();

            ////setting up connection to the redis server  
            //ConnectionMultiplexer conn = ConnectionMultiplexer.Connect("localhost");
            ////getting database instances of the redis  
            //IDatabase database = conn.GetDatabase();
            ////set value in redis server  
            //database.StringSet("redisKey", "redisvalue");
            ////get value from redis server  
            // var value = database.StringGet("redisKey");

            //MemoryCache memoryCache = new MemoryCache();

            // var cts = new CancellationTokenSource();

            // cts.Token.Register(() => {

            //     Console.WriteLine("Canceled task yes");

            // });

            // Task t1 = new Task(() => {
            //     Print("c",cts.Token);

            //     },cts.Token);
            // t1.Start();

            // //  t1.Wait();

            // //Task.Run(() => Print("b"));



            // //Task.Factory.StartNew(() => Print("a"));

            // Console.ReadKey();
            //cts.Cancel();

            // Console.WriteLine(t1.Status);

            // Console.WriteLine("Value cached in redis server is: ");
            // Console.ReadLine();



            // Console.WriteLine("Hello World!");


            //IEnumerable<int> numbers = Enumerable.Range(0, 10000);
            //CancellationTokenSource tokenSource = new CancellationTokenSource();
            //CancellationToken token = tokenSource.Token;

            //Task startedTask = Task.Factory.StartNew(() =>
            //{
            //    for (int i = 0; i < numbers.Count(); i++)
            //    {
            //        //  Thread.Sleep(8000);
            //        token.WaitHandle.WaitOne(8000);

            //        i++;
            //        i--;
            //        i *= 2;
            //        Console.WriteLine("Time : {0}...", DateTime.Now.ToLongTimeString());
            //        //token.ThrowIfCancellationRequested();
            //    }
            //}
            //, token);

            //token.Register(() =>
            //{
            //    Console.WriteLine("Time : {0} , İşlem iptali", DateTime.Now.ToLongTimeString());
            //}
            //);

            //Console.WriteLine("İşlemler devam ediyor. İptal etmek için bir tuşa basınız.");
            //Console.ReadLine();

            //tokenSource.Cancel();

            //Console.ReadLine();
            //Console.WriteLine("Time : {0} , Task 1 Status = {1}", DateTime.Now.ToLongTimeString(), startedTask.Status);
        }
    }


    //public class MemoryCache
    //{

    //    private readonly IDistributedCache _distributedCache;

    //    public MemoryCache(IDistributedCache distributedCache)
    //    {
    //        _distributedCache = distributedCache;
    //    }


    //    public string GetValue()
    //    {

    //        return _distributedCache.GetString("redisKey");
    //    }








    //}


}
