using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace VerletBallSimulation {
    [BurstCompile(FloatPrecision.Medium, FloatMode.Fast, CompileSynchronously = true)]
    public unsafe struct SolveCollisionJob :IJobParallelFor{
        [NativeDisableUnsafePtrRestriction]
        public  float2* Positions;
        public CollisionGrid Grid;
        public int Offset;
        public SolveCollisionJob(float2* positions, CollisionGrid grid,int offset) {
            Positions = positions;
            Grid = grid;
            Offset = offset;
        }
        public void Execute(int index) {
            var startX= 6*index+3*Offset;
            var height= Grid.Height;
            var maxX=math.min(startX+3,Grid.Width);
            for (int x = startX; x < maxX; x++) {
                for (int y = 0; y < height; y++) {
                    var id = x * height + y;
                    var currentCell= Grid[id];
                    for (int i = 0; i < currentCell.ObjectsCount; i++) {
                        ref  var  pos = ref Positions[currentCell.Indices[i]];
                        SolveContactsInCell(ref pos,i,currentCell);
                        if (x != 0) {
                            SolveContacts(ref pos,id-height);
                            if(y!=0) SolveContacts(ref pos, id-height-1);
                            if(y!=height-1) SolveContacts(ref pos, id-height + 1);
                        }
                        if(x!=Grid.Width-1) {
                            SolveContacts(ref pos, id+height);
                            if(y!=0) SolveContacts(ref pos, id+height - 1);
                            if(y!=height-1) SolveContacts(ref pos, id+height + 1);
                        }
                        if (y != 0) SolveContacts(ref pos, Grid[id- 1]);
                        if(y!=height-1) SolveContacts(ref pos, Grid[id+ 1]);
                    }
                }
            }
        } 
        
        void SolveContacts(ref float2 pos,  CollisionCell c)
        {
            for (int i = 0; i < c.ObjectsCount; i++) {
                ref  var  pos2 = ref Positions[c.Indices[i]];
                var distV = pos - pos2;
                float dist2 = math.lengthsq(distV );
                if (dist2 is < 1f and > 0.0001f) {
                    var  dist   = math.sqrt(dist2);
                    var  delta  =  0.5f * (1.0f - dist);
                    var  colVec = distV* ( delta/dist);
                    pos += colVec;
                    pos2 -= colVec;
                }
            }
        }
        
        void SolveContacts(ref float2 pos,  int index)
        {
            var indices=Grid.ObjectIndexArray + index * CollisionGrid.MaxObjectsPerCell;
            for (int i = 0; i < Grid.ObjectCountArray[index]; i++) {
                ref  var  pos2 = ref Positions[indices[i]];
                var distV = pos - pos2;
                float dist2 = math.lengthsq(distV );
                if (dist2 is < 1f and > 0.0001f) {
                    var  dist   = math.sqrt(dist2);
                    var  delta  =  0.5f * (1.0f - dist);
                    var  colVec = distV* ( delta/dist);
                    pos += colVec;
                    pos2 -= colVec;
                }
            }
        }
        void SolveContactsInCell(ref float2 pos,  int indexInCell,CollisionCell c)
        {
            for (int i = 0; i < c.ObjectsCount; i++) {
                if(indexInCell==i) continue;
                ref  var  pos2 = ref Positions[c.Indices[i]];
                var distV = pos - pos2;
                var  dist2 = math.lengthsq(distV );
                if (dist2 is >= 1f or <= 0.0001f) continue;
                var  dist   = math.sqrt(dist2);
                var  delta  =  0.5f * (1.0f - dist);
                var  colVec = distV* ( delta/dist);
                pos += colVec;
                pos2 -= colVec;
            }
        }
    }
}