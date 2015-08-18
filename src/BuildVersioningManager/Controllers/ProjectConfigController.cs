using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using BuildVersioning.Data.Providers.EFProvider;
using BuildVersioning.Entities;
using BuildVersioningManager.Models.ProjectConfigModels;

namespace BuildVersioningManager.Controllers
{
	public class ProjectConfigController : Controller
	{
		[HttpGet]
		public ActionResult AddProjectConfig(string projectName)
		{
			if (string.IsNullOrWhiteSpace(projectName))
				return View("Error");

			Project project;

			using (var db = new BuildVersioningDataContext())
			{
				project =
					db.Projects
						.SingleOrDefault(p =>
							string.Compare(p.Name, projectName, StringComparison.OrdinalIgnoreCase) == 0
							);
			}

			if (null == project)
				return View("Error");

			var model =
				new AddProjectConfigModel
				{
					ParentProjectId = project.Id,
					ParentProjectName = project.Name
				};
			return View(model);
		}

		[HttpPost]
		public ActionResult AddProjectConfig(AddProjectConfigModel addProjectConfig)
		{
			if (null == addProjectConfig)
				return View("Error");

			// Validate ParentProjectId
			var parentProjectId = addProjectConfig.ParentProjectId;
			if (parentProjectId < 1)
				return View("Error");

			// Validate ParentProjectName
			var parentProjectName = addProjectConfig.ParentProjectName;
			if (string.IsNullOrWhiteSpace(parentProjectName))
				return View("Error");

			// Validate Name
			var name = addProjectConfig.Name;
			if (string.IsNullOrWhiteSpace(name))
				return View("Error");

			var desc = addProjectConfig.Description;
			if (string.IsNullOrWhiteSpace(desc))
				desc = null;

			var buildNumberPostion = (int)addProjectConfig.BuildNumberPosition;
			if (buildNumberPostion < 3 || buildNumberPostion > 4)
				return View("Error");

			// Force the GeneratedVersion part that corresponds to the build number position to a value of zero.
			switch (buildNumberPostion)
			{
				case 3:
					addProjectConfig.GeneratedVersionPart3 = 0;
					break;
				case 4:
					addProjectConfig.GeneratedVersionPart4 = 0;
					break;
			}

			using (var db = new BuildVersioningDataContext())
			{
				if (db.ProjectConfigs
					.Include(c => c.Project)
					.Any(c =>
						c.Project.Id == parentProjectId &&
						string.Compare(c.Name, name, StringComparison.OrdinalIgnoreCase) == 0
						))
					return View("Error"); // <-- A ProjectConfig with the same parent Project and the same name already exists.

				var projectConfig =
					new ProjectConfig
					{
						Description = desc,
						GeneratedBuildNumberPosition = buildNumberPostion,
						GeneratedVersionPart1 = addProjectConfig.GeneratedVersionPart1,
						GeneratedVersionPart2 = addProjectConfig.GeneratedVersionPart2,
						GeneratedVersionPart3 = addProjectConfig.GeneratedVersionPart3,
						GeneratedVersionPart4 = addProjectConfig.GeneratedVersionPart4,
						Name = name,
						ProductVersionPart1 = addProjectConfig.ProductVersionPart1,
						ProductVersionPart2 = addProjectConfig.ProductVersionPart2,
						ProductVersionPart3 = addProjectConfig.ProductVersionPart3,
						ProductVersionPart4 = addProjectConfig.ProductVersionPart4,
						ProjectId = parentProjectId,
						ReleaseType = addProjectConfig.ReleaseType
					};

				db.ProjectConfigs.Add(projectConfig);
				db.SaveChanges();
			}

			return RedirectToRoute("ViewProject", new { name = parentProjectName });
		}

		[HttpGet]
		public ActionResult PrepareToDeleteProjectConfig(string projectName, string name)
		{
			if (string.IsNullOrWhiteSpace(projectName))
				return View("Error");

			if (string.IsNullOrWhiteSpace(name))
				return View("Error");

			ProjectConfig projectConfig;

			using (var db = new BuildVersioningDataContext())
			{
				projectConfig =
					db.ProjectConfigs
						.Include(c => c.Project)
						.SingleOrDefault(c =>
							string.Compare(c.Project.Name, projectName, StringComparison.OrdinalIgnoreCase) == 0 &&
							string.Compare(c.Name, name, StringComparison.OrdinalIgnoreCase) == 0
							);
			}

			if (null == projectConfig)
				return RedirectToRoute("ViewProject", new { name = projectName });

			var model = new PrepareToDeleteProjectConfigModel { ProjectConfig = projectConfig };
			return View(model);
		}

		[HttpPost]
		public ActionResult DeleteProjectConfig(string projectName, string name)
		{
			if (string.IsNullOrWhiteSpace(projectName))
				return View("Error");

			if (string.IsNullOrWhiteSpace(name))
				return View("Error");

			using (var db = new BuildVersioningDataContext())
			{
				var projectConfig = db.ProjectConfigs
					.Include(c => c.Project)
					.SingleOrDefault(c =>
						string.Compare(c.Project.Name, projectName, StringComparison.OrdinalIgnoreCase) == 0 &&
						string.Compare(c.Name, name, StringComparison.OrdinalIgnoreCase) == 0
					);

				if (null == projectConfig)
					return RedirectToRoute("ViewProject", new { name = projectName });

				db.ProjectConfigs.Remove(projectConfig);
				db.SaveChanges();
			}

			return RedirectToRoute("ViewProject", new { name = projectName });
		}

		[HttpGet]
		public ActionResult EditProjectConfig(string projectName, string name)
		{
			if (string.IsNullOrWhiteSpace(projectName))
				return View("Error");

			if (string.IsNullOrWhiteSpace(name))
				return View("Error");

			ProjectConfig projectConfig;

			using (var db = new BuildVersioningDataContext())
			{
				projectConfig =
					db.ProjectConfigs
						.Include(c => c.Project)
						.SingleOrDefault(c =>
							string.Compare(c.Project.Name, projectName, StringComparison.OrdinalIgnoreCase) == 0 &&
							string.Compare(c.Name, name, StringComparison.OrdinalIgnoreCase) == 0
							);
			}

			if (null == projectConfig)
				return View("Error");

			var generatedBuildNumberPosition = projectConfig.GeneratedBuildNumberPosition;

			BuildNumberPositions buildNumberPosition;
			switch (generatedBuildNumberPosition)
			{
				case 3:
					buildNumberPosition = BuildNumberPositions.Three;
					break;

				case 4:
					buildNumberPosition = BuildNumberPositions.Four;
					break;

				default:
					return View("Error");
			}

			var model =
				new EditProjectConfigModel
				{
					BuildNumberPosition = buildNumberPosition,
					Description = projectConfig.Description,
					GeneratedBuildNumberPosition = projectConfig.GeneratedBuildNumberPosition,
					GeneratedVersionPart1 = projectConfig.GeneratedVersionPart1,
					GeneratedVersionPart2 = projectConfig.GeneratedVersionPart2,
					GeneratedVersionPart3 = projectConfig.GeneratedVersionPart3,
					GeneratedVersionPart4 = projectConfig.GeneratedVersionPart4,
					Id = projectConfig.Id,
					Name = name,
					OriginalName = name,
					ProductVersionPart1 = projectConfig.ProductVersionPart1,
					ProductVersionPart2 = projectConfig.ProductVersionPart2,
					ProductVersionPart3 = projectConfig.ProductVersionPart3,
					ProductVersionPart4 = projectConfig.ProductVersionPart4,
					ParentProjectId = projectConfig.Project.Id,
					ParentProjectName = projectConfig.Project.Name,
					ReleaseType = projectConfig.ReleaseType
				};
			return View(model);
		}

		[HttpPost]
		public ActionResult EditProjectConfig(string projectName, string name, EditProjectConfigModel editProjectConfig)
		{
			if (null == editProjectConfig)
				return View("Error");

			// Validate id (the ProjectConfig.Id to be edited)
			var id = editProjectConfig.Id;
			if (id < 1)
				return View("Error");

			// Validate ParentProjectId
			var parentProjectId = editProjectConfig.ParentProjectId;
			if (parentProjectId < 1)
				return View("Error");

			// Validate ParentProjectName
			var parentProjectName = editProjectConfig.ParentProjectName;
			if (string.IsNullOrWhiteSpace(parentProjectName))
				return View("Error");

			// Validate Name
			var newName = editProjectConfig.Name;
			if (string.IsNullOrWhiteSpace(name))
				return View("Error");

			var desc = editProjectConfig.Description;
			if (string.IsNullOrWhiteSpace(desc))
				desc = null;

			var buildNumberPostion = (int)editProjectConfig.BuildNumberPosition;
			if (buildNumberPostion < 3 || buildNumberPostion > 4)
				return View("Error");

			// Force the GeneratedVersion part that corresponds to the build number position to a value of zero.
			switch (buildNumberPostion)
			{
				case 3:
					editProjectConfig.GeneratedVersionPart3 = 0;
					break;
				case 4:
					editProjectConfig.GeneratedVersionPart4 = 0;
					break;
			}

			using (var db = new BuildVersioningDataContext())
			{
				if (name != newName && db.ProjectConfigs
					.Include(c => c.Project)
					.Any(c =>
						c.Project.Id == parentProjectId &&
						string.Compare(c.Name, newName, StringComparison.OrdinalIgnoreCase) == 0
						))
					return View("Error"); // <-- A ProjectConfig with the same parent Project and the same name already exists.

				var projectConfig =
					db.ProjectConfigs
						.Include(c => c.Project)
						.SingleOrDefault(c =>
							c.Project.Id == parentProjectId &&
							c.Id == id
							);

				if (null == projectConfig)
					return View("Error");

				projectConfig.Description = desc;
				projectConfig.GeneratedBuildNumberPosition = buildNumberPostion;
				projectConfig.GeneratedVersionPart1 = editProjectConfig.GeneratedVersionPart1;
				projectConfig.GeneratedVersionPart2 = editProjectConfig.GeneratedVersionPart2;
				projectConfig.GeneratedVersionPart3 = editProjectConfig.GeneratedVersionPart3;
				projectConfig.GeneratedVersionPart4 = editProjectConfig.GeneratedVersionPart4;
				projectConfig.Name = newName;
				projectConfig.ProductVersionPart1 = editProjectConfig.ProductVersionPart1;
				projectConfig.ProductVersionPart2 = editProjectConfig.ProductVersionPart2;
				projectConfig.ProductVersionPart3 = editProjectConfig.ProductVersionPart3;
				projectConfig.ProductVersionPart4 = editProjectConfig.ProductVersionPart4;
				projectConfig.ReleaseType = editProjectConfig.ReleaseType;

				db.SaveChanges();
			}

			return RedirectToRoute("ViewProjectConfig", new { projectName = parentProjectName, name });
		}

		public ActionResult ViewProjectConfig(string projectName, string name)
		{
			ProjectConfig projectConfig;

			using (var db = new BuildVersioningDataContext())
			{
				projectConfig = db.ProjectConfigs
					.Include(c => c.Project)
					.SingleOrDefault(c =>
						string.Compare(c.Project.Name, projectName, StringComparison.OrdinalIgnoreCase) == 0 &&
						string.Compare(c.Name, name, StringComparison.OrdinalIgnoreCase) == 0
						);
			}

			if (null == projectConfig)
				return View("Error");

			var model = new ProjectConfigModel { ProjectConfig = projectConfig };
			return View(model);
		}

		public ActionResult ListProjectConfigsByProject(string projectName)
		{
			List<ProjectConfig> projectConfigs;

			using (var db = new BuildVersioningDataContext())
			{
				projectConfigs = db.ProjectConfigs
					.Include(c => c.Project)
					.Where(c =>
						string.Compare(c.Project.Name, projectName, StringComparison.OrdinalIgnoreCase) == 0
						)
					.OrderBy(c => c.Name).ToList();
			}

			var model = new ProjectConfigsModel { ProjectConfigs = projectConfigs };
			return View(model);
		}
	}
}