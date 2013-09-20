﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AlgoSecondLayer
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="STOCHPOP_RESULTS")]
	public partial class SPPOPRESULTDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InserttblResults_5Min(tblResults_5Min instance);
    partial void UpdatetblResults_5Min(tblResults_5Min instance);
    partial void DeletetblResults_5Min(tblResults_5Min instance);
    #endregion
		
		public SPPOPRESULTDataContext() : 
				base(global::AlgoSecondLayer.Properties.Settings.Default.STOCHPOP_RESULTSConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public SPPOPRESULTDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SPPOPRESULTDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SPPOPRESULTDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SPPOPRESULTDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<tblResults_5Min> tblResults_5Mins
		{
			get
			{
				return this.GetTable<tblResults_5Min>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.tblResults_5Min")]
	public partial class tblResults_5Min : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _Sequence;
		
		private double _Profit;
		
		private int _Trades;
		
		private string _Notes;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnSequenceChanging(string value);
    partial void OnSequenceChanged();
    partial void OnProfitChanging(double value);
    partial void OnProfitChanged();
    partial void OnTradesChanging(int value);
    partial void OnTradesChanged();
    partial void OnNotesChanging(string value);
    partial void OnNotesChanged();
    #endregion
		
		public tblResults_5Min()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Sequence", DbType="VarChar(200) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string Sequence
		{
			get
			{
				return this._Sequence;
			}
			set
			{
				if ((this._Sequence != value))
				{
					this.OnSequenceChanging(value);
					this.SendPropertyChanging();
					this._Sequence = value;
					this.SendPropertyChanged("Sequence");
					this.OnSequenceChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Profit", DbType="Float NOT NULL")]
		public double Profit
		{
			get
			{
				return this._Profit;
			}
			set
			{
				if ((this._Profit != value))
				{
					this.OnProfitChanging(value);
					this.SendPropertyChanging();
					this._Profit = value;
					this.SendPropertyChanged("Profit");
					this.OnProfitChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Trades", DbType="Int NOT NULL")]
		public int Trades
		{
			get
			{
				return this._Trades;
			}
			set
			{
				if ((this._Trades != value))
				{
					this.OnTradesChanging(value);
					this.SendPropertyChanging();
					this._Trades = value;
					this.SendPropertyChanged("Trades");
					this.OnTradesChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Notes", DbType="VarChar(MAX)")]
		public string Notes
		{
			get
			{
				return this._Notes;
			}
			set
			{
				if ((this._Notes != value))
				{
					this.OnNotesChanging(value);
					this.SendPropertyChanging();
					this._Notes = value;
					this.SendPropertyChanged("Notes");
					this.OnNotesChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
