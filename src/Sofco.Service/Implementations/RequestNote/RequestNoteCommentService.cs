using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Common;
using Sofco.Core.Logger;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Core.Services.RequestNote;
using Sofco.Core.Models.RequestNote;
using Sofco.Core.Models.Recruitment;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Sofco.Data.Admin;
using Sofco.Domain.Models.RequestNote;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteCommentService : IRequestNoteCommentService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<RequestNoteAnalitycService> logger;
        private readonly IUserData userData;



        public RequestNoteCommentService(IUnitOfWork unitOfWork, ILogMailer<RequestNoteAnalitycService> logger, IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;   
            this.userData = userData;
        }


        public Response<int> Add(Comments requestNoteComment)
        {
            var response = new Response<int>();

            if (requestNoteComment == null)
            {
                response.AddError(Resources.Recruitment.Applicant.ModelNull);
                return response;
            }
            try
            {
                var domain = this.CreateDomain(requestNoteComment);

                unitOfWork.RequestNoteCommentRepository.Add(domain);
                unitOfWork.Save();

                response.Data = domain.Id;
                response.AddSuccess(Resources.RequestNote.RequestNote.CommentSaved);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<int> Delete(int id)
        {
            var response = new Response<int>();
            var entity = unitOfWork.RequestNoteCommentRepository.GetById(id);

            try
            {
                this.ValidateDelete(entity);
                unitOfWork.RequestNoteCommentRepository.Delete(entity);
                unitOfWork.Save();
                response.AddSuccess(Resources.RequestNote.RequestNote.CommentDeleted);
                return response;
            }
            catch(Exception ex)
            {
                response.AddError(ex.Message);
                return response;
            }   
        }

        private void ValidateDelete(RequestNoteComment entity)
        {
            if (entity == null)
                throw new ArgumentNullException(Resources.RequestNote.RequestNote.NotFound);
           
            this.CheckUser(entity.UserName);
        }

        private void CheckUser(string userName)
        {
            if (userName != userData.GetCurrentUser().UserName)
                 throw new Exception(Resources.RequestNote.RequestNote.NotFound);
        }

        public IList<Comments> GetByRequestNoteId(int id)
        {
            var coments = this.unitOfWork.RequestNoteCommentRepository.GetByRequestNoteId(id);
            return coments.Select(h => new Comments()
            {
                Comment = h.Comment,
                UserName = h.UserName,
                RequestNoteId = h.RequestNoteId,
                Id = h.Id,
                Date =  h.Date
               
            }).ToList();

        }


        private RequestNoteComment CreateDomain(Comments comment)
        {
            var commentDomain = new RequestNoteComment();
            commentDomain.UserName = userData.GetCurrentUser().UserName;
            commentDomain.RequestNoteId = comment.RequestNoteId;
            commentDomain.Date = DateTime.UtcNow;
            commentDomain.Comment = comment.Comment;
            
            return commentDomain;
        }
    }
}
