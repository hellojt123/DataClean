//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/linq2db).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------

#pragma warning disable 1591

using System;
using System.Linq;

using LinqToDB;
using LinqToDB.Mapping;

namespace DataModels
{
	/// <summary>
	/// Database       : Database1
	/// Data Source    : Database1
	/// Server Version : 3.24.0
	/// </summary>
	public partial class Database1DB : LinqToDB.Data.DataConnection
	{
		public ITable<Test1> Test1 { get { return this.GetTable<Test1>(); } }

		public Database1DB()
		{
			InitDataContext();
			InitMappingSchema();
		}

		public Database1DB(string configuration)
			: base(configuration)
		{
			InitDataContext();
			InitMappingSchema();
		}

		partial void InitDataContext  ();
		partial void InitMappingSchema();
	}

	[Table("test1")]
	public partial class Test1
	{
		[Column("d_tid"), PrimaryKey,  NotNull] public long   DTid { get; set; } // integer
		[Column("id"),       Nullable         ] public string Id   { get; set; } // text(max)
		[Column("c1"),       Nullable         ] public string C1   { get; set; } // text(max)
		[Column("c2"),       Nullable         ] public string C2   { get; set; } // text(max)
		[Column("c3"),       Nullable         ] public string C3   { get; set; } // text(max)
	}

	public static partial class TableExtensions
	{
		public static Test1 Find(this ITable<Test1> table, long DTid)
		{
			return table.FirstOrDefault(t =>
				t.DTid == DTid);
		}
	}
}

#pragma warning restore 1591
