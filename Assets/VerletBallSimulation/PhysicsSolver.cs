using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

namespace VerletBallSimulation {
    public unsafe class PhysicsSolver :IDisposable {
        public float2* PositionPtr;
        public NativeArray<float2> LastPositionArray;
        public NativeArray<float2> PositionArray;
        public NativeArray<Color32> ColorArray;
        public CollisionGrid Grid;
        public int ObjectCount;
        public float2 Gravity;
        public readonly int Capacity;
        public PhysicsSolver(int width, int height,int capacity) {
            Capacity =capacity;
            Grid = new CollisionGrid(width, height);
            LastPositionArray= new NativeArray<float2>(Capacity, Allocator.Persistent);
            PositionArray= new NativeArray<float2>(Capacity, Allocator.Persistent);
            PositionPtr = (float2*)PositionArray.GetUnsafePtr();
            ColorArray=new NativeArray<Color32>(Capacity, Allocator.Persistent);

        }
        public JobHandle SolveCollisions(JobHandle jobHandle) {

          jobHandle = new SolveCollisionJob(PositionPtr, Grid, 0).Schedule((Grid.Width+5)/6,0,jobHandle);
          return    new SolveCollisionJob(PositionPtr, Grid, 1).Schedule((Grid.Width+2)/6,0,jobHandle);
           // jobHandle = new SolveCollisionJob(PositionPtr, Grid, 0).Schedule((Grid.Width+2)/3,0,jobHandle);
           // jobHandle=   new SolveCollisionJob(PositionPtr, Grid, 1).Schedule((Grid.Width+1)/3,0,jobHandle);
           // return   new SolveCollisionJob(PositionPtr, Grid, 2).Schedule(Grid.Width/3,0,jobHandle);
        }
        public bool AddObject(float2 position, float2 velocity,Color32 color) {
            if(ObjectCount>=Capacity) return false;
            LastPositionArray[ObjectCount] = position-velocity;
            ColorArray[ObjectCount] = color;
            PositionArray[ObjectCount] = position;
            ObjectCount++;
            return true;
        }
        public bool AddObjectLastColor(float2 position, float2 velocity) {
            if(ObjectCount>=Capacity) return false;
            LastPositionArray[ObjectCount] = position-velocity;
            PositionArray[ObjectCount] = position;
            ObjectCount++;
            return true;
        }
        
        public void Update(int steps,float dt)
        {
            // Perform the sub steps
             float sub_dt = dt / steps;
             for (int i = 0; i < steps; i++) {
                 UpdateObjects(SolveCollisions(AddObjectsToGrid()),sub_dt);
             }
          
        }
        public void Update(float dt)
        {
            if(0<ObjectCount)
                UpdateObjects(SolveCollisions(AddObjectsToGrid()),dt);
            
        } public void Update(DarkStar darkStar,float dt)
        {
            if(0<ObjectCount)
                UpdateObjects(SolveCollisions(AddObjectsToGrid()),darkStar,dt);
            
        }

        JobHandle AddObjectsToGrid()
        {
            Grid.Clear();
            return new AddObjectsToGridJob() {
                Grid = Grid,
                Positions = PositionArray.GetSubArray(0,ObjectCount)
            }.Schedule();
            
        }
        public void UpdateObjects(JobHandle jobHandle,float dt) {
            
            new UpdateObjectsJob() {
                DeltaTime = dt,
                Gravity = Gravity,
                LastPositions =LastPositionArray,
                Positions = PositionArray,
                Width = Grid.Width,Height = Grid.Height
            }.Schedule(ObjectCount,64,jobHandle).Complete();
        }
        public void UpdateObjects(JobHandle jobHandle,DarkStar darkStar,float dt) {
            
            new UpdateObjectsWithDarkStarJob() {
                DeltaTime = dt,
                Gravity = Gravity,
                Star = darkStar,
                LastPositions =LastPositionArray,
                Positions = PositionArray,
                Width = Grid.Width,Height = Grid.Height
            }.Schedule(ObjectCount,64,jobHandle).Complete();
        }
        public void Dispose() {
            LastPositionArray.Dispose();
           // UnsafeUtility.Free(Objects, Allocator.Persistent);
            Grid.Dispose();
            PositionArray.Dispose();
            ColorArray.Dispose();
        }
    }
}