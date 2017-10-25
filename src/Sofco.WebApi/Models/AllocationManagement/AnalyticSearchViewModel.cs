﻿using Sofco.Model.Models.TimeManagement;
using System;

namespace Sofco.WebApi.Models.AllocationManagement
{
    public class AnalyticSearchViewModel
    {
        public AnalyticSearchViewModel(Analytic domain)
        {
            Id = domain.Id;
            Title = domain.Title;
            Name = domain.Name;
            ClientExternalName = domain.ClientExternalName;
            CommercialManager = domain.CommercialManager;
            StartDate = domain.StartDateContract;
            EndDate = domain.EndDateContract;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Title { get; private set; }
        public string ClientExternalName { get; private set; }
        public string CommercialManager { get; private set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
