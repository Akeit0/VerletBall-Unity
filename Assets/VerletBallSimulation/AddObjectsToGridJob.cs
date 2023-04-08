using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace VerletBallSimulation {
    [BurstCompile]
    public struct AddObjectsToGridJob:IJob {
        [ReadOnly]
        public NativeArray<float2> Positions;
        public CollisionGrid Grid;
        public void Execute() {
            float maxX=Grid.Width-1f;
            float maxY=Grid.Height-1f;
            for (int i = 0; i < Positions.Length; i++) {
                var position =  Positions[i];
                
            if (position.x > 1.0f && position.x < maxX &&
                position.y > 1.0f && position.y < maxY) {
                Grid.AddAtom((int) (position.x),(int) (position.y),(uint) i);
            }
            }
            

        }
    }
}