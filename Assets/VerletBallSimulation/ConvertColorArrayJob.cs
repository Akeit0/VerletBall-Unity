using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace VerletBallSimulation {
    [BurstCompile]
    public unsafe struct ConvertColorArrayJob :IJobParallelFor {
        [NativeDisableUnsafePtrRestriction]
        public Color32* TextureData;
        [WriteOnly]
        public NativeArray<Color32> WriteData;
        [ReadOnly]
        public NativeArray<float2> PositionData;
        public int TextureWidth;
        public int TextureHeight;
        public int GridWidth;
        public int GridHeight;

        public void Execute(int index) {
            var pos= PositionData[index];
            int gridX = (int) (pos.x / GridWidth * TextureWidth);
            int gridY = (int) (pos.y / GridHeight * TextureHeight);
            int gridIndex = gridX + gridY * TextureWidth;
            WriteData[index] = TextureData[gridIndex];
        }
    }
}