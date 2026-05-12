using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace ECS
{
    public enum SizeCommand { Increment, Decrement, Double, Halve }

    public static class SpawnerCommon
    {
        private const float RippleSpeed = 1f / 25f;

        private static readonly Queue<SizeCommand> _pendingCommands = new Queue<SizeCommand>();

        public static void Enqueue(SizeCommand command) => _pendingCommands.Enqueue(command);

        public static bool TryHandleSizeInput(ref int size)
        {
            var kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.numpad2Key.wasPressedThisFrame) _pendingCommands.Enqueue(SizeCommand.Increment);
                if (kb.numpad1Key.wasPressedThisFrame) _pendingCommands.Enqueue(SizeCommand.Decrement);
                if (kb.numpad5Key.wasPressedThisFrame) _pendingCommands.Enqueue(SizeCommand.Double);
                if (kb.numpad4Key.wasPressedThisFrame) _pendingCommands.Enqueue(SizeCommand.Halve);
            }

            if (_pendingCommands.Count == 0) return false;

            while (_pendingCommands.Count > 0)
            {
                switch (_pendingCommands.Dequeue())
                {
                    case SizeCommand.Increment: size++; break;
                    case SizeCommand.Decrement: size = math.max(0, size - 1); break;
                    case SizeCommand.Double:    size *= 2; break;
                    case SizeCommand.Halve:     size = math.max(0, size / 2); break;
                }
            }
            return true;
        }

        public static float3 GridPosition(int x, int z, int size)
        {
            return new float3(x - size / 2f, 0f, z - size / 2f) / 2f;
        }

        public static float ComputeRipplePhase(int x, int z, int size)
        {
            float center = size / 2f;
            float dx = x - center;
            float dz = z - center;
            float dist = math.sqrt(dx * dx + dz * dz);
            return (size * RippleSpeed) * 2f * math.PI * (dist / size);
        }
    }
}
