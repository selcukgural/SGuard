using System.Buffers;

namespace SGuard.Tests;

/// <summary>
/// <c>EmptyMemory</c> reads any <see cref="Memory{T}"/> or <see cref="ReadOnlyMemory{T}"/> as a
/// <c>ReadOnlyMemory&lt;byte&gt;</c>, and a boxed one through its <c>ToString</c>. These tests fail if a runtime changes
/// either.
/// </summary>
public sealed class MemoryTests
{
    private readonly record struct Large(long A, long B, long C, string D);

    private sealed class OwnedMemory(int length) : MemoryManager<int>
    {
        private readonly int[] _items = new int[length];

        public override Span<int> GetSpan() => _items;
        public override MemoryHandle Pin(int elementIndex = 0) => default;
        public override void Unpin() { }
        protected override void Dispose(bool disposing) { }
    }

    public static TheoryData<string, object, bool> Values() => new()
    {
        { "int array", new int[3].AsMemory(), false },
        { "int array slice of 0", new int[3].AsMemory(2, 0), true },
        { "int array slice at the end", new int[3].AsMemory(3), true },
        { "default int", default(Memory<int>), true },
        { "string", "abc".AsMemory(), false },
        { "string slice of 0", "abc".AsMemory(1, 0), true },
        { "string slice of 1", "abc".AsMemory(2, 1), false },
        { "char array", new[] { 'x' }.AsMemory(), false },
        { "empty char array", Array.Empty<char>().AsMemory(), true },
        { "reference type", new[] { "a", "b" }.AsMemory(1), false },
        { "large struct", new Large[2].AsMemory(), false },
        { "large struct slice of 0", new Large[2].AsMemory(1, 0), true },
        { "memory manager", new OwnedMemory(4).Memory, false },
        { "memory manager slice of 0", new OwnedMemory(4).Memory.Slice(4), true },
        { "read-only int", (ReadOnlyMemory<int>)new int[1].AsMemory(), false },
        { "read-only empty", ReadOnlyMemory<byte>.Empty, true },
        { "read-only char with brackets", "[0]".AsMemory(), false },
        { "read-only char empty", "x[0]".AsMemory(4), true },
        { "length ending in 0", new int[10].AsMemory(), false },
    };

    [Theory]
    [MemberData(nameof(Values))]
    public void BoxedMemory_IsEmpty_OnlyWithoutElements(string name, object memory, bool empty)
    {
        Assert.True(empty == Is.NullOrEmpty(memory), name);
    }

    [Fact]
    public void TypedMemory_IsEmpty_OnlyWithoutElements()
    {
        Assert.False(Is.NullOrEmpty(new int[3].AsMemory()));
        Assert.True(Is.NullOrEmpty(new int[3].AsMemory(3)));
        Assert.False(Is.NullOrEmpty(new Large[2].AsMemory(1)));
        Assert.True(Is.NullOrEmpty(new Large[2].AsMemory(2)));
        Assert.False(Is.NullOrEmpty((ReadOnlyMemory<char>)"abc".AsMemory(1)));
        Assert.True(Is.NullOrEmpty((ReadOnlyMemory<char>)"abc".AsMemory(3)));
        Assert.False(Is.NullOrEmpty(new OwnedMemory(2).Memory));
        Assert.True(Is.NullOrEmpty(new OwnedMemory(2).Memory[2..]));
        Assert.True(Is.NullOrEmpty(default(ReadOnlyMemory<Large>)));
    }

    [Fact]
    public void NullableMemory_IsEmpty_OnlyWithoutElements()
    {
        Memory<int>? empty = new int[1].AsMemory(1);
        Memory<int>? notEmpty = new int[1].AsMemory();

        Assert.True(Is.NullOrEmpty(empty));
        Assert.False(Is.NullOrEmpty(notEmpty));
    }
}
