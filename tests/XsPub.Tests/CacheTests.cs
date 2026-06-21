using Xunit;
using XsPub.Library;

namespace XsPub.Tests;

public class CacheTests
{
    private static Cache<string> Make(string value) => new(() => value);

    // ── Self / null ───────────────────────────────────────────────────────

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

    // ── Cache<T> equality ─────────────────────────────────────────────────

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

    [Fact]
    public void Equals_NeitherValueCreated_SameValue_ReturnsTrue()
    {
        var a = Make("hello");
        var b = Make("hello");
        Assert.False(a.IsValueCreated);
        Assert.False(b.IsValueCreated);
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

    // ── Lazy<T> equality ─────────────────────────────────────────────────

    [Fact]
    public void Equals_Lazy_SameValue_ReturnsTrue()
    {
        var cache = Make("hello");
        var lazy = new Lazy<string>(() => "hello");
        Assert.True(cache.Equals(lazy));
    }

    [Fact]
    public void Equals_Lazy_DifferentValue_ReturnsFalse()
    {
        var cache = Make("hello");
        var lazy = new Lazy<string>(() => "world");
        Assert.False(cache.Equals(lazy));
    }

    [Fact]
    public void Equals_NullLazy_ReturnsFalse()
    {
        Assert.False(Make("hello").Equals((Lazy<string>?)null));
    }

    [Fact]
    public void Equals_Lazy_ForcesEvaluationOfBothSides()
    {
        var cache = Make("hello");
        var lazy = new Lazy<string>(() => "hello");
        Assert.False(cache.IsValueCreated);
        Assert.False(lazy.IsValueCreated);
        _ = cache.Equals(lazy);
        Assert.True(cache.IsValueCreated);
        Assert.True(lazy.IsValueCreated);
    }

    [Fact]
    public void EqualsObject_Lazy_SameValue_ReturnsTrue()
    {
        var cache = Make("hello");
        Assert.True(cache.Equals((object)new Lazy<string>(() => "hello")));
    }

    // ── Raw T equality ────────────────────────────────────────────────────

    [Fact]
    public void Equals_RawValue_SameValue_ReturnsTrue()
    {
        Assert.True(Make("hello").Equals("hello"));
    }

    [Fact]
    public void Equals_RawValue_DifferentValue_ReturnsFalse()
    {
        Assert.False(Make("hello").Equals("world"));
    }

    [Fact]
    public void Equals_NullRawValue_ReturnsFalse()
    {
        Assert.False(Make("hello").Equals((string?)null));
    }

    [Fact]
    public void Equals_RawValue_ForcesValueCreation()
    {
        var cache = Make("hello");
        Assert.False(cache.IsValueCreated);
        _ = cache.Equals("hello");
        Assert.True(cache.IsValueCreated);
    }

    [Fact]
    public void EqualsObject_RawValue_SameValue_ReturnsTrue()
    {
        var cache = Make("hello");
        _ = cache.Value;
        Assert.True(cache.Equals((object)"hello"));
    }

    // ── Truly incompatible type ───────────────────────────────────────────

    [Fact]
    public void EqualsObject_UnrelatedType_ReturnsFalse()
    {
        // object is not Cache<string>, Lazy<string>, or string
        Assert.False(Make("hello").Equals(new object()));
    }

    [Fact]
    public void EqualsObject_IncompatibleType_DoesNotForceValueCreation()
    {
        // An unrecognised type short-circuits before touching Value
        var cache = Make("hello");
        Assert.False(cache.IsValueCreated);
        Assert.False(cache.Equals(new object()));
        Assert.False(cache.IsValueCreated);
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
    public void GetHashCode_MatchesValueHashCode()
    {
        var cache = Make("hello");
        Assert.Equal("hello".GetHashCode(), cache.GetHashCode());
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
