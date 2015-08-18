using System.Web.Mvc;
using System.Web.Routing;

namespace BuildVersioningManager
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute("AddProject", "projects/add", new { controller = "Project", action = "AddProject" });
			routes.MapRoute("DeleteProject", "projects/delete/{name}", new { controller = "Project", action = "PrepareToDeleteProject" });
			routes.MapRoute("EditProject", "projects/edit/{name}", new { controller = "Project", action = "EditProject" });
			routes.MapRoute("ListProjects", "projects/list", new { controller = "Project", action = "ListProjects" });
			routes.MapRoute("ViewProject", "projects/view/{name}", new { controller = "Project", action = "ViewProject" });

			routes.MapRoute("AddProjectConfig", "projects/{projectName}/projectconfigs/add", new { controller = "ProjectConfig", action = "AddProjectConfig" });
			routes.MapRoute("DeleteProjectConfig", "projects/{projectName}/projectconfigs/delete/{name}", new { controller = "ProjectConfig", action = "PrepareToDeleteProjectConfig" });
			routes.MapRoute("EditProjectConfig", "projects/{projectName}/projectconfigs/edit/{name}", new { controller = "ProjectConfig", action = "EditProjectConfig" });
			routes.MapRoute("ListProjectConfigsByProject", "projects/{projectName}/projectconfigs/list", new { controller = "ProjectConfig", action = "ListProjectConfigsByProject" });
			routes.MapRoute("ViewProjectConfig", "projects/{projectName}/projectconfigs/view/{name}", new { controller = "ProjectConfig", action = "ViewProjectConfig" });

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
