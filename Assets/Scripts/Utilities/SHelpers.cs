using Unity.Mathematics;
using UnityEngine;

namespace GC.Utilities
{
    public static class SHelpers // StaticHelpers
    {
        public static float3 GetMouseWorldPos(Vector3 mouseScreenPosition)
        {
            if (Camera.main == null)
                return new Vector3(0, -1000, 0);

            return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        }
    }
}
