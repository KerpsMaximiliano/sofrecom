﻿using System;
using System.Collections.Generic;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Domain.Models.Workflow
{
    public class Workflow : BaseEntity, IAudit
    {
        public string Description { get; set; }

        public int Version { get; set; }

        public bool Active { get; set; }

        public int WorkflowTypeId { get; set; }
        public WorkflowType WorkflowType { get; set; }

        public IList<WorkflowStateTransition> Transitions { get; set; }

        public IList<WorkflowReadAccess> WorkflowReadAccesses { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }

        public int CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public int ModifiedById { get; set; }

        public User ModifiedBy { get; set; }

        public IList<Advancement> Advancements { get; set; }

        public IList<Refund> Refunds { get; set; }

        public IList<RequestNote.RequestNote> RequestNotes { get; set; }
    }
}
