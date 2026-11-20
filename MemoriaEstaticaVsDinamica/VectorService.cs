namespace MemoriaEstaticaVsDinamica;

public sealed class VectorState
{
    public int StaticLength { get; init; }
    public int DynamicCount { get; init; }
    public int DynamicCapacity { get; init; }
    public int[] StaticPreview { get; init; } = [];
    public int[] DynamicPreview { get; init; } = [];
}

public sealed class VectorService
{
    private readonly Random _random = new();
    private int[] _staticArray = [];
    private List<int> _dynamicList = [];

    public int StaticLength => _staticArray.Length;
    public int DynamicCount => _dynamicList.Count;
    public int DynamicCapacity => _dynamicList.Capacity;

    public void Generate(int size)
    {
        if (size < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "El tamano no puede ser negativo.");
        }

        _staticArray = new int[size];
        _dynamicList = new List<int>(size);

        for (int i = 0; i < size; i++)
        {
            int value = _random.Next(1, 1000);
            _staticArray[i] = value;
            _dynamicList.Add(value);
        }
    }

    public void ResizeStatic(int newSize)
    {
        if (newSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newSize), "El nuevo tamano no puede ser negativo.");
        }

        int oldSize = _staticArray.Length;
        Array.Resize(ref _staticArray, newSize);

        for (int i = oldSize; i < newSize; i++)
        {
            _staticArray[i] = _random.Next(1, 1000);
        }
    }

    public void ResizeDynamic(int newSize)
    {
        if (newSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newSize), "El nuevo tamano no puede ser negativo.");
        }

        if (newSize > _dynamicList.Count)
        {
            int toAdd = newSize - _dynamicList.Count;
            for (int i = 0; i < toAdd; i++)
            {
                _dynamicList.Add(_random.Next(1, 1000));
            }
        }
        else if (newSize < _dynamicList.Count)
        {
            _dynamicList.RemoveRange(newSize, _dynamicList.Count - newSize);
        }
    }

    public void Clear()
    {
        _staticArray = [];
        _dynamicList.Clear();
        _dynamicList.TrimExcess();
    }

    public VectorState GetState(int maxPreviewItems = 50)
    {
        int staticPreviewCount = Math.Min(_staticArray.Length, maxPreviewItems);
        int dynamicPreviewCount = Math.Min(_dynamicList.Count, maxPreviewItems);

        int[] staticPreview = new int[staticPreviewCount];
        Array.Copy(_staticArray, staticPreview, staticPreviewCount);

        int[] dynamicPreview = new int[dynamicPreviewCount];
        _dynamicList.CopyTo(0, dynamicPreview, 0, dynamicPreviewCount);

        return new VectorState
        {
            StaticLength = _staticArray.Length,
            DynamicCount = _dynamicList.Count,
            DynamicCapacity = _dynamicList.Capacity,
            StaticPreview = staticPreview,
            DynamicPreview = dynamicPreview
        };
    }
}
