using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using BuildVersioning.Data.Providers.EFProvider;
using BuildVersioning.Entities;
using BuildVersioningManager.Models.ProjectModels;

namespace BuildVersioningManager.Controllers
{
	public class ProjectController : Controller
	{
		[HttpGet]
		public ActionResult AddProject()
		{
			return View();
		}

		[HttpPost]
		public ActionResult AddProject(AddProjectModel addProject)
		{
			if (null == addProject)
				return View("Error");

			var name = addProject.Name;

			if (string.IsNullOrWhiteSpace(name))
				return View("Error");

			var desc = addProject.Description;
			if (string.IsNullOrWhiteSpace(desc))
				desc = null;

			using (var db = new BuildVersioningDataContext())
			{
				if (db.Projects.Any(p => string.Compare(p.Name, name, StringComparison.OrdinalIgnoreCase) == 0))
					return View("Error"); // <-- A project with the same name already exists.

				var project =
					new Project
					{
						Name = name,
						Description = desc,
						BuildNumber = 0,
						DateBuildNumberUpdated = DateTime.Now
					};

				db.Projects.Add(project);
				db.SaveChanges();
			}

			return RedirectToRoute("ListProjects");
		}

		[HttpGet]
		public ActionResult PrepareToDeleteProject(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				return View("Error", new ErrorInfo("Project", "PrepareToDeleteProject", "The project name is null, empty or contains only whitespace which is not allowed."));

			Project project;

			using (var db = new BuildVersioningDataContext())
			{
				project = db.Projects
					.Include(p => p.ProjectConfigs)
					.SingleOrDefault(p =>
						string.Compare(p.Name, name, StringComparison.OrdinalIgnoreCase) == 0
					);
			}

			if (null == project)
				return RedirectToRoute("ListProjects");

			var model = new PrepareToDeleteProjectModel { Project = project };
			return View(model);
		}

		[HttpPost]
		public ActionResult DeleteProject(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				return View("Error", new ErrorInfo("Project", "DeleteProject", "The project name is null, empty or contains only whitespace which is not allowed."));

			using (var db = new BuildVersioningDataContext())
			{
				var project = db.Projects
					.Include(p => p.ProjectConfigs)
					.SingleOrDefault(p =>
						string.Compare(p.Name, name, StringComparison.OrdinalIgnoreCase) == 0
						);

				if (null == project)
					return RedirectToRoute("ListProjects");

				var projectConfigIds = project.ProjectConfigs.Select(c => c.Id).ToList();

				// Delete all child project configs.
				foreach (var projectConfigId in projectConfigIds)
				{
					var projectConfig = db.ProjectConfigs.SingleOrDefault(c => c.Id == projectConfigId);
					if (null != projectConfig)
						db.ProjectConfigs.Remove(projectConfig);
				}

				// Delete the project.
				db.Projects.Remove(project);
				db.SaveChanges();
			}

			return RedirectToRoute("ListProjects");
		}

		[HttpGet]
		public ActionResult EditProject(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				return View("Error", new ErrorInfo("Project", "EditProject", "The project name is null, empty or contains only whitespace which is not allowed."));

			Project project;

			using (var db = new BuildVersioningDataContext())
			{
				project = db.Projects
					.SingleOrDefault(p =>
						string.Compare(p.Name, name, StringComparison.OrdinalIgnoreCase) == 0
						);
			}

			if (null == project)
				return View("Error");

			var model =
				new EditProjectModel
				{
					BuildNumber = project.BuildNumber,
					Description = project.Description,
					Id = project.Id,
					Name = project.Name
				};
			return View(model);
		}

		[HttpPost]
		public ActionResult EditProject(string name, EditProjectModel editProject)
		{
			if (null == editProject)
				return View("Error", new ErrorInfo("Project", "EditProject", "The editProject model is null which is not allowed."));

			var id = editProject.Id;
			if (id < 1)
				return View("Error", new ErrorInfo("Project", "EditProject", "The project id is not valid."));

			var newName = editProject.Name;
			if (string.IsNullOrWhiteSpace(newName))
				return View("Error", new ErrorInfo("Project", "EditProject", "The new project name is null, empty or contains only whitespace which is not allowed."));

			var desc = editProject.Description;
			if (string.IsNullOrWhiteSpace(desc))
				desc = null;

			var buildNumber = editProject.BuildNumber;
			if (buildNumber < 0)
				buildNumber = 0;

			using (var db = new BuildVersioningDataContext())
			{
				if (name != newName && db.Projects.Any(p => string.Compare(p.Name, newName, StringComparison.OrdinalIgnoreCase) == 0))
					return View("Error", new ErrorInfo("Project", "EditProject", "A project with the specified new name already exists."));

				var project = db.Projects.SingleOrDefault(p => p.Id == id);
				if (null == project)
					return View("Error", new ErrorInfo("Project", "EditProject", "The specified project id of the project to be edited does not exist."));

				project.Name = newName;
				project.Description = desc;

				if (project.BuildNumber != buildNumber)
				{
					project.BuildNumber = buildNumber;
					project.DateBuildNumberUpdated = DateTime.Now;
				}

				db.SaveChanges();
			}

			return RedirectToRoute("ViewProject", new { name });
		}

		public ActionResult ViewProject(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				return View("Error", new ErrorInfo("Project", "ViewProject", "The project name is null, empty or contains only whitespace which is not allowed."));

			Project project;

			using (var db = new BuildVersioningDataContext())
			{
				project = db.Projects
					.Include(p => p.ProjectConfigs)
					.SingleOrDefault(p =>
						string.Compare(p.Name, name, StringComparison.OrdinalIgnoreCase) == 0
						);
			}

			if (null == project)
				return View("Error");

			var model = new ProjectModel { Project = project };
			return View(model);
		}

		public ActionResult ListProjects()
		{
			List<Project> projects;

			using (var db = new BuildVersioningDataContext())
			{
				projects = db.Projects.OrderBy(p => p.Name).ToList();
			}

			var model = new ProjectsModel { Projects = projects };
			return View(model);
		}
	}
}