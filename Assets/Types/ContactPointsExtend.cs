using UnityEngine;

namespace Assets.Types
{
    public static class ContactPointsExtend {
        public static ContactPoint Nearest(this ContactPoint[] points, Vector3 observerCoord)
        {
            if(points.Length == 0)
                throw new UnityException("Cant find nearest point. Contacts array length is 0");
            var nearest = points[0];
            if (points.Length == 1) return nearest;
            for (int i = 1; i < points.Length; i++)
            {
                if (Vector3.Distance(observerCoord, nearest.point) >=
                    Vector3.Distance(observerCoord, points[i].point))
                {
                    nearest = points[i]; 
                }
            }
            return nearest;
        }
    }
}
