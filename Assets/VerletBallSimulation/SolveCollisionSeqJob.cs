// using Unity.Burst;
// using Unity.Collections;
// using Unity.Collections.LowLevel.Unsafe;
// using Unity.Jobs;
// using Unity.Mathematics;
// namespace VerletBallSimulation {
//     [BurstCompile]
//     public unsafe struct SolveCollisionSeqYJob:IJobParallelFor{
//         [NativeDisableUnsafePtrRestriction]
//         public  float2* Positions;
//         public CollisionGrid Grid;
//         public int Offset;
//         public SolveCollisionSeqYJob(float2* positions, CollisionGrid grid,int offset) {
//             Positions = positions;
//             Grid = grid;
//             Offset = offset;
//         }
//         public void Execute(int index) {
//             var x= 2*index+Offset;
//             CollisionCell lastCell = default;
//             CollisionCell currentCell = Grid[x, 0];
//             for (int y = 0; y < Grid.Height; y++) {
//                 var nextCell = (y != Grid.Height - 1) ? Grid[x, y + 1] : default;
//                 for (int i = 0; i < currentCell.ObjectsCount; i++) {
//                     ref  var  pos = ref Positions[currentCell.Indices[i]];
//                     SolveContactsInCell(ref pos,i,currentCell);
//                     SolveContacts(ref pos, lastCell);
//                     SolveContacts(ref pos, nextCell);
//                 }
//                 lastCell= currentCell;
//                 currentCell= nextCell;
//             }
//         }
//         void SolveContacts(ref float2 pos,  CollisionCell c)
//         {
//             for (int i = 0; i < c.ObjectsCount; i++) {
//                 ref  var  pos2 = ref Positions[c.Indices[i]];
//                 var distV = pos - pos2;
//                 float dist2 = math.lengthsq(distV );
//                 if (dist2 is < 1f and > 0.0001f) {
//                     var  dist   = math.sqrt(dist2);
//                     var  delta  =  0.5f * (1.0f - dist);
//                     var  colVec = distV* ( delta/dist);
//                     pos += colVec;
//                     pos2 -= colVec;
//                 }
//             }
//         }
//         void SolveContactsInCell(ref float2 pos,  int indexInCell,CollisionCell c)
//         {
//             for (int i = 0; i < c.ObjectsCount; i++) {
//                 if(indexInCell==i) continue;
//                 ref  var  pos2 = ref Positions[c.Indices[i]];
//                 var distV = pos - pos2;
//                 var  dist2 = math.lengthsq(distV );
//                 if (dist2 is >= 1f or <= 0.0001f) continue;
//                 var  dist   = math.sqrt(dist2);
//                 var  delta  =  0.5f * (1.0f - dist);
//                 var  colVec = distV* ( delta/dist);
//                 pos += colVec;
//                 pos2 -= colVec;
//             }
//         }
//
//        
//     }
//      [BurstCompile]
//         public unsafe struct SolveCollisionSeqXJob : IJobParallelFor {
//             [NativeDisableUnsafePtrRestriction] public float2* Positions;
//             public CollisionGrid Grid;
//             public int Offset;
//
//             public SolveCollisionSeqXJob(float2* positions, CollisionGrid grid, int offset) {
//                 Positions = positions;
//                 Grid = grid;
//                 Offset = offset;
//             }
//
//             public void Execute(int index) {
//                 var y = 2 * index + Offset;
//                 CollisionCell lastCell = default;
//                 CollisionCell currentCell = Grid[0, y];
//                 for (int x = 0; x < Grid.Width; x++) {
//                     var nextCell = (x != Grid.Width - 1) ? Grid[x + 1, y] : default;
//                     for (int i = 0; i < currentCell.ObjectsCount; i++) {
//                         ref var pos = ref Positions[currentCell.Indices[i]];
//                         SolveContactsInCell(ref pos, i, currentCell);
//                         SolveContacts(ref pos, lastCell);
//                         SolveContacts(ref pos, nextCell);
//                     }
//
//                     lastCell = currentCell;
//                     currentCell = nextCell;
//                 }
//             }
//
//             void SolveContacts(ref float2 pos, CollisionCell c) {
//                 for (int i = 0; i < c.ObjectsCount; i++) {
//                     ref var pos2 = ref Positions[c.Indices[i]];
//                     var distV = pos - pos2;
//                     float dist2 = math.lengthsq(distV);
//                     if (dist2 is < 1f and > 0.0001f) {
//                         var dist = math.sqrt(dist2);
//                         var delta = 0.5f * (1.0f - dist);
//                         var colVec = distV * (delta / dist);
//                         pos += colVec;
//                         pos2 -= colVec;
//                     }
//                 }
//             }
//
//             void SolveContactsInCell(ref float2 pos, int indexInCell, CollisionCell c) {
//                 for (int i = 0; i < c.ObjectsCount; i++) {
//                     if (indexInCell == i) continue;
//                     ref var pos2 = ref Positions[c.Indices[i]];
//                     var distV = pos - pos2;
//                     var dist2 = math.lengthsq(distV);
//                     if (dist2 is >= 1f or <= 0.0001f) continue;
//                     var dist = math.sqrt(dist2);
//                     var delta = 0.5f * (1.0f - dist);
//                     var colVec = distV * (delta / dist);
//                     pos += colVec;
//                     pos2 -= colVec;
//                 }
//             }
//         }
// }