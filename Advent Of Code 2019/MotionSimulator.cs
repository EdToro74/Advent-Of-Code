using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    static class MotionSimulator
    {
        public class MotionObject
        {
            public Vector3 Position { get; }
            public Vector3 Velocity { get; } = new Vector3();

            public long PotentialEnergy => Math.Abs(Position.X) + Math.Abs(Position.Y) + Math.Abs(Position.Z);
            public long KineticEnergy => Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) + Math.Abs(Velocity.Z);
            public long TotalEnergy => PotentialEnergy * KineticEnergy;

            public MotionObject(Vector3 position)
            {
                Position = position;
            }

            public void ApplyGravity(IEnumerable<MotionObject> system)
            {
                var xDelta = system.Count(o => o.Position.X > Position.X) - system.Count(o => o.Position.X < Position.X);
                Velocity.X += xDelta;

                var yDelta = system.Count(o => o.Position.Y > Position.Y) - system.Count(o => o.Position.Y < Position.Y);
                Velocity.Y += yDelta;

                var zDelta = system.Count(o => o.Position.Z > Position.Z) - system.Count(o => o.Position.Z < Position.Z);
                Velocity.Z += zDelta;
            }

            public void ApplyVelocity()
            {
                Position.X += Velocity.X;
                Position.Y += Velocity.Y;
                Position.Z += Velocity.Z;
            }
        }

        public static IEnumerable<IEnumerable<MotionObject>> SimulateSystem(IEnumerable<string> input)
        {
            var system = input.Select(s =>
            {
                var parts = s.Replace("<", "").Replace(">", "").Replace(",", "").Split(" ");
                return new MotionObject(new Vector3() { X = long.Parse(parts[0].Split("=")[1]), Y = long.Parse(parts[1].Split("=")[1]), Z = long.Parse(parts[2].Split("=")[1]) });
            }).ToList();

            yield return system;

            while (true)
            {
                foreach (var motionObject in system)
                {
                    motionObject.ApplyGravity(system);
                }

                foreach (var motionObject in system)
                {
                    motionObject.ApplyVelocity();
                }

                yield return system;
            }
        }
    }
}
