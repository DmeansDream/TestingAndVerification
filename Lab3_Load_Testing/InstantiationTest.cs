using System.Collections.Generic;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.Pool;

namespace Load
{
    public class InstantiationTest
    {
        private const int MeasurementCount = 10;
        
        private ObjectPool<GameObject> _pool;
        public GameObject BasicPrefab   { get; private set; }
        public const string BasicPath = "Performance/BasicPrefab";

        [SetUp]
        public virtual void SetUp()
        {
            BasicPrefab = Resources.Load<GameObject>(BasicPath);
            
            Assert.IsNotNull(BasicPrefab);
        }

        [TearDown]
        public void TearDown()
        {
            _pool?.Dispose();
            _pool = null;
        }
        
        public ObjectPool<GameObject> CreatePool(GameObject prefab, int capacity, bool collectionCheck = false)
        {
            return new ObjectPool<GameObject>(
                createFunc: () => Object.Instantiate(prefab),
                actionOnGet: obj=> obj.SetActive(true),
                actionOnRelease: obj=> obj.SetActive(false),
                actionOnDestroy: obj=> Object.DestroyImmediate(obj),
                collectionCheck: collectionCheck,
                defaultCapacity: capacity,
                maxSize: capacity
            );
        }
    
        public void PrewarmPool(ObjectPool<GameObject> pool, int count)
        {
            var buffer = new GameObject[count];

            for (int i = 0; i < count; i++)
                buffer[i] = pool.Get();

            for (int i = 0; i < count; i++)
                pool.Release(buffer[i]);
        }
    
        public static IEnumerable<TestCaseData> InstantiationCases() 
        {
            yield return new TestCaseData(100).SetName("Light (N=100)");
            yield return new TestCaseData(1000).SetName("Heavy (N=1000)");
        }

        private void PerformRawInstantiation(GameObject[] instances, int count)
        {
            for (int i = 0; i < count; i++)
                instances[i] = Object.Instantiate(BasicPrefab);
        }

        private void PerformRawDestruction(GameObject[] instances, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (instances[i] == null) continue;
                Object.DestroyImmediate(instances[i]);
                instances[i] = null;
            }
        }

        private void PerformPoolAcquire(GameObject[] acquired, int count)
        {
            for (int i = 0; i < count; i++)
                acquired[i] = _pool.Get();
        }

        private void PerformPoolRelease(GameObject[] acquired, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (acquired[i] == null) continue;
                _pool.Release(acquired[i]);
                acquired[i] = null;
            }
        }

        [Test, Performance]
        [TestCaseSource(nameof(InstantiationCases))]
        public void RawInstantiation_BurstCreate(int objectCount)
        {
            var instances = new GameObject[objectCount];

            Measure.Method(() => PerformRawInstantiation(instances, objectCount))
                .WarmupCount(0)
                .MeasurementCount(MeasurementCount)
                .CleanUp(() => PerformRawDestruction(instances, objectCount))
                .GC()
                .Run();
        }

        [Test, Performance]
        [TestCaseSource(nameof(InstantiationCases))]
        public void Pool_AcquireRelease(int objectCount)
        {
            bool isLight = objectCount == 100;
            _pool = CreatePool(BasicPrefab, objectCount, collectionCheck: isLight);
            PrewarmPool(_pool, objectCount);

            var acquired = new GameObject[objectCount];

            Measure.Method(() => PerformPoolAcquire(acquired, objectCount))
                .WarmupCount(0)
                .MeasurementCount(MeasurementCount)
                .CleanUp(() => PerformPoolRelease(acquired, objectCount))
                .GC()
                .Run();
            
            Measure.Method(() => PerformPoolRelease(acquired, objectCount))
                .WarmupCount(0)
                .MeasurementCount(MeasurementCount)
                .SetUp(() => PerformPoolAcquire(acquired, objectCount))
                .GC()
                .Run();
        }

        [Test, Performance]
        [TestCaseSource(nameof(InstantiationCases))]
        public void HeadToHead_RawInstantiate_vs_Pool(int objectCount)
        {
            var instances = new GameObject[objectCount];
            var acquired  = new GameObject[objectCount];
            
            Measure.Method(() =>
                {
                    PerformRawInstantiation(instances, objectCount);
                    PerformRawDestruction(instances, objectCount);
                })
                .WarmupCount(0)
                .MeasurementCount(MeasurementCount)
                .SampleGroup(new SampleGroup("HeadToHead_Raw", SampleUnit.Millisecond))
                .GC()
                .Run();
            
            bool isLight = objectCount == 100;
            _pool = CreatePool(BasicPrefab, objectCount, collectionCheck: isLight);
            //PrewarmPool(_pool, objectCount);

            Measure.Method(() =>
                {
                    PerformPoolAcquire(acquired, objectCount);
                    PerformPoolRelease(acquired, objectCount);
                })
                .WarmupCount(0)
                .MeasurementCount(MeasurementCount)
                .SampleGroup(new SampleGroup("HeadToHead_Pool", SampleUnit.Millisecond))
                .GC()
                .Run();
        }
    }
}

