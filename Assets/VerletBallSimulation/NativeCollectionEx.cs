using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System;
namespace VerletBallSimulation {
    public static unsafe class NativeCollectionEx {
        public static void Clear<T>(this NativeArray<T> array) where T : unmanaged {
            UnsafeUtility.MemClear(array.GetUnsafePtr(), array.Length * UnsafeUtility.SizeOf<T>());
        }

        public static Span<T> AsSpan<T>(this NativeArray<T> array) where T : unmanaged {
            var ptr = array.GetUnsafePtr();
            return new Span<T>(ptr, array.Length);
        }
        public static NativeArray<T> PtrToNativeArray<T>(T* ptr, int length)
            where T : unmanaged
        {
            var arr = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(ptr, length, Allocator.Invalid);

            // これをやらないとNativeArrayのインデクサアクセス時に死ぬ
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref arr, AtomicSafetyHandle.GetTempMemoryHandle());
#endif

            return arr;
        }
    }
}