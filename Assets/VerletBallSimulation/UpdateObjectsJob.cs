using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace VerletBallSimulation {
    [BurstCompile]
    public struct UpdateObjectsJob : IJobParallelFor {
        public float DeltaTime;
        public float Width;
        public float Height;
        public float2 Gravity;
        public NativeArray<PhysicsObject> Objects;
        public NativeArray<float2> Positions;

        public void Execute(int i) {
            const float margin = 2.0f;
            float maxX = Width - margin;
            float maxY = Height - margin;
            var obj = Objects[i];
            obj.Acceleration += Gravity;
            // Apply Verlet integration
            var pos = obj.Update(Positions[i], DeltaTime);
            // Apply map borders collisions
            if (pos.x > maxX) {
                //obj.LastPosition.x = 2*maxX - obj.LastPosition.x;
                pos.x = maxX;
            }
            else if (pos.x < margin) {
                //obj.LastPosition.x = 2 * margin - obj.LastPosition.x;
                pos.x = margin;

            }

            if (pos.y > maxY) {
                //obj.LastPosition.y = 2 * maxY - obj.LastPosition.y;
                pos.y = maxY;

            }
            else if (pos.y < margin) {
                // obj.LastPosition.y = 2 * margin - obj.LastPosition.y;
                pos.y = margin;
            }

            Objects[i] = obj;
            Positions[i] = pos;
        }
    }

    [BurstCompile]
    public struct UpdateObjectsWithDarkStarJob :IJobParallelFor {
        public float DeltaTime;
        public float Width;
        public float Height;
        public DarkStar Star;
        public float2 Gravity;
        public NativeArray<PhysicsObject> Objects;
        public NativeArray<float2> Positions;
        public void Execute(int i) {
            const float margin = 2.0f;
            float maxX=Width-margin;
            float maxY=Height-margin ;
             var obj =  Objects[i];
             var pos = Positions[i];
             obj.Acceleration += Star.GetGravity(10000f,pos)+Gravity;
            // Apply Verlet integration
            pos=obj.Update(Positions[i],DeltaTime);
            // Apply map borders collisions
            if (pos.x > maxX ) {
                //obj.LastPosition.x = 2*maxX - obj.LastPosition.x;
                pos.x = maxX;
            } else if (pos.x < margin) {
                //obj.LastPosition.x = 2 * margin - obj.LastPosition.x;
                pos.x = margin;
             
            }
            if (pos.y > maxY ) {
                //obj.LastPosition.y = 2 * maxY - obj.LastPosition.y;
                pos.y = maxY;
                
            } else if (pos.y < margin) {
                // obj.LastPosition.y = 2 * margin - obj.LastPosition.y;
                pos.y = margin;
            }
            Objects[i] = obj;
            Positions[i] = pos;
        }
    }
}