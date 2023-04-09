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
        public NativeArray<float2> LastPositions;
        public NativeArray<float2> Positions;

        public void Execute(int i) {
            const float margin = 2.0f;
            float maxX = Width - margin;
            float maxY = Height - margin;
            var obj=new PhysicsObject(LastPositions[i],Gravity) ;
            // Apply Verlet integration
            var pos = obj.Update(Positions[i], DeltaTime);
            // Apply map borders collisions
            if (pos.x > maxX) {
                pos.x = maxX;
            }
            else if (pos.x < margin) {
                pos.x = margin;
            }
            if (pos.y > maxY) {
                pos.y = maxY;
            }
            else if (pos.y < margin) {
                pos.y = margin;
            }

            LastPositions[i] = obj.LastPosition;
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
        public NativeArray<float2> LastPositions;
        public NativeArray<float2> Positions;
        public void Execute(int i) {
            const float margin = 2.0f;
            float maxX=Width-margin;
            float maxY=Height-margin ;
            
            var pos = Positions[i];
            var obj =  new PhysicsObject(LastPositions[i],Star.GetGravity(10000f,pos)+Gravity);
            // Apply Verlet integration
            pos=obj.Update(pos,DeltaTime);
            // Apply map borders collisions
            if (pos.x > maxX ) {
                pos.x = maxX;
            } else if (pos.x < margin) {
                pos.x = margin;
            }
            if (pos.y > maxY ) {
                pos.y = maxY;
                
            } else if (pos.y < margin) {
                pos.y = margin;
            }
            LastPositions[i] = obj.LastPosition;
            Positions[i] = pos;
        }
    }
}