﻿using Atropos.Server.Listener;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atroposServerTest.Listener
{
	[TestFixture]
	public class MapperTest
	{
		[TestCase]
		public void MapSingle()
		{
			var mapper = new Mapper<SourceType, DestType>();

			var source = new SourceType { Id = 312, Name = "ababa", Period = TimeSpan.FromHours(1) };

			var dest = mapper.Map(source);

			Assert.AreEqual(source.Id, dest.Id);
			Assert.AreEqual(source.Name, dest.Name);
			Assert.AreEqual(source.Period, dest.Period);
		}

		[TestCase]
		public void MapEnumeration()
		{
			var mapper = new Mapper<SourceType, DestType>();

			var source = new SourceType { Id = 312, Name = "ababa", Period = TimeSpan.FromHours(1) };
			var sources = new List<SourceType>() { source, new SourceType { Id = 678, Name = "zxcv", Period = TimeSpan.FromHours(55) } };

			var destinations = mapper.Map(sources);

			Assert.AreEqual(sources.Count, destinations.Count());

			var dest = destinations.First();

			Assert.AreEqual(source.Id, dest.Id);
			Assert.AreEqual(source.Name, dest.Name);
			Assert.AreEqual(source.Period, dest.Period);

		}
	}

	public class SourceType
	{
		public int Id;
		public string Name { get; set; }
		public TimeSpan Period { get; set; }
	}

	public class DestType
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public TimeSpan Period; 
	}
}
