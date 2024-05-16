using System.Linq;
using UnityEngine;

namespace Ceresta
{
    public static class CerestaGame
    {
        public static string baseUrl = "https://backend-test-k12i.onrender.com";

        public static void ClearChildren(this Transform t)
        {
            var children = t.Cast<Transform>().ToArray();

            foreach (var child in children)
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }
    }
}
