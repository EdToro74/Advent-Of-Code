using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    public static class Day25
    {
        enum Mode
        {
            Exploring,
            TestingWeights
        }

        public static string Part1(IEnumerable<string> input)
        {
            var command = string.Empty;
            var commandIndex = 0;

            var dangerousItems = new[] { "molten lava", "giant electromagnet", "infinite loop", "escape pod", "photons" };

            var room = string.Empty;
            var nodes = new Dictionary<string, Node>();
            var inventory = new HashSet<string>();
            string lastDirection = null;

            Node currentNode = null;
            var currentPath = new Queue<string>();
            var mode = Mode.Exploring;
            var inventoryAttempts = new Queue<IEnumerable<string>>();
            var currentInventoryAttempt = Enumerable.Empty<string>();

            string OppositeDirection(string direction) => direction switch
            {
                "north" => "south",
                "south" => "north",
                "east" => "west",
                "west" => "east",
                _ => throw new Exception($"Unknown direction {direction}"),
            };

            long inputHandler()
            {
                if (!string.IsNullOrWhiteSpace(command) && commandIndex == command.Length)
                {
                    command = string.Empty;
                    return 10;
                }

                if (string.IsNullOrWhiteSpace(command))
                {
                    if (mode == Mode.Exploring)
                    {
                        if (currentPath.Any())
                        {
                            command = currentPath.Dequeue();
                        }
                        else if (currentNode != null)
                        {
                            if (currentNode.Items.Any(item => !dangerousItems.Contains(item)))
                            {
                                command = $"take {currentNode.Items.First()}";
                            }
                            else if (currentNode.UnexploredExits.Any(direction => OppositeDirection(direction) != lastDirection))
                            {
                                command = currentNode.UnexploredExits.First(direction => OppositeDirection(direction) != lastDirection);
                            }
                            else
                            {
                                var targetNode = nodes.Values.FirstOrDefault(n => n.UnexploredExits.Any()) ?? nodes["Security Checkpoint"];
                                if (targetNode != null && currentNode != targetNode)
                                {
                                    var path = PathFinding.FindPath(currentNode, targetNode, (_, __) => 1, (_, __) => 0, node => node.Neighbors.Select(n => n.location));
                                    if (!path.success)
                                    {
                                        throw new Exception($"Couldn't path from {currentNode.Name} to {targetNode.Name}");
                                    }

                                    currentPath.Clear();

                                    Node currentPathNode = null;
                                    foreach (var node in path.path.Reverse())
                                    {
                                        if (currentPathNode == null)
                                        {
                                            currentPathNode = node;
                                            continue;
                                        }

                                        var direction = currentPathNode.Neighbors.Where(kvp => kvp.location == node).Select(kvp => kvp.direction).Single();
                                        currentPath.Enqueue(direction);
                                        currentPathNode = node;
                                    }

                                    if (currentPath.Any())
                                    {
                                        command = currentPath.Dequeue();
                                    }
                                }
                                else
                                {
                                    // We've explored everywhere and gathered all we can, now try different combos until we get the correct weight
                                    mode = Mode.TestingWeights;
                                    return inputHandler();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (inventoryAttempts.Count == 0 && !currentInventoryAttempt.Any())
                        {
                            var powerSet = PowerSet.FastPowerSet(inventory.ToArray());
                            foreach (var set in powerSet)
                            {
                                inventoryAttempts.Enqueue(set);
                            }

                            currentInventoryAttempt = inventoryAttempts.Dequeue();
                        }

                        if (!currentInventoryAttempt.Any())
                        {
                            if (inventoryAttempts.Count == 0)
                            {
                                throw new Exception("Tried all inventory combinations");
                            }
                            currentInventoryAttempt = inventoryAttempts.Dequeue();
                        }

                        foreach (var item in inventory)
                        {
                            if (!currentInventoryAttempt.Contains(item))
                            {
                                command = $"drop {item}";
                                break;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(command))
                        {
                            foreach (var item in currentNode.Items)
                            {
                                if (currentInventoryAttempt.Contains(item))
                                {
                                    command = $"take {item}";
                                    break;
                                }
                            }
                        }

                        if (string.IsNullOrWhiteSpace(command))
                        {
                            command = "north";
                            currentInventoryAttempt = Enumerable.Empty<string>();
                        }
                    }

                    if (string.IsNullOrWhiteSpace(command))
                    {
                        while (string.IsNullOrWhiteSpace(command))
                        {
                            command = Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine(command);
                    }

                    commandIndex = 0;
                }

                if (currentNode != null && currentNode.Exits.Contains(command))
                {
                    lastDirection = command;
                }

                var c = command[commandIndex++];

                return c;
            };

            foreach (var c in IntCodeProcessor.ProcessProgramEnumerable(input, inputHandler))
            {
                room += (char)c;
                if (room.EndsWith("Command?\n"))
                {
                    var node = ParseText(room, currentNode, inventory, nodes);
                    if (node != null)
                    {
                        nodes[node.Name] = node;
                        if (currentNode != null && lastDirection != null)
                        {
                            currentNode.AddNeighbor(lastDirection, node);
                            currentNode.AddExplored(lastDirection);
                            node.AddNeighbor(OppositeDirection(lastDirection), currentNode);
                            node.AddExplored(OppositeDirection(lastDirection));
                        }
                        currentNode = node;
                    }
                    room = string.Empty;
                }
                Console.Write((char)c);
                if (room.EndsWith(" on the keypad at the main airlock.\"\n"))
                {
                    Console.WriteLine("Final inventory: ");
                    foreach (var item in inventory)
                    {
                        Console.WriteLine(item);
                    }

                    return string.Join("", room.Where(c => char.IsDigit(c)));
                }
            }

            return string.Empty;
        }

        private static Node ParseText(string room, Node currentNode, HashSet<string> inventory, Dictionary<string, Node> nodes)
        {
            var lines = room.Split((char)10, StringSplitOptions.RemoveEmptyEntries);

            string name = null;
            var exits = new List<string>();

            var items = new List<string>();

            var inExits = false;
            var inItems = false;

            foreach (var line in lines)
            {
                if (line.StartsWith("You take the "))
                {
                    var item = line.Replace("You take the ", "").TrimEnd('.');
                    if (!inventory.Add(item))
                    {
                        throw new Exception($"Already had the {item} in inventory!");
                    }
                    currentNode.ItemTaken(item);
                }
                if (line.StartsWith("You drop the "))
                {
                    var item = line.Replace("You drop the ", "").TrimEnd('.');
                    if (!inventory.Remove(item))
                    {
                        throw new Exception($"Didn't have the {item} in inventory but dropped it!");
                    }
                    currentNode.ItemDropped(item);
                }
                if (line.StartsWith("== ") && line.EndsWith(" =="))
                {
                    if (name != null)
                    {
                        inExits = false;
                        inItems = false;
                        items = new List<string>();
                        exits = new List<string>();
                    }
                    name = line[3..^3];
                }

                if (line == "Doors here lead:")
                {
                    inExits = true;
                }
                else if (line == "Items here:")
                {
                    inExits = false;
                    inItems = true;
                }
                else if (line == "Command?")
                {
                    break;
                }
                else if (inExits)
                {
                    exits.Add(line.Substring(2));
                }
                else if (inItems)
                {
                    items.Add(line.Substring(2));
                }
            }

            if (name != null)
            {
                if (!nodes.TryGetValue(name, out var node))
                {
                    node = new Node(name, exits, items);
                }
                return node;
            }

            return null;
        }

        public class Node
        {
            public string Name { get; }
            public IEnumerable<string> Exits { get; }
            public IEnumerable<string> Items { get; }
            public IEnumerable<(string direction, Node location)> Neighbors => _neighbors.Select(kvp => (kvp.Key, kvp.Value));

            private HashSet<string> _exitsTaken = new HashSet<string>();

            private Dictionary<string, Node> _neighbors = new Dictionary<string, Node>();

            public Node(string name, IEnumerable<string> exits, IEnumerable<string> items)
            {
                Name = name;
                Exits = new HashSet<string>(exits);
                Items = new HashSet<string>(items);
            }

            public void AddExplored(string direction)
            {
                if (!Exits.Contains(direction))
                {
                    throw new Exception("That direction is not a valid exit!");
                }

                _exitsTaken.Add(direction);
            }

            public IEnumerable<string> UnexploredExits => Exits.Except(_exitsTaken);

            public void ItemTaken(string item)
            {
                ((HashSet<string>)Items).Remove(item);
            }

            public void ItemDropped(string item)
            {
                ((HashSet<string>)Items).Add(item);
            }

            public void AddNeighbor(string lastDirection, Node node)
            {
                _neighbors[lastDirection] = node;
            }
        }
    }
}
