using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace VerletBallSimulation {
    public unsafe class PointsMesh:IDisposable {
        public Mesh Mesh;
        NativeArray<uint> indices;
        public PointsMesh(int capacity) {
            
             indices = new NativeArray<uint>(capacity,Allocator.Persistent);
            var span = indices.AsSpan();
            for (int i = 0; i < span.Length; i++) {
                span[i] = (uint)i;
            }
            Mesh = new Mesh();
            Mesh.MarkDynamic();
        }
        static readonly VertexAttributeDescriptor[] _attributes = {
            new(VertexAttribute.Position, VertexAttributeFormat.Float32, 2),
            new(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4)
        };

        const MeshUpdateFlags MESH_UPDATE_FLAGS = MeshUpdateFlags.DontRecalculateBounds |
                                                  MeshUpdateFlags.DontValidateIndices |
                                                  MeshUpdateFlags.DontNotifyMeshUsers;

        public void Update(PhysicsSolver solver) {
            Mesh.Clear();
            var count = solver.ObjectCount;
             Mesh.SetVertexBufferParams(count,_attributes);
             using var vertices= new NativeArray<(float2,Color32)>(count, Allocator.Temp);
             var span = vertices.AsSpan();
             var posPtr=solver.PositionPtr;
             var oPtr=(Color32*)solver.ColorArray.GetUnsafePtr();
             for (int i = 0; i < span.Length; i++) {
                 span[i] = (posPtr[i],oPtr[i]);
             }
             Mesh.SetIndexBufferParams(count,IndexFormat.UInt32);
            var meshDesc = new SubMeshDescriptor(0, count, MeshTopology.Points) {
                firstVertex = 0,
                vertexCount = count
            };
            Mesh.SetSubMesh(0, meshDesc, MESH_UPDATE_FLAGS);
            
            Mesh.SetIndexBufferData(indices.GetSubArray(0, count), 0, 0, count, MESH_UPDATE_FLAGS);
            Mesh.SetVertexBufferData(vertices, 0, 0, count, 0, MESH_UPDATE_FLAGS);
        }

        public void Dispose() {
            indices.Dispose();
        }
    }
}