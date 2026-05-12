using System;
using UnityEngine;

namespace ECS
{
    public static class SpawnerEventChannel
    {
        public static Action<int> OnSpawn;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            OnSpawn = null;
        }
    }
}
