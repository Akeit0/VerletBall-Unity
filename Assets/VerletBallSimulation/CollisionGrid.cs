using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace VerletBallSimulation {
    public unsafe  struct CollisionGrid :IDisposable{
        [NativeDisableUnsafePtrRestriction]
        public int* ObjectCountArray;
        [NativeDisableUnsafePtrRestriction]
        public uint* ObjectIndexArray;
        
        public readonly int Width;
        public readonly int Height;
        public const int MaxObjectsPerCell = 4;
        
        public CollisionGrid(int width, int height) {
            Width = width;
            Height = height;
            ObjectCountArray = (int*) UnsafeUtility.Malloc(sizeof(int) * width * height, UnsafeUtility.AlignOf<int>(), Allocator.Persistent);
            ObjectIndexArray = (uint*) UnsafeUtility.Malloc(MaxObjectsPerCell*sizeof(uint) * width * height, UnsafeUtility.AlignOf<uint>(), Allocator.Persistent);
        }

        public void AddAtom(int x, int y, uint atom) {
            var id=  x * Height + y;
            var count = ObjectCountArray[id];
            if (count < MaxObjectsPerCell) {
                ObjectIndexArray[id * MaxObjectsPerCell + count] = atom;
                ObjectCountArray[id] = count + 1;
            }
        }

        public CollisionCell this[int x, int y] {
            get {
                var id=  x * Height + y;
                return new CollisionCell(ObjectIndexArray+ id * MaxObjectsPerCell, ObjectCountArray[id]);
            }
        }
        public CollisionCell this[int id] => new CollisionCell(ObjectIndexArray+ id * MaxObjectsPerCell, ObjectCountArray[id]);

        public void Clear() {
            UnsafeUtility.MemClear(ObjectCountArray,Width*Height*4);
            UnsafeUtility.MemClear(ObjectIndexArray,Width*Height*4*MaxObjectsPerCell);
        }
        public void Dispose() {
            UnsafeUtility.Free(ObjectCountArray, Allocator.Persistent);
            UnsafeUtility.Free(ObjectIndexArray, Allocator.Persistent);
        }
    }
    public unsafe struct CollisionCell {
        public uint* Indices;
        public int ObjectsCount;
        public CollisionCell(uint* indices, int indicesCount) {
            Indices = indices;
            ObjectsCount = indicesCount;
        }
    }
}