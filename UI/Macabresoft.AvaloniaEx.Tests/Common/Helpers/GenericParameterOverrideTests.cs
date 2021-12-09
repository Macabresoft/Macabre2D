namespace Macabresoft.AvaloniaEx.Tests;

using System;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Unity;

[TestFixture]
public class GenericParameterOverrideTests {
    private class SingleParameterTestClass {
        // ReSharper disable once MemberCanBeProtected.Local
        public SingleParameterTestClass(object firstParameter) {
            this.FirstParameter = firstParameter;
        }

        public object FirstParameter { get; }
    }

    private class DoubleParameterTestClass : SingleParameterTestClass {
        public DoubleParameterTestClass(object firstParameter, int secondParameter) : base(firstParameter) {
            this.SecondParameter = secondParameter;
        }

        public int SecondParameter { get; }
    }

    private class TripleParameterTestClass : DoubleParameterTestClass {
        public TripleParameterTestClass(object firstParameter, int secondParameter, Guid thirdParameter) : base(firstParameter, secondParameter) {
            this.ThirdParameter = thirdParameter;
        }

        public Guid ThirdParameter { get; }
    }

    [Test]
    [Category("Unit Tests")]
    public void DoubleParameterResolveTest() {
        var unityContainer = new UnityContainer();
        var firstParameter = new object();
        var secondParameter = new Random().Next();
        var resolvedObject = unityContainer.Resolve<DoubleParameterTestClass>(new GenericParameterOverride(firstParameter, secondParameter));

        using (new AssertionScope()) {
            resolvedObject.Should().NotBeNull();
            resolvedObject.FirstParameter.Should().Be(firstParameter);
            resolvedObject.SecondParameter.Should().Be(secondParameter);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public void SingleParameterResolveTest() {
        var unityContainer = new UnityContainer();
        var firstParameter = new object();
        var resolvedObject = unityContainer.Resolve<SingleParameterTestClass>(new GenericParameterOverride(firstParameter));

        using (new AssertionScope()) {
            resolvedObject.Should().NotBeNull();
            resolvedObject.FirstParameter.Should().Be(firstParameter);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public void TripleParameterResolveTest() {
        var unityContainer = new UnityContainer();
        var firstParameter = new object();
        var secondParameter = new Random().Next();
        var thirdParameter = Guid.NewGuid();
        var resolvedObject = unityContainer.Resolve<TripleParameterTestClass>(new GenericParameterOverride(firstParameter, secondParameter, thirdParameter));

        using (new AssertionScope()) {
            resolvedObject.Should().NotBeNull();
            resolvedObject.FirstParameter.Should().Be(firstParameter);
            resolvedObject.SecondParameter.Should().Be(secondParameter);
            resolvedObject.ThirdParameter.Should().Be(thirdParameter);
        }
    }
}