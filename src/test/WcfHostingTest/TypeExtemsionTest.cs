using com.Tools.WcfHosting.Extension;
using NUnit.Framework;
using System;

namespace WcfHostingTest
{
	[TestFixture]
	public class TypeExtemsionTest
	{
		[TestCase]
		public void GetMarked()
		{
			var typeMarked = typeof(ServiceNoAttrButInterface).GetMarked<SampleAttribute>();

			Assert.AreEqual(typeof(IService), typeMarked);

			typeMarked = typeof(ServiceAttr).GetMarked<SampleAttribute>();

			Assert.AreEqual(typeof(ServiceAttr), typeMarked);

			typeMarked = typeof(ServiceNoAttr).GetMarked<SampleAttribute>();

			Assert.AreEqual(default(Type), typeMarked);
		}
	}


	[Sample]
	public interface IService
	{

	}

	public interface INoService
	{ }

	public class ServiceNoAttrButInterface : IService, INoService
	{

	}

	[Sample]
	public class ServiceAttr : INoService
	{ }

	public class ServiceNoAttr
	{ }

	public class SampleAttribute : Attribute
	{

	}
}
