
using Unity.Mathematics;
using UnityEngine;

namespace VerletBallSimulation {
    public struct PhysicsObject {
        public float2 LastPosition;
        public float2 Acceleration;
        public PhysicsObject(float2 lastPosition,float2 acceleration) {
            LastPosition = lastPosition;
            Acceleration = acceleration;
        }
        const float MaxDelta=0.5f;
        public float2 Update(float2 position,float dt)
        {
            var  lastUpdateMove = position - LastPosition;
            var absX=math.abs(lastUpdateMove.x);
            if (MaxDelta < absX) {
                lastUpdateMove*= (MaxDelta/absX);
            }
            var absY=math.abs(lastUpdateMove.y);
            if (MaxDelta < absY) {
                lastUpdateMove*= (MaxDelta/absY);
            }
            var  newPosition = position + lastUpdateMove + (Acceleration - lastUpdateMove * 40.0f) * (dt * dt);
            LastPosition           = position;
            return                newPosition;
        }
    }
}
