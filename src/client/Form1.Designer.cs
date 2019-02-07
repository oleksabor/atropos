namespace client
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.lUser = new System.Windows.Forms.ToolStripLabel();
			this.cbUsers = new System.Windows.Forms.ToolStripComboBox();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.bsUser = new System.Windows.Forms.BindingSource(this.components);
			this.bsCurfew = new System.Windows.Forms.BindingSource(this.components);
			this.gvCurfew = new System.Windows.Forms.DataGridView();
			this.colCFWeekDay = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colCFTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colCFBreak = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bsUser)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bsCurfew)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gvCurfew)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lUser,
            this.cbUsers});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(800, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// lUser
			// 
			this.lUser.Name = "lUser";
			this.lUser.Size = new System.Drawing.Size(30, 22);
			this.lUser.Text = "User";
			// 
			// cbUsers
			// 
			this.cbUsers.Name = "cbUsers";
			this.cbUsers.Size = new System.Drawing.Size(121, 25);
			// 
			// dataGridView1
			// 
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView1.Location = new System.Drawing.Point(0, 25);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.Size = new System.Drawing.Size(800, 425);
			this.dataGridView1.TabIndex = 1;
			// 
			// gvCurfew
			// 
			this.gvCurfew.AutoGenerateColumns = false;
			this.gvCurfew.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gvCurfew.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCFWeekDay,
            this.colCFTime,
            this.colCFBreak});
			this.gvCurfew.DataSource = this.bsCurfew;
			this.gvCurfew.Location = new System.Drawing.Point(8, 33);
			this.gvCurfew.Name = "gvCurfew";
			this.gvCurfew.Size = new System.Drawing.Size(780, 175);
			this.gvCurfew.TabIndex = 2;
			// 
			// colCFWeekDay
			// 
			this.colCFWeekDay.DataPropertyName = "WeekDay";
			this.colCFWeekDay.HeaderText = "week day";
			this.colCFWeekDay.Name = "colCFWeekDay";
			// 
			// colCFTime
			// 
			this.colCFTime.DataPropertyName = "Time";
			this.colCFTime.HeaderText = "time";
			this.colCFTime.Name = "colCFTime";
			// 
			// colCFBreak
			// 
			this.colCFBreak.DataPropertyName = "Break";
			this.colCFBreak.HeaderText = "break";
			this.colCFBreak.Name = "colCFBreak";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.gvCurfew);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.toolStrip1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bsUser)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bsCurfew)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gvCurfew)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.ToolStripLabel lUser;
		private System.Windows.Forms.ToolStripComboBox cbUsers;
		private System.Windows.Forms.BindingSource bsUser;
		private System.Windows.Forms.BindingSource bsCurfew;
		private System.Windows.Forms.DataGridView gvCurfew;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCFWeekDay;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCFTime;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCFBreak;
	}
}

