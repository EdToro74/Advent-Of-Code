using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Advent_Of_Code_2019
{
    public static class Day23
    {
        public static long Part1(IEnumerable<string> input)
        {
            var copies = 50;

            PrepareNetwork(input, copies, out var networkStates, out var nodes, out var cancellationTokenSource);

            ThreadPool.GetMinThreads(out var workerMin, out var cpMin);
            _ = ThreadPool.SetMinThreads(copies, cpMin);

            var tasks = new Task<long>[copies];

            for (var i = 0; i < copies; i++)
            {
                var node = nodes[i];

                tasks[i] = Task.Run(() =>
                {
                    var enumerator = node.node.GetEnumerator();

                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        if (!enumerator.MoveNext())
                        {
                            return 0;
                        }

                        var address = enumerator.Current;
                        _ = enumerator.MoveNext();
                        var x = enumerator.Current;
                        _ = enumerator.MoveNext();
                        var y = enumerator.Current;

                        //Console.WriteLine($"Node {node.id} sending {x}, {y} to {address}");

                        if (address == 255)
                        {
                            return y;
                        }

                        networkStates[address].QueueMessage(x, y);
                    }

                    return 0;
                }, cancellationTokenSource.Token);
            }

            var index = Task.WaitAny(tasks);

            cancellationTokenSource.Cancel();

            return tasks[index].Result;
        }

        public static long Part2(IEnumerable<string> input)
        {
            var copies = 50;

            PrepareNetwork(input, copies, out var networkStates, out var nodes, out var cancellationTokenSource);

            var tasks = new Task[copies];

            ThreadPool.GetMinThreads(out var workerMin, out var cpMin);
            _ = ThreadPool.SetMinThreads(copies + 1, cpMin);

            var natLock = new object();
            var natMessage = (x: -1L, y: -1L);
            var natYs = new HashSet<long>();

            for (var i = 0; i < copies; i++)
            {
                var node = nodes[i];

                tasks[i] = Task.Run(() =>
                {
                    var enumerator = node.node.GetEnumerator();

                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        if (!enumerator.MoveNext())
                        {
                            return;
                        }

                        var address = enumerator.Current;
                        _ = enumerator.MoveNext();
                        var x = enumerator.Current;
                        _ = enumerator.MoveNext();
                        var y = enumerator.Current;

                        //Console.WriteLine($"Node {node.id} sending {x}, {y} to {address}");

                        if (address == 255)
                        {
                            lock (natLock)
                            {
                                //Console.WriteLine($"Node {node.id} sending {x}, {y} to NAT");
                                natMessage = (x, y);
                            }
                        }
                        else
                        {
                            networkStates[address].QueueMessage(x, y);
                        }
                    }
                }, cancellationTokenSource.Token);
            }

            var natTask = Task.Run(() =>
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    if (natMessage == (-1, -1))
                    {
                        continue;
                    }

                    if (networkStates.All(s => s.Idle))
                    {
                        lock (natLock)
                        {
                            if (natYs.Contains(natMessage.y))
                            {
                                return natMessage.y;
                            }

                            _ = natYs.Add(natMessage.y);
                            //Console.WriteLine($"Network idle, sending {natMessage} to 0");
                            networkStates[0].QueueMessage(natMessage.x, natMessage.y);
                            natMessage = (-1, -1);
                        }
                    }
                }

                return -1;
            });

            natTask.Wait();

            cancellationTokenSource.Cancel();

            return natTask.Result;
        }

        private static void PrepareNetwork(IEnumerable<string> input, int copies, out NetworkState[] networkStates, out (int id, IEnumerable<long> node)[] nodes, out CancellationTokenSource cancellationTokenSource)
        {
            var program = IntCodeProcessor.ParseProgram(input);
            cancellationTokenSource = new CancellationTokenSource();

            networkStates = new NetworkState[copies];
            nodes = new (int id, IEnumerable<long> node)[copies];
            for (var i = 0; i < copies; i++)
            {
                var networkState = new NetworkState(i);
                networkStates[i] = networkState;

                var node = IntCodeProcessor.ProcessProgramEnumerable(program.Copy(), networkState.InputHandler, cancellationTokenSource);
                nodes[i] = (i, node);
            }
        }

        private class NetworkState
        {
            private int _state = 0;
            private readonly Queue<long> _queue = new Queue<long>();
            private readonly int _id;
            private long _lastX = 0;

            public NetworkState(int id) => _id = id;

            public bool Idle { get; private set; }

            public long InputHandler()
            {
                switch (_state)
                {
                    // Boot
                    case 0:
                        _state = 1;
                        return _id;
                    // Get X
                    case 1:
                        lock (_queue)
                        {
                            if (_queue.Count == 0)
                            {
                                Idle = true;
                                return -1;
                            }

                            if (_queue.Count % 2 != 0)
                            {
                                throw new Exception("Odd number of messages waiting when they should all be X/Y pairs");
                            }

                            Idle = false;
                            _state = 2;
                            _lastX = _queue.Dequeue();
                            return _lastX;
                        }
                    // Get Y
                    case 2:
                        lock (_queue)
                        {
                            if (_queue.Count == 0)
                            {
                                throw new Exception("Returned X but now there is no Y");
                            }

                            _state = 1;
                            //Console.WriteLine($"Node {_id} receiving {_lastX}, {_queue.Peek()}");
                            return _queue.Dequeue();
                        }
                    default:
                        throw new Exception($"Unknown state {_state}");
                }
            }

            public void QueueMessage(long x, long y)
            {
                lock (_queue)
                {
                    _queue.Enqueue(x);
                    _queue.Enqueue(y);
                    Idle = false;
                }
            }
        }
    }
}
