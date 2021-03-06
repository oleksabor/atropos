﻿using Atropos.Server.Db;
using Atropos.Server.Listener;
using LinqToDB.Mapping;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atroposTest.Listener
{
	[TestFixture]
	public class MapperTest
	{
		[TestCase]
		public void MapSingle()
		{
			var mapper = new Mapper<SourceType, DestType>();

			var source = new SourceType { Id = 312, Name = "ababa", Period = TimeSpan.FromHours(1), NonExisitngInDest = "blabla" };

			var dest = mapper.Map(source);

			Assert.AreEqual(source.Id, dest.Id);
			Assert.AreEqual(source.Name, dest.Name);
			Assert.AreEqual(source.Period, dest.Period);
		}

		[TestCase]
		public void MapSingleReverse()
		{
			var mapper = new Mapper<DestType, SourceType>();

			var source = new DestType { Id = 312, Name = "ababa", Period = TimeSpan.FromHours(1) };

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

		[TestCase]
		public void MapUsageLog()
		{
			var mapper = new Mapper<UsageLog, Atropos.Common.Dto.UsageLog>();
			
			var dto = mapper.Map(new UsageLog { Id = 12, Used = TimeSpan.FromSeconds(99) });
			Assert.AreEqual(12, dto.Id);
		}
	}

	public class SourceType
	{
		[PrimaryKey, Identity] public int Id;
		public string Name { get; set; }
		public TimeSpan Period { get; set; }

		public string NonExisitngInDest { get; set; }
	}

	public class DestType
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public TimeSpan Period; 
	}
}
