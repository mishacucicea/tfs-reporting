//------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using TfsConnector;

namespace TfsReportingClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var tasksRds = new ReportDataSource("Tasks", GetTfsTasks());
            this.reportViewer1.LocalReport.DataSources.Add(tasksRds);
            this.reportViewer1.RefreshReport();
        }

        private static IEnumerable<TfsItem> GetRandomTasks()
        {
            var randomizer = new Random(100);
            var tasks = new List<TfsItem>();

            for (int i = 0; i < 10; i++)
            {
                int random = randomizer.Next(10000);
                var task = new TfsItem()
                {
                    Id = random,
                    Title = "Task " + random,
                    //OriginalEstimate = random * 2
                };
                tasks.Add(task);
            }

            return tasks;
        }

        private static IEnumerable<TfsItem> GetTfsTasks()
        {
            TfsProjects tp = new TfsProjects(new TfsConfigurationContext());
            TfsQueries query = new TfsQueries(tp);
            var tasks = query.ExecuteQuery(new Guid("528b8489-8649-4324-bc66-cbf8d1ea84a7"));

            return tasks;

        }
    }
}