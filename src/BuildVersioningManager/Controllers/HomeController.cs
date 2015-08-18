using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BuildVersioning.Data.Providers.EFProvider;
using BuildVersioningManager.Models.HomeModels;

namespace BuildVersioningManager.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			List<RecentActivityModel> recentActivities;

			using (var db = new BuildVersioningDataContext())
			{
				recentActivities =
					db.VersionHistoryItems
						.OrderByDescending(item => item.Date)
						.Take(10)
						.Select(item =>
							new RecentActivityModel
							{
								Date = item.Date,
								ProjectConfigId = item.ProjectConfigId,
								ProjectConfigName = item.ProjectConfigName,
								ProjectId = item.ProjectId,
								ProjectName = item.ProjectName,
								Version = item.Version
							})
						.ToList();
			}

			var model = new IndexModel { RecentActivities = recentActivities };
			return View(model);
		}

		public ActionResult About()
		{
			return View();
		}

		public ActionResult Contact()
		{
			return View();
		}
	}
}