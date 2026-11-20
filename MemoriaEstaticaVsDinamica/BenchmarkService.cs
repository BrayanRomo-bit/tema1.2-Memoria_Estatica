using System.Diagnostics;

namespace MemoriaEstaticaVsDinamica;

public sealed class BenchmarkResult
{
    public string StructureName { get; init; } = string.Empty;
    public int OperationsCount { get; init; }
    public int FinalSize { get; init; }
    public int FinalCapacity { get; init; }
    public double ElapsedMilliseconds { get; init; }
    public long AllocatedBytes { get; init; }
    public int Gen0Collections { get; init; }
    public int Reallocations { get; init; }
}

public sealed class BenchmarkService
{
    public IReadOnlyList<BenchmarkResult> RunGrowthComparison(int totalElements, int staticResizeStep = 1)
    {
        if (totalElements <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalElements), "La cantidad de elementos debe ser mayor a cero.");
        }

        if (staticResizeStep <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(staticResizeStep), "El paso de redimensionamiento debe ser mayor a cero.");
        }

        BenchmarkResult staticResult = MeasureStaticGrowth(totalElements, staticResizeStep);
        BenchmarkResult dynamicResult = MeasureDynamicGrowth(totalElements);

        return [staticResult, dynamicResult];
    }

    private static BenchmarkResult MeasureStaticGrowth(int totalElements, int resizeStep)
    {
        PrepareGc();

        int gen0Before = GC.CollectionCount(0);
        long bytesBefore = GC.GetAllocatedBytesForCurrentThread();
        Stopwatch sw = Stopwatch.StartNew();

        int[] array = [];
        int reallocations = 0;

        for (int i = 0; i < totalElements; i++)
        {
            if (i >= array.Length)
            {
                int nextSize = Math.Min(array.Length + resizeStep, totalElements);
                Array.Resize(ref array, nextSize);
                reallocations++;
            }

            array[i] = i;
        }

        sw.Stop();
        long bytesAfter = GC.GetAllocatedBytesForCurrentThread();
        int gen0After = GC.CollectionCount(0);

        return new BenchmarkResult
        {
            StructureName = $"Arreglo estatico (int[], paso={resizeStep})",
            OperationsCount = totalElements,
            FinalSize = array.Length,
            FinalCapacity = array.Length,
            ElapsedMilliseconds = sw.Elapsed.TotalMilliseconds,
            AllocatedBytes = Math.Max(0, bytesAfter - bytesBefore),
            Gen0Collections = Math.Max(0, gen0After - gen0Before),
            Reallocations = reallocations
        };
    }

    private static BenchmarkResult MeasureDynamicGrowth(int totalElements)
    {
        PrepareGc();

        int gen0Before = GC.CollectionCount(0);
        long bytesBefore = GC.GetAllocatedBytesForCurrentThread();
        Stopwatch sw = Stopwatch.StartNew();

        List<int> list = [];
        int previousCapacity = list.Capacity;
        int reallocations = 0;

        for (int i = 0; i < totalElements; i++)
        {
            list.Add(i);
            if (list.Capacity != previousCapacity)
            {
                reallocations++;
                previousCapacity = list.Capacity;
            }
        }

        sw.Stop();
        long bytesAfter = GC.GetAllocatedBytesForCurrentThread();
        int gen0After = GC.CollectionCount(0);

        return new BenchmarkResult
        {
            StructureName = "Lista dinamica (List<int>)",
            OperationsCount = totalElements,
            FinalSize = list.Count,
            FinalCapacity = list.Capacity,
            ElapsedMilliseconds = sw.Elapsed.TotalMilliseconds,
            AllocatedBytes = Math.Max(0, bytesAfter - bytesBefore),
            Gen0Collections = Math.Max(0, gen0After - gen0Before),
            Reallocations = reallocations
        };
    }

    private static void PrepareGc()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}
