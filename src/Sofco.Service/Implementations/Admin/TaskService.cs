using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Admin;
using Sofco.Core.Services.Admin;
using Sofco.Model.Models.Admin;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Admin
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<CategoryService> logger;

        public TaskService(IUnitOfWork unitOfWork, ILogMailer<CategoryService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public Response Add(TaskModel model)
        {
            var response = new Response();

            Validate(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var task = new Task
                {
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    Active = true,
                    StartDate = DateTime.UtcNow
                };

                unitOfWork.TaskRepository.Insert(task);
                unitOfWork.Save();

                response.AddSuccess(Resources.Admin.Task.Created);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public IList<Task> GetAll(bool active)
        {
            var list = active ? unitOfWork.TaskRepository.GetAllActives().ToList() : unitOfWork.TaskRepository.GetAllReadOnly().ToList();

            return list;
        }

        public Response<TaskModel> GetById(int id)
        {
            var response = new Response<TaskModel>();
            var task = unitOfWork.TaskRepository.GetById(id);

            if (task != null)
            {
                response.Data = MapTask(task);
                return response;
            }

            response.AddError(Resources.Admin.Task.NotFound);
            return response;
        }

        public Response<Task> Active(int id, bool active)
        {
            var response = new Response<Task>();
            var entity = unitOfWork.TaskRepository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.Active = active;

                if (active)
                {
                    entity.StartDate = DateTime.UtcNow;
                    entity.EndDate = null;
                }
                else
                {
                    entity.EndDate = DateTime.UtcNow;
                }

                unitOfWork.TaskRepository.Update(entity);
                unitOfWork.Save();

                response.Data = entity;
                response.AddSuccess(active ? Resources.Admin.Task.Enabled : Resources.Admin.Task.Disabled);
                return response;
            }

            response.AddError(Resources.Admin.Category.NotFound);
            return response;
        }

        public Response Update(TaskModel model)
        {
            var response = new Response();
            var entity = unitOfWork.TaskRepository.GetSingle(x => x.Id == model.Id);

            if (entity == null)
            {
                response.AddError(Resources.Admin.Task.NotFound);
                return response;
            }

            Validate(model, response);

            if (response.HasErrors()) return response;

            try
            {
                entity.Description = model.Description;
                entity.CategoryId = model.CategoryId;

                unitOfWork.TaskRepository.Update(entity);
                unitOfWork.Save();
                response.AddSuccess(Resources.Admin.Task.Updated);
            }
            catch (Exception)
            {
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void Validate(TaskModel model, Response response)
        {
            if (string.IsNullOrWhiteSpace(model.Description))
            {
                response.AddError(Resources.Admin.Task.DescriptionRequired);
            }

            if (model.CategoryId <= 0)
            {
                response.AddError(Resources.Admin.Task.CategoryRequired);
            }
        }

        private TaskModel MapTask(Task task)
        {
            var model = new TaskModel
            {
                Id = task.Id,
                Description = task.Description,
                CategoryId = task.CategoryId
            };

            return model;
        }
    }
}
