using LeaveMeAloneCSharp.Playground;
using Microsoft.Reactive.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace LeaveMeAloneCSharp.Test
{
    public class ReactiveLTests
    {
        [Fact]
        public async Task BasicStream_ShouldReturnFirstEven()
        {
            // Arrange
            var scheduler = new TestScheduler();

            // Act
            var result = await ReactiveL
                .RxBasicStream(scheduler)
                .FirstAsync();

            scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks); // to trigger the emission of the first value

            // Assert
            Assert.Equal("Even tick: 0", result);
        }

        [Fact]
        public void BasicStream_WithScheduler_ShouldEmitValues()
        {
            // Arrange + Act
            var scheduler = new TestScheduler();
            var results = new List<string>();

            ReactiveL.RxBasicStream(scheduler)
                .Subscribe(results.Add);

            scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            // Assert
            Assert.Contains("Even tick: 0", results);
            Assert.Contains("Even tick: 2", results);
        }

        [Fact]
        public async Task ErrorStream_ShouldThrow()
        {
            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await ReactiveL
                    .RxErrorStream()
                    .FirstAsync();
            });
        }

        [Fact]
        public async Task SubjectStream_ShouldFilter()
        {
            // Arrange + Act
            var source = new Subject<int>();
            var results = new List<int>();

            ReactiveL.RxSubjectStream(source) // value >= 10
                .Subscribe(results.Add);

            source.OnNext(5);
            source.OnNext(15);
            source.OnNext(25);

            source.OnCompleted();

            // Assert
            Assert.Equal(new List<int> { 15, 25 }, results);
        }

        [Fact]
        public async Task SearchStream_ShouldReturnResults()
        {
            // Arrange
            var input = Observable.Return("react"); // Simulate user input stream

            Task<List<string>> Fake(string q)       // Simulate a fake search API
            {
                return Task.FromResult(new List<string> { q + "_1" }); 
            }

            // Act
            var result = await ReactiveL
                .RxSearchStream(input, Fake)        // simulate user typing "react" and getting an aproximate search result "react_1"
                .SingleAsync();

            // Assert
            Assert.Equal("react_1", result[0]);
        }
    }
}
