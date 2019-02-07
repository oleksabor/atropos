using Atropos.Common;
using Atropos.Common.Dto;
using client.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		public Form1(DataLoader service)
			: this()
		{
			Service = service;

			bsUser.DataSource = Service.Users.Value;
			cbUsers.ComboBox.DataSource = bsUser;
			cbUsers.ComboBox.AddBinding(c => c.SelectedItem, Service, s => s.SelectedUser);

			bsCurfew.DataSource = Service.Curfews.Value;


			//AddBinding(txtGeminiUrl, c => c.Text, _settings, s => s.GeminiURL);

			Service.Users.Load();
		}

		public DataLoader Service { get; }
	}
}
