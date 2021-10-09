using Hangfire;
using Hangfire.Common;
using Hangfire.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HangfireWithDotNet
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HangfireAspNet.Use(GetHangfireServers);

                // Let's also create a sample background job
                var manager = new RecurringJobManager();
                manager.AddOrUpdate("firstJob", Job.FromExpression(() => FireJob()), Cron.Minutely());
            }
        }
        private IEnumerable<IDisposable> GetHangfireServers()
        {
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage("Data Source=pixcilelab;Initial Catalog=TestingDatabase;Integrated Security=True", new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                });

            yield return new BackgroundJobServer();
        }
        private void FireJob()
        {
            using (DataClasses1DataContext eDataBase = new DataClasses1DataContext())
            {
                User eUser = new User();
                eUser.UserName = "A" + DateTime.UtcNow.ToString();
                eUser.AccountStatus = true;
                eUser.IsActive = true;
                eUser.IsDelete = false;
                eUser.LastUpdated = DateTime.Now;
                eDataBase.Users.InsertOnSubmit(eUser);
                eDataBase.SubmitChanges();
            }
        }
    }
}