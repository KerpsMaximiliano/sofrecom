﻿using System;
using System.Collections.Generic;

namespace Sofco.Core.Models.Reports
{
    public class PurchaseOrderBalanceViewModel
    {
        public int Id { get; set; }

        public int PurchaseOrderId { get; set; }

        public string Number { get; set; }

        public string Title { get; set; }

        public string Proposal { get; set; }

        public string ClientExternalName { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyText { get; set; }

        public decimal Ammount { get; set; }

        public decimal Adjustment { get; set; }

        public DateTime AdjustmentDate { get; set; }

        public int StatusId { get; set; }

        public string StatusText { get; set; }

        public decimal Balance { get; set; }

        public DateTime ReceptionDate { get; set; }

        public string AccountManagerNames { get; set; }

        public string ProjectManagerNames { get; set; }

        public string ManagerNames { get; set; }

        public int? FileId { get; set; }

        public string FileName { get; set; }

        public string PdfUrl { get; set; }

        public List<PurchaseOrderBalanceDetailViewModel> Details { get; set; }
    }
}
