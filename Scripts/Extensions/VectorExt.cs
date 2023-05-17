using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace VectorExt
{
    public static class VectorExt
    {
        public static bool inRange(this Vector2 vect, Vector2 minVect, Vector2 maxVect)
        {
            return (vect.x >= minVect.x && vect.x < maxVect.x && vect.y >= minVect.y && vect.y < maxVect.y);
        }

        public static bool inRange(this Vector3 vect, Vector3 minVect, Vector3 maxVect)
        {
            return ((vect.x >= minVect.x && vect.x < maxVect.x) && (vect.y >= minVect.y && vect.y < maxVect.y) && (vect.z >= minVect.z && vect.z < maxVect.z));
        }
    }
}
