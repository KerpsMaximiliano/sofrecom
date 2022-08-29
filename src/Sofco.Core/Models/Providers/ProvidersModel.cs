using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.Providers
{
    public class ProvidersModel
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int? ProviderAreaId { get; set; }
        public int? UserApplicantId { get; set; }
        public int? Score { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Active { get; set; }
        public string CUIT { get; set; }
        public int? IngresosBrutos { get; set; }
        public int? CondicionIVA { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZIPCode { get; set; }
        public string Province { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string Comments { get; set; }
        public string Country { get; set; }

        public ProvidersModel(Sofco.Domain.Models.Providers.Providers providers)
        {
            /*
             Id = providers.Id;
            Name = providers.Name;
            ProviderAreaId = providers.ProviderAreaId;
            UserApplicantId = providers.UserEvaluatorId;
            Score = providers.Score;
            StartDate = providers.StartDate;
            Active = providers.Active;
            CUIT = providers.CUIT;
            IngresosBrutos = providers.IngresosBrutos;
            CondicionIVA = providers.CondicionIVA;
            Address = providers.Address;
            City = providers.City;
            ZIPCode = providers.ZIPCode;
            Province = providers.Province;
            ContactName = providers.ContactName;
            Phone = providers.Phone;
            Email = providers.Email;
            WebSite = providers.WebSite;
            Comments = providers.Comments;
             */


        }

    }

    public class ProvidersModelGet
    {

        public int? Id { get; set; }
        public string Name { get; set; }
        public int? ProviderAreaId { get; set; }
        public int? UserApplicantId { get; set; }
        public int? Score { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Active { get; set; }
        public string CUIT { get; set; }
        public int? IngresosBrutos { get; set; }
        public int? CondicionIVA { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZIPCode { get; set; }
        public string Province { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string Comments { get; set; }
        public string Country { get; set; }
        public ProvidersModelGet(Sofco.Domain.Models.Providers.Providers providers)
        {
            Id = providers.Id;
            Name = providers.Name;
            ProviderAreaId = providers.ProviderAreaId;
            UserApplicantId = providers.UserEvaluatorId;
            Score = providers.Score;
            StartDate = providers.StartDate;
            Active = providers.Active;
            CUIT = providers.CUIT;
            IngresosBrutos = providers.IngresosBrutos;
            CondicionIVA = providers.CondicionIVA;
            Address = providers.Address;
            City = providers.City;
            ZIPCode = providers.ZIPCode;
            Province = providers.Province;
            ContactName = providers.ContactName;
            Phone = providers.Phone;
            Email = providers.Email;
            WebSite = providers.WebSite;
            Comments = providers.Comments;
            Country = providers.Country;


        }
        
    }
    



    /*
     * 
     * public ProvidersModel()
        {

    }

     public ProvidersModel(Sofco.Domain.Models.Providers.Providers providers)
    {
        Id = providers.Id;
        Name = providers.Name;
        ProviderAreaId = providers.ProviderAreaId;
        UserApplicantId = providers.UserEvaluatorId;
        Score = providers.Score;
        StartDate = providers.StartDate;
        Active = providers.Active;
        CUIT = providers.CUIT;
        IngresosBrutos = providers.IngresosBrutos;
        CondicionIVA = providers.CondicionIVA;
        Address = providers.Address;
        City = providers.City;
        ZIPCode = providers.ZIPCode;
        Province = providers.Province;
        ContactName = providers.ContactName;
        Phone = providers.Phone;
        Email = providers.Email;
        WebSite = providers.WebSite;
        Comments = providers.Comments;


    }
    public int Id { get; set; }
    public string Name { get; set; }
    public int ProviderAreaId { get; set; }
    public int UserApplicantId { get; set; }
    public int Score { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool Active { get; set; }
    public string CUIT { get; set; }
    public int IngresosBrutos { get; set; }
    public int CondicionIVA { get; set; }
    public string Address { get; set; }
    public string City  { get; set; }
    public string ZIPCode { get; set; }
    public string Province { get; set; }
    public string ContactName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string WebSite { get; set; }
    public string Comments { get; set; }
     */



}



