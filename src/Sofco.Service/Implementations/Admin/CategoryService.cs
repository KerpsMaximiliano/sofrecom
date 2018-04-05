using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models;
using Sofco.Core.Models.Admin;
using Sofco.Core.Services.Admin;
using Sofco.Model.Models.Admin;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Admin
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<CategoryService> logger;

        public CategoryService(IUnitOfWork unitOfWork, ILogMailer<CategoryService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public Response Add(string description)
        {
            var response = new Response();

            Validate(description, response);

            if (response.HasErrors()) return response;

            try
            {
                var category = new Category
                {
                    Description = description,
                    Active = true,
                    StartDate = DateTime.UtcNow
                };

                unitOfWork.CategoryRepository.Insert(category);
                unitOfWork.Save();

                response.AddSuccess(Resources.Admin.Category.Created);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public IList<Category> GetAll(bool active)
        {
            var list = active ? unitOfWork.CategoryRepository.GetAllActives().ToList() : unitOfWork.CategoryRepository.GetAllReadOnly().ToList();

            return list;
        }

        public Response<CategoryModel> GetById(int id)
        {
            var response = new Response<CategoryModel>();
            var category = unitOfWork.CategoryRepository.GetById(id);

            if (category != null)
            {
                response.Data = MapCategory(category);
                return response;
            }

            response.AddError(Resources.Admin.Category.NotFound);
            return response;
        }

        public Response<Category> Active(int id, bool active)
        {
            var response = new Response<Category>();
            var entity = unitOfWork.CategoryRepository.GetSingle(x => x.Id == id);

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

                unitOfWork.CategoryRepository.Update(entity);
                unitOfWork.Save();

                response.Data = entity;
                response.AddSuccess(active ? Resources.Admin.Category.Enabled : Resources.Admin.Category.Disabled);
                return response;
            }

            response.AddError(Resources.Admin.Category.NotFound);
            return response;
        }

        public Response Update(CategoryModel model)
        {
            var response = new Response();
            var entity = unitOfWork.CategoryRepository.GetSingle(x => x.Id == model.Id);

            if (entity == null)
            {
                response.AddError(Resources.Admin.Category.NotFound);
                return response;
            }

            Validate(model.Description, response);

            if (response.HasErrors()) return response;

            try
            {
                entity.Description = model.Description;

                unitOfWork.CategoryRepository.Update(entity);
                unitOfWork.Save();
                response.AddSuccess(Resources.Admin.Category.Updated);
            }
            catch (Exception)
            {
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private CategoryModel MapCategory(Category category)
        {
            var model = new CategoryModel
            {
                Id = category.Id,
                Description = category.Description,
                Tasks = category.Tasks.Select(x => new TaskModel {Id = x.Id, Description = x.Description}).ToList()
            };

            return model;
        }

        private void Validate(string description, Response response)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                response.AddError(Resources.Admin.Category.DescriptionRequired);
            }

            if (unitOfWork.CategoryRepository.DescriptionExist(description))
            {
                response.AddError(Resources.Admin.Category.DescriptionAlreadyExist);
            }
        }
    }
}
