using UnityEngine;

namespace Utils
{
    public static class Layers
    {
        public static int Unit => LayerMask.NameToLayer(nameof(Unit));
    }
}