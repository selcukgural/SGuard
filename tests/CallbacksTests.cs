using SGuard.Exceptions;

namespace SGuard.Tests;

public sealed class CallbacksTests
{
    #region ThrowIf Tests

    private class CustomException : Exception
    {
        public CustomException() { }
        public CustomException(string message) : base(message) { }
    }

    // 1) LessThan — Failure and Success (OnFailure / OnSuccess)
    [Fact]
    public void LessThan_WhenLeftLess_Throws_And_OnFailureRuns()
    {
        var failureCalled = false;
        var cb = SGuardCallbacks.OnFailure(() => failureCalled = true);

        Assert.Throws<LessThanException>(() => ThrowIf.LessThan(1, 2, cb));

        Assert.True(failureCalled);
    }

    [Fact]
    public void LessThan_WhenLeftNotLess_DoesNotThrow_And_OnSuccessRuns()
    {
        var successCalled = false;
        var cb = SGuardCallbacks.OnSuccess(() => successCalled = true);

        ThrowIf.LessThan(5, 2, cb);

        Assert.True(successCalled);
    }

    // 2) Between (inclusive) — Failure inside range, Success outside range
    [Fact]
    public void Between_WhenInsideInclusive_Throws_And_OnFailureRuns()
    {
        var failureCalled = false;
        var cb = SGuardCallbacks.OnFailure(() => failureCalled = true);

        Assert.Throws<BetweenException>(() => ThrowIf.Between(5, 1, 10, cb));

        Assert.True(failureCalled);
    }

    [Fact]
    public void Between_WhenOutside_DoesNotThrow_And_OnSuccessRuns()
    {
        var successCalled = false;
        var cb = SGuardCallbacks.OnSuccess(() => successCalled = true);

        ThrowIf.Between(0, 1, 10, cb);

        Assert.True(successCalled);
    }

    // 3) NullOrEmpty (string) — Failure for null/empty, Success otherwise
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NullOrEmpty_String_WhenNullOrEmpty_Throws_And_OnFailureRuns(string? value)
    {
        var failureCalled = false;
        var cb = SGuardCallbacks.OnFailure(() => failureCalled = true);

        Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty(value, cb));

        Assert.True(failureCalled);
    }

    [Fact]
    public void NullOrEmpty_String_WhenNonEmpty_DoesNotThrow_And_OnSuccessRuns()
    {
        var successCalled = false;
        var cb = SGuardCallbacks.OnSuccess(() => successCalled = true);

        ThrowIf.NullOrEmpty("ok", cb);

        Assert.True(successCalled);
    }

    // 4) LessThanOrEqual — Failure when <=, Success when >
    [Fact]
    public void LessThanOrEqual_WhenEqual_Throws_And_OnFailureRuns()
    {
        var failureCalled = false;
        var cb = SGuardCallbacks.OnFailure(() => failureCalled = true);

        Assert.Throws<LessThanOrEqualException>(() => ThrowIf.LessThanOrEqual(10, 10, cb));

        Assert.True(failureCalled);
    }

    [Fact]
    public void LessThanOrEqual_WhenGreater_DoesNotThrow_And_OnSuccessRuns()
    {
        var successCalled = false;
        var cb = SGuardCallbacks.OnSuccess(() => successCalled = true);

        ThrowIf.LessThanOrEqual(20, 10, cb);

        Assert.True(successCalled);
    }

    // 5) Combined callbacks — only the matching branch runs
    [Fact]
    public void CombinedCallbacks_WhenFailure_OnlyFailureActionRuns()
    {
        int successCount = 0, failureCount = 0;
        var onFailure = SGuardCallbacks.OnFailure(() => failureCount++);
        var onSuccess = SGuardCallbacks.OnSuccess(() => successCount++);
        SGuardCallback combined = onFailure + onSuccess;

        Assert.Throws<BetweenException>(() => ThrowIf.Between(5, 1, 10, combined));

        Assert.Equal(1, failureCount);
        Assert.Equal(0, successCount);
    }

    [Fact]
    public void CombinedCallbacks_WhenSuccess_OnlySuccessActionRuns()
    {
        int successCount = 0, failureCount = 0;
        var onFailure = SGuardCallbacks.OnFailure(() => failureCount++);
        var onSuccess = SGuardCallbacks.OnSuccess(() => successCount++);
        SGuardCallback combined = onFailure + onSuccess;

        ThrowIf.Between(0, 1, 10, combined);

        Assert.Equal(0, failureCount);
        Assert.Equal(1, successCount);
    }

    // 6) Argument validation errors do NOT invoke callback
    [Fact]
    public void Between_NullExceptionInstance_ThrowsArgumentNullException_And_DoesNotCallCallback()
    {
        var called = false;
        SGuardCallback cb = _ => called = true;

        Assert.Throws<ArgumentNullException>(() => ThrowIf.Between(5, 1, 10, (CustomException)null!, cb));

        Assert.False(called);
    }

    // 7) Inline callback — verify the observed outcome value
    [Fact]
    public void InlineCallback_ObservesFailure_OnGuardThrow()
    {
        GuardOutcome? observed = null;

        Assert.Throws<LessThanException>(() => ThrowIf.LessThan(1, 2, outcome => observed = outcome));

        Assert.Equal(GuardOutcome.Failure, observed);
    }

    [Fact]
    public void InlineCallback_ObservesSuccess_OnGuardPass()
    {
        GuardOutcome? observed = null;

        ThrowIf.LessThan(5, 2, outcome => observed = outcome);

        Assert.Equal(GuardOutcome.Success, observed);
    }

    #endregion

    #region IsTests

    // 1) LessThan — OnSuccess when true, OnFailure when false
    [Fact]
    public void LessThan_WhenLeftLess_ResultTrue_And_OnSuccessRuns()
    {
        var successCalled = false;
        var cb = SGuardCallbacks.OnSuccess(() => successCalled = true);

        var result = Is.LessThan(1, 2, cb);

        Assert.True(result);
        Assert.True(successCalled);
    }

    [Theory]
    [InlineData(5, 2)] // greater
    [InlineData(5, 5)] // equal
    public void LessThan_WhenLeftEqualOrGreater_ResultFalse_And_OnFailureRuns(int left, int right)
    {
        var failureCalled = false;
        var cb = SGuardCallbacks.OnFailure(() => failureCalled = true);

        var result = Is.LessThan(left, right, cb);

        Assert.False(result);
        Assert.True(failureCalled);
    }

    // 2) Between (inclusive) — OnSuccess when inside, OnFailure when outside
    [Fact]
    public void Between_WhenInsideInclusive_ResultTrue_And_OnSuccessRuns()
    {
        var successCalled = false;
        var cb = SGuardCallbacks.OnSuccess(() => successCalled = true);

        var result = Is.Between(5, 1, 10, cb);

        Assert.True(result);
        Assert.True(successCalled);
    }

    [Theory]
    [InlineData(0, 1, 10)]
    [InlineData(11, 1, 10)]
    public void Between_WhenOutside_ResultFalse_And_OnFailureRuns(int value, int min, int max)
    {
        var failureCalled = false;
        var cb = SGuardCallbacks.OnFailure(() => failureCalled = true);

        var result = Is.Between(value, min, max, cb);

        Assert.False(result);
        Assert.True(failureCalled);
    }

    // 3) NullOrEmpty — OnSuccess when null/empty (result true), OnFailure otherwise
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NullOrEmpty_String_WhenNullOrEmpty_ResultTrue_And_OnSuccessRuns(string? value)
    {
        var successCalled = false;
        var cb = SGuardCallbacks.OnSuccess(() => successCalled = true);

        var result = Is.NullOrEmpty(value, cb);

        Assert.True(result);
        Assert.True(successCalled);
    }

    [Fact]
    public void NullOrEmpty_String_WhenNonEmpty_ResultFalse_And_OnFailureRuns()
    {
        var failureCalled = false;
        var cb = SGuardCallbacks.OnFailure(() => failureCalled = true);

        var result = Is.NullOrEmpty("ok", cb);

        Assert.False(result);
        Assert.True(failureCalled);
    }

    [Fact]
    public void NullOrEmpty_Collection_WhenEmpty_ResultTrue_And_OnSuccessRuns()
    {
        var successCalled = false;
        var cb = SGuardCallbacks.OnSuccess(() => successCalled = true);

        var result = Is.NullOrEmpty(new List<int>(), cb);

        Assert.True(result);
        Assert.True(successCalled);
    }

    [Fact]
    public void NullOrEmpty_Collection_WhenNonEmpty_ResultFalse_And_OnFailureRuns()
    {
        var failureCalled = false;
        var cb = SGuardCallbacks.OnFailure(() => failureCalled = true);

        var result = Is.NullOrEmpty(new List<int> { 1 }, cb);

        Assert.False(result);
        Assert.True(failureCalled);
    }

    // 4) LessThanOrEqual — OnSuccess when <=, OnFailure when >
    [Theory]
    [InlineData(1, 2)]   // less
    [InlineData(10, 10)] // equal
    public void LessThanOrEqual_WhenLessOrEqual_ResultTrue_And_OnSuccessRuns(int left, int right)
    {
        var successCalled = false;
        var cb = SGuardCallbacks.OnSuccess(() => successCalled = true);

        var result = Is.LessThanOrEqual(left, right, cb);

        Assert.True(result);
        Assert.True(successCalled);
    }

    [Fact]
    public void LessThanOrEqual_WhenGreater_ResultFalse_And_OnFailureRuns()
    {
        var failureCalled = false;
        var cb = SGuardCallbacks.OnFailure(() => failureCalled = true);

        var result = Is.LessThanOrEqual(20, 10, cb);

        Assert.False(result);
        Assert.True(failureCalled);
    }

    // 5) Combined callbacks — only matching branch runs per evaluation
    [Fact]
    public void CombinedCallbacks_WhenTrue_OnlySuccessRuns()
    {
        int successCount = 0, failureCount = 0;
        var onFailure = SGuardCallbacks.OnFailure(() => failureCount++);
        var onSuccess = SGuardCallbacks.OnSuccess(() => successCount++);
        SGuardCallback combined = onFailure + onSuccess;

        var result = Is.Between(5, 1, 10, combined);

        Assert.True(result);
        Assert.Equal(0, failureCount);
        Assert.Equal(1, successCount);
    }

    [Fact]
    public void CombinedCallbacks_WhenFalse_OnlyFailureRuns()
    {
        int successCount = 0, failureCount = 0;
        var onFailure = SGuardCallbacks.OnFailure(() => failureCount++);
        var onSuccess = SGuardCallbacks.OnSuccess(() => successCount++);
        SGuardCallback combined = onFailure + onSuccess;

        var result = Is.Between(0, 1, 10, combined);

        Assert.False(result);
        Assert.Equal(1, failureCount);
        Assert.Equal(0, successCount);
    }

    // 6) Argument validation errors do NOT invoke callback (e.g., null string args for LessThan with StringComparison)
    [Fact]
    public void LessThan_String_NullArguments_ThrowsArgumentNullException_And_DoesNotCallCallback()
    {
        var called = false;
        SGuardCallback cb = _ => called = true;

        Assert.Throws<ArgumentNullException>(() => Is.LessThan(null!, "b", StringComparison.Ordinal, cb));
        Assert.False(called);

        Assert.Throws<ArgumentNullException>(() => Is.LessThan("a", null!, StringComparison.Ordinal, cb));
        Assert.False(called);
    }

    // 7) Callback throwing exceptions should not propagate from Is.* (method returns the boolean result)
    [Fact]
    public void CallbackThrows_IsMethodsDoNotPropagateException_TrueAndFalsePaths()
    {
        SGuardCallback throwingCb = _ => throw new InvalidOperationException("from callback");

        // True path
        var r1 = Is.Between(5, 1, 10, throwingCb);
        Assert.True(r1);

        // False path
        var r2 = Is.Between(0, 1, 10, throwingCb);
        Assert.False(r2);
    }

    // 8) Inline callback — outcome matches boolean result (Success when true, Failure when false)
    [Fact]
    public void InlineCallback_OutcomeMatches_ResultTrue_Success()
    {
        GuardOutcome? observed = null;

        var result = Is.LessThan(1, 2, outcome => observed = outcome);

        Assert.True(result);
        Assert.Equal(GuardOutcome.Success, observed);
    }

    [Fact]
    public void InlineCallback_OutcomeMatches_ResultFalse_Failure()
    {
        GuardOutcome? observed = null;

        var result = Is.LessThan(5, 2, outcome => observed = outcome);

        Assert.False(result);
        Assert.Equal(GuardOutcome.Failure, observed);
    }

    #endregion
}