﻿using Atropos.Common;
using Atropos.Common.DateTimeConv;
using Atropos.Common.Dto;
using client.Wpf;
using client.Wpf.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace client.Data
{
	public class DataLoader : PropertyChangedBase, IDisposable
	{
		public DataLoader(IDataService service)
		{
			this.Service = service;
			date = DateTime.Today;

			Users = new DataItems<User>(LoadUsers);
			UsageLog = new DataItems<UsageLog>(LoadUsageLog);
			Curfews = new DataItems<CurfewGui>(LoadCurfews);
			WeekUsages = new DataItems<UsageWeekLog>(LoadWeekUsage);

			AddCurfew = new DelegateCommand() { CommandAction = () => AddCurfewMethod(), CanExecuteFunc = () => SelectedUser != null };
			DelCurfew = new DelegateCommand() { CommandAction = () => DelCurfewMethod(), CanExecuteFunc = () => SelectedCurfew != null };
			SaveCurfews = new DelegateCommand() { CommandAction = () => SaveCurfewsMethod(), CanExecuteFunc = () => SelectedUser != null };
			ReloadUsageLog = new DelegateCommand() { CommandAction = () => ReloadUsageLogMethod(), CanExecuteFunc = () => SelectedUser != null };
		}

		ObservableCollection<UsageLog> LoadUsageLog()
		{
			var usages = Service.GetUsageLog(SelectedUser?.Login, Date);

			if (usages != null && usages.Any())
				UsedTotal = usages.Sum(_ => _.UsedValue).ToTime();
			else
				UsedTotal = TimeSpan.Zero;

			return new ObservableCollection<UsageLog>(usages);
		}

		ObservableCollection<User> LoadUsers()
		{
			var users = Service.GetUsers();
			return new ObservableCollection<User>(users);
		}

		ObservableCollection<CurfewGui> LoadCurfews()
		{
			var curfews = Service.GetCurfews(SelectedUser?.Login);
			return new ObservableCollection<CurfewGui>(curfews.Select(_ => _.ToGui()));
		}

#if DEBUG
		static Random random = new Random();

		public static bool RandomizeIfZero = true;
#endif

		ObservableCollection<UsageWeekLog> LoadWeekUsage()
		{
			var today = DateTime.Today;
			var res = new ObservableCollection<UsageWeekLog>();

			for (int q = 0; q < 7; q++)
			{
				var date = today.AddDays(-q);
				var usages = Service.GetUsageLog(SelectedUser?.Login, date);

				var used = usages.Sum(_ => _.Used.ToDto()).ToTime();
				var weekUsage = new UsageWeekLog(date.DayOfWeek, used);

#if DEBUG
				if (RandomizeIfZero && weekUsage.Used == TimeSpan.Zero)
					weekUsage.Used = TimeSpan.FromMinutes(random.Next(20, 900)); // 15 hours * 60 minutes
#endif

				res.Insert(0, weekUsage);
			}
			return res;
		}

		DateTime date;
		public DateTime Date { get { return date; } set { if (Set(ref date, value)) ReloadUsageLogMethod(); } }

		public DateTime Today => DateTime.Today;

		public DataItems<User> Users { get; set; }

		public DataItems<UsageLog> UsageLog { get; set; }

		public DataItems<CurfewGui> Curfews { get; set; }

		/// <summary>
		/// Gets or sets the week usages.
		/// </summary>
		/// <value>
		/// The week usages.
		/// </value>
		public DataItems<UsageWeekLog> WeekUsages { get; set; }

		public bool IsNotAdmin { get { return !IsAdmin; } }
		public bool IsAdmin
		{
			get
			{
				isAdmin = isAdmin ?? new UserPrivilege().IsAdmin();
				return isAdmin.GetValueOrDefault();
			}
		}

		bool? isAdmin;

		TimeSpan _usedTotal;
		public TimeSpan UsedTotal
		{
			get { return _usedTotal; }
			set { Set(ref _usedTotal, value); }
		}

		CurfewGui _selectedCurfew;
		public CurfewGui SelectedCurfew { get { return _selectedCurfew; } set { Set(ref _selectedCurfew, value); } }

		public ICommand AddCurfew { get; protected set; }
		public void AddCurfewMethod()
		{
			var value = new CurfewGui();
			Curfews.Value.Add(value);
			SelectedCurfew = value;
		}

		public ICommand DelCurfew { get; protected set; }
		public void DelCurfewMethod()
		{
			Curfews.Value.Remove(SelectedCurfew);
			SelectedCurfew = null;
		}

		public ICommand SaveCurfews { get; protected set; }
		public void SaveCurfewsMethod()
		{
			var dtos = Curfews.Value.Select(_ => _.ToDto());
			Service.SaveCurfew(dtos.ToArray(), SelectedUser.Login);
		}

		public ICommand ReloadUsageLog { get; protected set; }
		public void ReloadUsageLogMethod()
		{
			UsageLog.Value = LoadUsageLog();
			WeekUsages.Value = LoadWeekUsage();
		}

		public void Dispose()
		{
			if (Service != null)
				Service.Dispose();
		}

		private User _user;
		private readonly IDataService Service;

		public User SelectedUser
		{
			get { return _user; }
			set
			{
				if (Set(ref _user, value))
				{
					Curfews.LoadAsync();
					UsageLog.LoadAsync();
					WeekUsages.LoadAsync();
				}
			}
		}
	}
}
