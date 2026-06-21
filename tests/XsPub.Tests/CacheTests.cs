using Xunit;
using XsPub.Library;

namespace XsPub.Tests;

public class CacheTests
{
    private static Cache<string> Make(string value) => new(() => value);

    // ── Self / null / type ────────────────────────────────────────────────

    [Fact]
    public void Equals_SameInstance_ReturnsTrue()
    {
        var c = Make("hello");
        Assert.True(c.Equals(c));
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        Assert.False(Make("hello").Equals((Cache<string>?)null));
    }

    [Fact]
    public void EqualsObject_Null_ReturnsFalse()
    {
        Assert.False(Make("hello").Equals((object?)null));
    }

    [Fact]
    public void EqualsObject_UnrelatedType_ReturnsFalse()
    {
        Assert.False(Make("hello").Equals("hello"));
    }

    [Fact]
    public void EqualsObject_RawStringMatchingValue_ReturnsFalse()
    {
        // Cache<string> and string are different types; the value inside
        // the cache should never be conflated with the cache itself.
        var cache = Make("hello");
        _ = cache.Value;
        Assert.False(cache.Equals((object)"hello"));
    }

    [Fact]
    public void EqualsObject_IncompatibleType_DoesNotForceValueCreation()
    {
        // Passing a non-Cache argument should short-circuit before ever
        // touching Value — the cast to Cache<T> returns null, which the
        // null-guard in Equals(Cache<T>?) catches without evaluating.
        var cache = Make("hello");
        Assert.False(cache.IsValueCreated);
        Assert.False(cache.Equals((object)"hello"));
        Assert.False(cache.IsValueCreated);
    }

    // ── Value already created ─────────────────────────────────────────────

    [Fact]
    public void Equals_BothValuesCreated_SameValue_ReturnsTrue()
    {
        var a = Make("hello");
        var b = Make("hello");
        _ = a.Value; _ = b.Value;
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_BothValuesCreated_DifferentValues_ReturnsFalse()
    {
        var a = Make("hello");
        var b = Make("world");
        _ = a.Value; _ = b.Value;
        Assert.False(a.Equals(b));
    }

    // ── Value NOT yet created ─────────────────────────────────────────────

    [Fact]
    public void Equals_NeitherValueCreated_SameValue_ReturnsTrue()
    {
        var a = Make("hello");
        var b = Make("hello");
        Assert.False(a.IsValueCreated);
        Assert.False(b.IsValueCreated);
        // Equals must compare values, which means forcing evaluation of both.
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_NeitherValueCreated_DifferentValues_ReturnsFalse()
    {
        var a = Make("hello");
        var b = Make("world");
        Assert.False(a.IsValueCreated);
        Assert.False(b.IsValueCreated);
        Assert.False(a.Equals(b));
    }

    [Fact]
    public void Equals_OneValueCreated_OtherNot_SameValue_ReturnsTrue()
    {
        var a = Make("hello");
        var b = Make("hello");
        _ = a.Value;
        Assert.True(a.IsValueCreated);
        Assert.False(b.IsValueCreated);
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_ForcesValueCreation_OnBothSides()
    {
        var a = Make("hello");
        var b = Make("hello");
        Assert.False(a.IsValueCreated);
        Assert.False(b.IsValueCreated);
        _ = a.Equals(b);
        Assert.True(a.IsValueCreated);
        Assert.True(b.IsValueCreated);
    }

    // ── Contracts ─────────────────────────────────────────────────────────

    [Fact]
    public void Equals_IsSymmetric()
    {
        var a = Make("hello");
        var b = Make("hello");
        Assert.Equal(a.Equals(b), b.Equals(a));
    }

    [Fact]
    public void GetHashCode_EqualCaches_SameHashCode()
    {
        var a = Make("hello");
        var b = Make("hello");
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ForcesValueCreation()
    {
        var c = Make("hello");
        Assert.False(c.IsValueCreated);
        _ = c.GetHashCode();
        Assert.True(c.IsValueCreated);
    }

    // ── Reset ─────────────────────────────────────────────────────────────

    [Fact]
    public void Reset_ClearsValueCreated()
    {
        var c = Make("hello");
        _ = c.Value;
        Assert.True(c.IsValueCreated);
        c.Reset();
        Assert.False(c.IsValueCreated);
    }

    [Fact]
    public void Equals_AfterReset_StillEqual()
    {
        var a = Make("hello");
        var b = Make("hello");
        _ = a.Value; _ = b.Value;
        Assert.True(a.Equals(b));
        a.Reset();
        Assert.False(a.IsValueCreated);
        Assert.True(a.Equals(b));
    }
}
