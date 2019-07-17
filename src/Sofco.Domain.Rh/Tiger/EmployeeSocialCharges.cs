using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Rh.Tiger
{
    public class EmployeeSocialCharges
    {
        public string EmployeeNumber { get; set; }

        public int AccountNumber { get; set; }

        public string AccountName { get; set; }

        public decimal Value { get; set; }
    }
}
