using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpDevLib.Extensions.DI;
using System;

namespace SharpDevLib.Extensions.Tests.DI;

#region TestClasses
public interface I1 { }
public interface I2 : I1 { }
public interface I3 { }
internal interface I4 { }
public class A : I2, I3, I4
{
    public A()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
}

public interface IB1 { }
public class B : IB1 { }

public interface IC1 { }
public class C : IC1 { }

public interface IG1<T1, T2> where T1 : class where T2 : class
{

}

public interface IG : IG1<A, B>
{

}

public class G1 : IG1<A, B>
{

}

public class G2 : IG1<B, C>
{

}

public class G3 : IG
{

}
#endregion

[TestClass]
public class DITests
{
    [TestMethod]
    public void AddByAssemblyTest()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddByAssembly(this.GetType().Assembly, typeof(B));
        var serviceProvider = services.BuildServiceProvider();

        var a0 = serviceProvider.GetService<I1>();
        var a1 = serviceProvider.GetService<I1>();
        var a2 = serviceProvider.GetService<I2>();
        var a3 = serviceProvider.GetService<I3>();
        var a4 = serviceProvider.GetService<I4>();
        var b1 = serviceProvider.GetService<IB1>();
        var c1 = serviceProvider.GetService<IC1>();

        Assert.AreEqual(a0, a1);
        Assert.AreEqual(a1!.GetType(), a2!.GetType());
        Assert.AreEqual(a1.GetType(), a3!.GetType());
        Assert.IsNull(a4);
        Assert.IsNull(b1);
        Assert.IsNotNull(c1);
    }

    [TestMethod]
    public void AddByImplementionTest()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddByImplemention<A>();
        var serviceProvider = services.BuildServiceProvider();

        var a0 = serviceProvider.GetService<I1>();
        var a1 = serviceProvider.GetService<I1>();
        var a2 = serviceProvider.GetService<I2>();
        var a3 = serviceProvider.GetService<I3>();
        var a4 = serviceProvider.GetService<I4>();
        var b1 = serviceProvider.GetService<IB1>();
        var c1 = serviceProvider.GetService<IC1>();

        Assert.AreEqual(a0, a1);
        Assert.AreEqual(a1!.GetType(), a2!.GetType());
        Assert.AreEqual(a1.GetType(), a3!.GetType());
        Assert.IsNull(a4);
        Assert.IsNull(b1);
        Assert.IsNull(c1);
    }

    [TestMethod]
    public void AddByInterfaceTest()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddByInterface<I1>(this.GetType().Assembly);
        var serviceProvider = services.BuildServiceProvider();

        var a0 = serviceProvider.GetService<I1>();
        var a1 = serviceProvider.GetService<I1>();
        var a2 = serviceProvider.GetService<I2>();
        var a3 = serviceProvider.GetService<I3>();
        var a4 = serviceProvider.GetService<I4>();
        var b1 = serviceProvider.GetService<IB1>();
        var c1 = serviceProvider.GetService<IC1>();

        Assert.AreEqual(a0, a1);
        Assert.AreEqual(a1!.GetType(), a2!.GetType());
        Assert.AreEqual(a1.GetType(), a3!.GetType());
        Assert.IsNull(a4);
        Assert.IsNull(b1);
        Assert.IsNull(c1);
    }

    [TestMethod]
    public void AddByGenericInterfaceTest()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddByInterface(typeof(IG1<,>), this.GetType().Assembly);
        var serviceProvider = services.BuildServiceProvider();

        var instanceA = serviceProvider.GetService<IG1<A, B>>();
        var instanceB = serviceProvider.GetService<IG1<B, C>>();
        var instanceC = serviceProvider.GetService<IG1<A, C>>();
        var instance = serviceProvider.GetService<IG>();

        Assert.IsNotNull(instanceA);
        Assert.IsNotNull(instanceB);
        Assert.IsNull(instanceC);
        Assert.IsNotNull(instance);
        Assert.AreEqual(instanceA.GetType(), instance.GetType());
    }
}
