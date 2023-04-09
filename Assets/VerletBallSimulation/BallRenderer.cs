using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

namespace VerletBallSimulation {
    public class BallRenderer {
        Mesh _mesh;
        public GraphicsBuffer positionBuffer;
        public GraphicsBuffer colorsBuffer;
        public GraphicsBuffer argsBuffer;
        public uint[] args;
        static readonly int PositionBufferID = Shader.PropertyToID("_PositionBuffer");
        static readonly int ColorBufferID = Shader.PropertyToID("_ColorBuffer");

        public BallRenderer(int capacity) {
            _mesh= QuadMaker.Quad();
            args = new uint[] { 6, 0, 0, 0, 0 };
            argsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments,1, args.Length * sizeof(uint));
            positionBuffer=new GraphicsBuffer(GraphicsBuffer.Target.Structured, capacity, UnsafeUtility.SizeOf<Vector2>());
            colorsBuffer=new GraphicsBuffer(GraphicsBuffer.Target.Structured, capacity, UnsafeUtility.SizeOf<Color32>());
           
        }

        public  void Update(Material material,PhysicsSolver solver) {
            var count=solver.ObjectCount;
            positionBuffer.SetData(solver.PositionArray.GetSubArray(0,count));
            material.SetBuffer(PositionBufferID,positionBuffer);
            colorsBuffer.SetData(solver.ColorArray.GetSubArray(0,count));
           material.SetBuffer(ColorBufferID,colorsBuffer);
           args[1] = (uint) count;
            argsBuffer.SetData(args);
          
        }

        public void Draw(Material material) {
            Graphics.DrawMeshInstancedIndirect(_mesh, 0, material,
            new Bounds(default,new Vector3(float.MaxValue,float.MaxValue,float.MaxValue)),
            argsBuffer, 0, null,
            ShadowCastingMode.Off, false, 0, null, LightProbeUsage.Off);
        }
        public void DestroyBuffers() {
            if(positionBuffer != null)
                positionBuffer.Release();
            positionBuffer = null;
            if(colorsBuffer != null)
                colorsBuffer.Release();
            colorsBuffer = null;
            if(argsBuffer != null)
                argsBuffer.Release();
            argsBuffer = null;
        }
        public void Dispose() {
            DestroyBuffers();
        }
    }
}