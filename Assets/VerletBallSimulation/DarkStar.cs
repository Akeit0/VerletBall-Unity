using System;
using Unity.Mathematics;

namespace VerletBallSimulation {
    [Serializable]
    public  struct DarkStar {
        public  float2 Position;
        public  float Radius;
        public  float Mass;
        public DarkStar(float2 position,float radius, float mass) {
            Position = position;
            Radius = radius;
            Mass = mass;
        }
        public float2 GetGravity(float gravitationConstant,float2 position) {
            var delta= Position - position;
            var sqrDistance = math.lengthsq(delta);
            var sqrRadius = Radius * Radius;
            return sqrDistance < sqrRadius ? delta*(gravitationConstant*Mass / (sqrRadius * Radius)) : delta* (gravitationConstant*Mass / (sqrDistance*math.sqrt(sqrDistance)));
        }
    }
}