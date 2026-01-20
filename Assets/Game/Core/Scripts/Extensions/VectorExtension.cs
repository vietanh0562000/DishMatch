using UnityEngine;

namespace BasePuzzle
{
    public static class VectorExtension
    {
        /// <summary>
        /// Add a vector with a vector
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="vector2Add"></param>
        /// <returns></returns>
        public static Vector2 Add(this Vector2 origin, Vector2 vector2Add)
        {
            Vector2 sum = Vector2.zero;

            sum.x = origin.x + vector2Add.x;
            sum.y = origin.y + vector2Add.y;

            return sum;
        }

        /// <summary>
        /// Subtract a vector with a vector
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="vector2Subtract"></param>
        /// <returns></returns>
        public static Vector2 Subtract(this Vector2 origin, Vector2 vector2Subtract)
        {
            Vector2 sub = Vector2.zero;

            sub.x = origin.x - vector2Subtract.x;
            sub.y = origin.y - vector2Subtract.y;

            return sub;
        }

        /// <summary>
        /// Add a vector with a vector
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="vector3Add"></param>
        /// <returns></returns>
        public static Vector3 Add(this Vector3 origin, Vector3 vector3Add)
        {
            Vector3 sum = Vector3.zero;

            sum.x = origin.x + vector3Add.x;
            sum.y = origin.y + vector3Add.y;
            sum.z = origin.z + vector3Add.z;

            return sum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 Add(this Vector3 origin, float x, float y, float z)
        {
            Vector3 sum = Vector3.zero;

            sum.x = origin.x + x;
            sum.y = origin.y + y;
            sum.z = origin.z + z;

            return sum;
        }

        /// <summary>
        /// Subtract a vector with a vector
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="vector3Subtract"></param>
        /// <returns></returns>
        public static Vector3 Subtract(this Vector3 origin, Vector3 vector3Subtract)
        {
            Vector3 sub = Vector3.zero;

            sub.x = origin.x - vector3Subtract.x;
            sub.y = origin.y - vector3Subtract.y;
            sub.z = origin.z - vector3Subtract.z;

            return sub;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="maxDistanceDelta"></param>
        /// <returns></returns>
        public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            if (current.y != 0f)
            {
                current.y = Mathf.Lerp(current.y, 0f, maxDistanceDelta * 12f);
            }

            if (current.z != 0f)
            {
                current.z = Mathf.Lerp(current.z, 0f, maxDistanceDelta * 12f);
            }

            return Vector3.MoveTowards(current, target, maxDistanceDelta);
        }
        
        /// <summary>
        /// Extension equals Vector3 in each dimensions.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="other"></param>
        /// <param name="checkX"></param>
        /// <param name="checkY"></param>
        /// <param name="checkZ"></param>
        /// <returns></returns>
        public static bool EqualsExtension(this Vector3 origin, Vector3 other, bool checkX, bool checkY, bool checkZ)
        {
            if (!checkX && !checkY && !checkZ) return false;

            var boolX = checkX ? (origin.x == other.x) : true;
            var boolY = checkY ? (origin.y == other.y) : true;
            var boolZ = checkZ ? (origin.z == other.z) : true;

            return boolX && boolY && boolZ;
        }

        /// <summary>
        /// Return a random position on a ring, which random distance to center from  minRange to maxRange
        /// </summary>
        /// <param name="minRange"> the radius of small circle</param>
        /// <param name="maxRange">The radius of big circle</param>
        /// <returns></returns>
        public static Vector3 RandomOnRing(float minRange, float maxRange)
        {
            return  (Vector3) Random.insideUnitCircle.normalized * Random.Range(minRange, maxRange);
        }
        
        /// <summary>
        /// Clamp this vector 3 so it doesn't lower than value ~ always out of sphere
        /// </summary>
        /// <returns></returns>
        public static Vector3 ClampOutsize(this Vector3 v, float value, bool includeZ)
        {
            if (value <= 0) return v;

            float ClampFloat(float a, float b)
            {
                if (a < 0 && a > -b)
                {
                    a = -b;
                }
            
                if (a >= 0 && a < b)
                {
                    a = b;
                }

                return a;
            }

            v.x = ClampFloat(v.x, value);
            v.y = ClampFloat(v.y, value);
            
            if (includeZ) v.z = ClampFloat(v.z, value);

            return v;
        }
    }
}