using LeaveMeAloneFuncSkillForge.Common;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class StateTests
    {
        [Fact]
        public void ToState_ShouldInitializeStateAndValue()
        {
            var state = "user".ToState(42);

            Assert.Equal("user", state.CurrentState);
            Assert.Equal(42, state.CurrentValue);
        }

        [Fact]
        public void ToState_ShouldWorkWithDifferentTypes()
        {
            var state = 10.ToState("hello");

            Assert.Equal(10, state.CurrentState);
            Assert.Equal("hello", state.CurrentValue);
        }

        [Fact]
        public void ToState_ShouldAcceptNullStateReferenceType()
        {
            string? s = null;

            var state = s.ToState(99);

            Assert.Null(state.CurrentState);
            Assert.Equal(99, state.CurrentValue);
        }

        [Fact]
        public void ToState_ShouldAcceptNullValueReferenceType()
        {
            var state = "state".ToState<string, string?>(null);

            Assert.Equal("state", state.CurrentState);
            Assert.Null(state.CurrentValue);
        }

        [Fact]
        public void Update_ShouldModifyState()
        {
            var state = 10.ToState("value");

            var updated = state.Update(s => s + 5);

            Assert.Equal(15, updated.CurrentState);
            Assert.Equal("value", updated.CurrentValue); // value must not change
        }

        [Fact]
        public void Update_ShouldNotModifyValue()
        {
            var state = "initial".ToState(100);

            var updated = state.Update(s => "changed");

            Assert.Equal("changed", updated.CurrentState);
            Assert.Equal(100, updated.CurrentValue);
        }

        [Fact]
        public void Update_ShouldWorkWithReferenceTypes()
        {
            var obj = new TestObj { Num = 1 };
            var state = obj.ToState(5);

            var updated = state.Update(s =>
            {
                s.Num = 10;
                return s;
            });

            Assert.Equal(10, updated.CurrentState.Num);
            Assert.Equal(5, updated.CurrentValue);
        }

        [Fact]
        public void Update_ShouldBeIdempotentWhenIdentityFunction()
        {
            var state = 50.ToState("abc");

            var updated = state.Update(s => s);

            Assert.Equal(50, updated.CurrentState);
            Assert.Equal("abc", updated.CurrentValue);
        }

        [Fact]
        public void Update_ShouldAllowNullState()
        {
            string? initial = null;
            var state = initial.ToState("data");

            var updated = state.Update(s => s == null ? "fixed" : s);

            Assert.Equal("fixed", updated.CurrentState);
            Assert.Equal("data", updated.CurrentValue);
        }

        [Fact]
        public void Update_ShouldCreateNewInstanceEveryTime()
        {
            var state = 1.ToState("test");

            var updated = state.Update(s => s + 1);

            Assert.NotSame(state, updated); // must be new object
        }

        [Fact]
        public void Bind_ShouldChainMultipleStepsCorrectly()
        {
            var result = 10.ToState(2)
                .Bind((s, x) => x + s)   // 2 + 10 = 12
                .Bind((s, x) => x * s)   // 12 * 10 = 120
                .Bind((s, x) => x - 5);  // 120 - 5 = 115

            Assert.Equal(10, result.CurrentState);
            Assert.Equal(115, result.CurrentValue);
        }

        [Fact]
        public void Bind_ShouldPreserveStateIfNoUpdate()
        {
            var result = "user123".ToState(1)
                .Bind((s, x) => x + 5)
                .Bind((s, x) => x * 2);

            Assert.Equal("user123", result.CurrentState);
            Assert.Equal(12, result.CurrentValue);
        }

        [Fact]
        public void Bind_WithUpdate_ShouldAffectNextBind()
        {
            var result = 10.ToState(3)
                .Bind((s, x) => x * s)       // 3 * 10 = 30
                .Update(s => s + 5)          // new state = 15
                .Bind((s, x) => x + s);      // 30 + 15 = 45

            Assert.Equal(15, result.CurrentState);
            Assert.Equal(45, result.CurrentValue);
        }

        [Fact]
        public void Bind_ShouldSupportChangingValueType()
        {
            var result = 100.ToState(20)
                .Bind((s, x) => x > 10)           // bool
                .Bind((s, x) => x ? "OK" : "NO")  // string
                .Bind((s, x) => x.Length);        // int

            Assert.Equal(100, result.CurrentState);
            Assert.Equal(2, result.CurrentValue); // "OK".Length = 2
        }

        [Fact]
        public void Bind_ShouldWorkWithReferenceTypes()
        {
            var result = "token123".ToState(new Customer { Id = 5 })
                .Bind((state, customer) => new Order { CustomerId = customer.Id })
                .Bind((state, order) => order.CustomerId * 2);

            Assert.Equal("token123", result.CurrentState);
            Assert.Equal(10, result.CurrentValue);
        }

        [Fact]
        public void Bind_ShouldBeAssociative()
        {
            Func<int, int, int> f = (s, x) => x + 1;
            Func<int, int, int> g = (s, x) => x * 2;

            var a = 10.ToState(5)
                .Bind((s, x) => f(s, x))
                .Bind((s, x) => g(s, x));

            var b = 10.ToState(5)
                .Bind((s, x) => g(s, f(s, x)));

            Assert.Equal(a.CurrentValue, b.CurrentValue);
            Assert.Equal(a.CurrentState, b.CurrentState);
        }

        [Fact]
        public void Bind_ShouldWorkAfterMultipleUpdates()
        {
            var result = 1.ToState(10)
                .Update(s => s + 1)   // 2
                .Update(s => s + 3)   // 5
                .Bind((s, x) => x * s); // 10 * 5 = 50

            Assert.Equal(5, result.CurrentState);
            Assert.Equal(50, result.CurrentValue);
        }

        [Fact]
        public void Bind_ComplexFlow_ShouldProduceCorrectResult()
        {
            var result = 2.ToState(3)
                .Bind((s, x) => x + s)    // 3 + 2 = 5
                .Update(s => s * 10)      // state = 20
                .Bind((s, x) => x * s)    // 5 * 20 = 100
                .Update(s => s - 15)      // state = 5
                .Bind((s, x) => x - s);   // 100 - 5 = 95

            Assert.Equal(5, result.CurrentState);
            Assert.Equal(95, result.CurrentValue);
        }

        [Fact]
        public void Bind_ShouldHandleNullValue()
        {
            var result = "ctx".ToState<string, string?>(null)
                .Bind((s, x) => x == null ? 99 : -1);

            Assert.Equal("ctx", result.CurrentState);
            Assert.Equal(99, result.CurrentValue);
        }

        [Fact]
        public void Bind_ShouldAllowFunctionsThatUseOnlyState()
        {
            var result = 7.ToState(0)
                .Bind((s, x) => s * s); // 7 * 7 = 49

            Assert.Equal(49, result.CurrentValue);
        }

        // Helper classes for type testing
        private class TestObj
        {
            public int Num { get; set; }
        }

        private class Customer { public int Id { get; set; } }
        private class Order { public int CustomerId { get; set; } }
    }
}
