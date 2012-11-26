﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.296
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NinjaTest
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
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="Sim")]
	public partial class SimDbDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertSS_RSI(SS_RSI instance);
    partial void UpdateSS_RSI(SS_RSI instance);
    partial void DeleteSS_RSI(SS_RSI instance);
    #endregion
		
		public SimDbDataContext() : 
				base(global::NinjaTest.Properties.Settings.Default.SimConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public SimDbDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SimDbDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SimDbDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SimDbDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<SS_RSI> SS_RSIs
		{
			get
			{
				return this.GetTable<SS_RSI>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.SS_RSI")]
	public partial class SS_RSI : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private long _C;
		
		private System.Nullable<int> _RSI;
		
		private System.Nullable<int> _RSI_MA;
		
		private System.Nullable<int> _RSI_MA2;
		
		private System.Nullable<int> _Mid_Long;
		
		private System.Nullable<int> _Mid_Short;
		
		private System.Nullable<int> _CloseLong;
		
		private System.Nullable<int> _CloseShort;
		
		private System.Nullable<long> _Total_Profit;
		
		private System.Nullable<decimal> _Total_avg_PL;
		
		private System.Nullable<int> _Trade_Count;
		
		private System.Nullable<decimal> _PL_Ratio;
		
		private System.Nullable<decimal> _Avg_Profit;
		
		private System.Nullable<decimal> _Avg_Loss;
		
		private System.Nullable<decimal> _Pct_Profit;
		
		private System.Nullable<decimal> _Pct_Loss;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnCChanging(long value);
    partial void OnCChanged();
    partial void OnRSIChanging(System.Nullable<int> value);
    partial void OnRSIChanged();
    partial void OnRSI_MAChanging(System.Nullable<int> value);
    partial void OnRSI_MAChanged();
    partial void OnRSI_MA2Changing(System.Nullable<int> value);
    partial void OnRSI_MA2Changed();
    partial void OnMid_LongChanging(System.Nullable<int> value);
    partial void OnMid_LongChanged();
    partial void OnMid_ShortChanging(System.Nullable<int> value);
    partial void OnMid_ShortChanged();
    partial void OnCloseLongChanging(System.Nullable<int> value);
    partial void OnCloseLongChanged();
    partial void OnCloseShortChanging(System.Nullable<int> value);
    partial void OnCloseShortChanged();
    partial void OnTotal_ProfitChanging(System.Nullable<long> value);
    partial void OnTotal_ProfitChanged();
    partial void OnTotal_avg_PLChanging(System.Nullable<decimal> value);
    partial void OnTotal_avg_PLChanged();
    partial void OnTrade_CountChanging(System.Nullable<int> value);
    partial void OnTrade_CountChanged();
    partial void OnPL_RatioChanging(System.Nullable<decimal> value);
    partial void OnPL_RatioChanged();
    partial void OnAvg_ProfitChanging(System.Nullable<decimal> value);
    partial void OnAvg_ProfitChanged();
    partial void OnAvg_LossChanging(System.Nullable<decimal> value);
    partial void OnAvg_LossChanged();
    partial void OnPct_ProfitChanging(System.Nullable<decimal> value);
    partial void OnPct_ProfitChanged();
    partial void OnPct_LossChanging(System.Nullable<decimal> value);
    partial void OnPct_LossChanged();
    #endregion
		
		public SS_RSI()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_C", AutoSync=AutoSync.OnInsert, DbType="BigInt NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public long C
		{
			get
			{
				return this._C;
			}
			set
			{
				if ((this._C != value))
				{
					this.OnCChanging(value);
					this.SendPropertyChanging();
					this._C = value;
					this.SendPropertyChanged("C");
					this.OnCChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RSI", DbType="Int")]
		public System.Nullable<int> RSI
		{
			get
			{
				return this._RSI;
			}
			set
			{
				if ((this._RSI != value))
				{
					this.OnRSIChanging(value);
					this.SendPropertyChanging();
					this._RSI = value;
					this.SendPropertyChanged("RSI");
					this.OnRSIChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RSI_MA", DbType="Int")]
		public System.Nullable<int> RSI_MA
		{
			get
			{
				return this._RSI_MA;
			}
			set
			{
				if ((this._RSI_MA != value))
				{
					this.OnRSI_MAChanging(value);
					this.SendPropertyChanging();
					this._RSI_MA = value;
					this.SendPropertyChanged("RSI_MA");
					this.OnRSI_MAChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RSI_MA2", DbType="Int")]
		public System.Nullable<int> RSI_MA2
		{
			get
			{
				return this._RSI_MA2;
			}
			set
			{
				if ((this._RSI_MA2 != value))
				{
					this.OnRSI_MA2Changing(value);
					this.SendPropertyChanging();
					this._RSI_MA2 = value;
					this.SendPropertyChanged("RSI_MA2");
					this.OnRSI_MA2Changed();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Mid_Long", DbType="Int")]
		public System.Nullable<int> Mid_Long
		{
			get
			{
				return this._Mid_Long;
			}
			set
			{
				if ((this._Mid_Long != value))
				{
					this.OnMid_LongChanging(value);
					this.SendPropertyChanging();
					this._Mid_Long = value;
					this.SendPropertyChanged("Mid_Long");
					this.OnMid_LongChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Mid_Short", DbType="Int")]
		public System.Nullable<int> Mid_Short
		{
			get
			{
				return this._Mid_Short;
			}
			set
			{
				if ((this._Mid_Short != value))
				{
					this.OnMid_ShortChanging(value);
					this.SendPropertyChanging();
					this._Mid_Short = value;
					this.SendPropertyChanged("Mid_Short");
					this.OnMid_ShortChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CloseLong", DbType="Int")]
		public System.Nullable<int> CloseLong
		{
			get
			{
				return this._CloseLong;
			}
			set
			{
				if ((this._CloseLong != value))
				{
					this.OnCloseLongChanging(value);
					this.SendPropertyChanging();
					this._CloseLong = value;
					this.SendPropertyChanged("CloseLong");
					this.OnCloseLongChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CloseShort", DbType="Int")]
		public System.Nullable<int> CloseShort
		{
			get
			{
				return this._CloseShort;
			}
			set
			{
				if ((this._CloseShort != value))
				{
					this.OnCloseShortChanging(value);
					this.SendPropertyChanging();
					this._CloseShort = value;
					this.SendPropertyChanged("CloseShort");
					this.OnCloseShortChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Total_Profit", DbType="BigInt")]
		public System.Nullable<long> Total_Profit
		{
			get
			{
				return this._Total_Profit;
			}
			set
			{
				if ((this._Total_Profit != value))
				{
					this.OnTotal_ProfitChanging(value);
					this.SendPropertyChanging();
					this._Total_Profit = value;
					this.SendPropertyChanged("Total_Profit");
					this.OnTotal_ProfitChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Total_avg_PL", DbType="Decimal(18,0)")]
		public System.Nullable<decimal> Total_avg_PL
		{
			get
			{
				return this._Total_avg_PL;
			}
			set
			{
				if ((this._Total_avg_PL != value))
				{
					this.OnTotal_avg_PLChanging(value);
					this.SendPropertyChanging();
					this._Total_avg_PL = value;
					this.SendPropertyChanged("Total_avg_PL");
					this.OnTotal_avg_PLChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Trade_Count", DbType="Int")]
		public System.Nullable<int> Trade_Count
		{
			get
			{
				return this._Trade_Count;
			}
			set
			{
				if ((this._Trade_Count != value))
				{
					this.OnTrade_CountChanging(value);
					this.SendPropertyChanging();
					this._Trade_Count = value;
					this.SendPropertyChanged("Trade_Count");
					this.OnTrade_CountChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PL_Ratio", DbType="Decimal(18,0)")]
		public System.Nullable<decimal> PL_Ratio
		{
			get
			{
				return this._PL_Ratio;
			}
			set
			{
				if ((this._PL_Ratio != value))
				{
					this.OnPL_RatioChanging(value);
					this.SendPropertyChanging();
					this._PL_Ratio = value;
					this.SendPropertyChanged("PL_Ratio");
					this.OnPL_RatioChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Avg_Profit", DbType="Decimal(18,0)")]
		public System.Nullable<decimal> Avg_Profit
		{
			get
			{
				return this._Avg_Profit;
			}
			set
			{
				if ((this._Avg_Profit != value))
				{
					this.OnAvg_ProfitChanging(value);
					this.SendPropertyChanging();
					this._Avg_Profit = value;
					this.SendPropertyChanged("Avg_Profit");
					this.OnAvg_ProfitChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Avg_Loss", DbType="Decimal(18,0)")]
		public System.Nullable<decimal> Avg_Loss
		{
			get
			{
				return this._Avg_Loss;
			}
			set
			{
				if ((this._Avg_Loss != value))
				{
					this.OnAvg_LossChanging(value);
					this.SendPropertyChanging();
					this._Avg_Loss = value;
					this.SendPropertyChanged("Avg_Loss");
					this.OnAvg_LossChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Pct_Profit", DbType="Decimal(18,0)")]
		public System.Nullable<decimal> Pct_Profit
		{
			get
			{
				return this._Pct_Profit;
			}
			set
			{
				if ((this._Pct_Profit != value))
				{
					this.OnPct_ProfitChanging(value);
					this.SendPropertyChanging();
					this._Pct_Profit = value;
					this.SendPropertyChanged("Pct_Profit");
					this.OnPct_ProfitChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Pct_Loss", DbType="Decimal(18,0)")]
		public System.Nullable<decimal> Pct_Loss
		{
			get
			{
				return this._Pct_Loss;
			}
			set
			{
				if ((this._Pct_Loss != value))
				{
					this.OnPct_LossChanging(value);
					this.SendPropertyChanging();
					this._Pct_Loss = value;
					this.SendPropertyChanged("Pct_Loss");
					this.OnPct_LossChanged();
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