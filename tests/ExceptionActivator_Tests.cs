using System.Reflection;

namespace SGuard.Tests;

public sealed class TestPayload
{
    public string? NonEmptyString { get; set; } = "SGuard";
    public string? EmptyString { get; set; } = "";
    public string? NullString { get; set; } = null;
    public List<int>? NonEmptyList { get; set; } = [1];
    public List<int>? EmptyList { get; set; } = [];
}

public sealed class CustomParameterlessException : Exception
{
    public CustomParameterlessException() : base("Exception with default ctor.") { }
}

public class CustomArgumentException : Exception
{
    public CustomArgumentException(string message) : base(message) { }
}


public sealed class ExceptionActivatorTests
{
    public sealed class CustomTestException : Exception
    {
        public int? Code { get; }

        public CustomTestException() { }

        public CustomTestException(string message) : base(message) { }

        public CustomTestException(string message, Exception inner) : base(message, inner) { }

        public CustomTestException(int code, string message) : base($"{code}:{message}")
        {
            Code = code;
        }

        public CustomTestException(bool triggerThrow)
        {
            if (triggerThrow)
            {
                throw new InvalidOperationException("Constructor exploded");
            }
        }
    }

    [Fact]
    public void Create_WithNullArgs_UsesParameterlessCtor()
    {
        // act
        var ex = ExceptionActivator.Create<CustomTestException>(null);

        // assert
        Assert.NotNull(ex);
        Assert.IsType<CustomTestException>(ex);
        Assert.Null(ex.Message == null ? null : ex.InnerException); // sadece erişim; belirgin bir inner beklenmez
    }

    [Fact]
    public void Create_WithEmptyArgs_UsesParameterlessCtor()
    {
        // arrange
        object?[] args = [];

        // act
        var ex = ExceptionActivator.Create<CustomTestException>(args);

        // assert
        Assert.NotNull(ex);
        Assert.IsType<CustomTestException>(ex);
    }

    [Fact]
    public void Create_WithMessage_SetsMessage()
    {
        // arrange
        var message = "Hello world";
        object?[] args = [message];

        // act
        var ex = ExceptionActivator.Create<CustomTestException>(args);

        // assert
        Assert.Equal(message, ex.Message);
        Assert.Null(ex.InnerException);
    }

    [Fact]
    public void Create_WithMessageAndInner_SetsBoth()
    {
        // arrange
        var inner = new ApplicationException("inner");
        object?[] args = ["outer", inner];

        // act
        var ex = ExceptionActivator.Create<CustomTestException>(args);

        // assert
        Assert.Equal("outer", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }

    [Fact]
    public void Create_WithDifferentSignature_MatchesOverload()
    {
        // arrange
        object?[] args = [404, "not-found"];

        // act
        var ex = ExceptionActivator.Create<CustomTestException>(args);

        // assert
        Assert.Equal("404:not-found", ex.Message);
        Assert.Equal(404, ex.Code);
    }

    [Fact]
    public void Create_WithMismatchedArgs_ThrowsInvalidOperationException_WithClearMessage()
    {
        // arrange
        // CustomTestException için tek başına int alan kurucu yok, bu yüzden eşleşen ctor bulunamaz
        object?[] args = [123];

        // act
        var ex = Assert.Throws<InvalidOperationException>(() => ExceptionActivator.Create<CustomTestException>(args));

        // assert
        Assert.Contains($"No matching constructor found for '{typeof(CustomTestException).FullName}'", ex.Message);
    }

    [Fact]
    public void Create_WhenCtorThrows_PropagatesTargetInvocationException()
    {
        // arrange
        object?[] args = [true]; // bool alan kurucu, içinde InvalidOperationException fırlatacak

        // act
        var tie = Assert.Throws<TargetInvocationException>(() => ExceptionActivator.Create<CustomTestException>(args));

        // assert
        Assert.IsType<InvalidOperationException>(tie.InnerException);
        Assert.Equal("Constructor exploded", tie.InnerException!.Message);
    }
    
    #region Tests for NullOrEmpty<TValue, TException>(...) where TException : new()

        [Fact]
        public void NullOrEmpty_WithNewableException_DoesNotThrow_ForValidProperty()
        {
            // Arrange
            var payload = new TestPayload();
            var callbackCalled = false;
            GuardOutcome? outcome = null;

            // Act
            var exception = Record.Exception(() => ThrowIf.NullOrEmpty<TestPayload, CustomParameterlessException>(
                payload, 
                p => p.NonEmptyString, 
                result => {
                    callbackCalled = true;
                    outcome = result;
                }));

            // Assert
            Assert.Null(exception);
            Assert.True(callbackCalled);
            Assert.Equal(GuardOutcome.Success, outcome);
        }

        [Fact]
        public void NullOrEmpty_WithNewableException_Throws_ForNullRootValue()
        {
            // Arrange
            TestPayload? payload = null;

            // Act & Assert
            Assert.Throws<CustomParameterlessException>(() => 
                ThrowIf.NullOrEmpty<TestPayload, CustomParameterlessException>(payload!, p => p.NonEmptyString));
        }

        [Fact]
        public void NullOrEmpty_WithNewableException_Throws_ForNullProperty()
        {
            // Arrange
            var payload = new TestPayload();
            var callbackCalled = false;
            GuardOutcome? outcome = null;

            // Act & Assert
            Assert.Throws<CustomParameterlessException>(() => 
                ThrowIf.NullOrEmpty<TestPayload, CustomParameterlessException>(
                    payload, 
                    p => p.NullString, 
                    result => {
                        callbackCalled = true;
                        outcome = result;
                    }));

            Assert.True(callbackCalled);
            Assert.Equal(GuardOutcome.Failure, outcome);
        }

        [Fact]
        public void NullOrEmpty_WithNewableException_Throws_ForEmptyProperty()
        {
            // Arrange
            var payload = new TestPayload();
            
            // Act & Assert
            Assert.Throws<CustomParameterlessException>(() => 
                ThrowIf.NullOrEmpty<TestPayload, CustomParameterlessException>(payload, p => p.EmptyString));

            Assert.Throws<CustomParameterlessException>(() => 
                ThrowIf.NullOrEmpty<TestPayload, CustomParameterlessException>(payload, p => p.EmptyList));
        }

        [Fact]
        public void NullOrEmpty_WithNewableException_ThrowsArgumentNullException_ForNullSelector()
        {
            // Arrange
            var payload = new TestPayload();
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                ThrowIf.NullOrEmpty<TestPayload, CustomParameterlessException>(payload,selector: null!));
        }

        #endregion

        #region Tests for NullOrEmpty<TValue, TException>(..., object?[] constructorArgs, ...)

        [Fact]
        public void NullOrEmpty_WithCtorArgs_DoesNotThrow_ForValidProperty()
        {
            // Arrange
            var payload = new TestPayload();

            // Act
            var exception = Record.Exception(() => ThrowIf.NullOrEmpty<TestPayload, CustomArgumentException>(
                payload, 
                p => p.NonEmptyString, ["This should not be thrown"]));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void NullOrEmpty_WithCtorArgs_Throws_ForNullRootValue_WithMessage()
        {
            // Arrange
            TestPayload? payload = null;
            var message = "Payload cannot be null.";

            // Act & Assert
            var ex = Assert.Throws<CustomArgumentException>(() => 
                ThrowIf.NullOrEmpty<TestPayload, CustomArgumentException>(
                    payload!, 
                    p => p.NonEmptyString, [message]));

            Assert.Equal(message, ex.Message);
        }

        [Fact]
        public void NullOrEmpty_WithCtorArgs_Throws_ForEmptyProperty_WithMessage()
        {
            // Arrange
            var payload = new TestPayload();
            var message = "Property cannot be empty.";
            var callbackCalled = false;
            GuardOutcome? outcome = null;

            // Act & Assert
            var ex = Assert.Throws<CustomArgumentException>(() => 
                ThrowIf.NullOrEmpty<TestPayload, CustomArgumentException>(
                    payload, 
                    p => p.EmptyString, [message], 
                    result => {
                        callbackCalled = true;
                        outcome = result;
                    }));
            
            Assert.Equal(message, ex.Message);
            Assert.True(callbackCalled);
            Assert.Equal(GuardOutcome.Failure, outcome);
        }
        
        [Fact]
        public void NullOrEmpty_WithCtorArgs_Throws_ForNullProperty_WithMessage()
        {
            // Arrange
            var payload = new TestPayload();
            var message = "Property cannot be null.";

            // Act & Assert
            var ex = Assert.Throws<CustomArgumentException>(() => 
                ThrowIf.NullOrEmpty<TestPayload, CustomArgumentException>(
                    payload, 
                    p => p.NullString, [message]));
            
            Assert.Equal(message, ex.Message);
        }

        [Fact]
        public void NullOrEmpty_WithMismatchedCtorArgs_ThrowsInvalidOperationException()
        {
            // Arrange
            var payload = new TestPayload();

            // Act & Assert
            // CustomArgumentException'ın (int, string) alan bir ctor'u yok.
            var ex = Assert.Throws<InvalidOperationException>(() => 
                ThrowIf.NullOrEmpty<TestPayload, CustomArgumentException>(
                    payload, 
                    p => p.EmptyString, [123, "wrong type"]));

            Assert.Contains("No matching constructor found", ex.Message);
        }

        #endregion

}