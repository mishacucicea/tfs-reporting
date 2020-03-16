using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TfsConnector
{
    public static class WorkItemExtensions
    {
        public static TfsTask ToTfsTask(this WorkItem workItem)
        {
            var item = new TfsTask{
                Id = workItem.Id,
                AssignedTo = (string)workItem.Fields[CoreField.AssignedTo].Value,
                Title = workItem.Title,
                Tags = workItem.Tags,
                CreatedDate = workItem.CreatedDate
            };

            // put task specific properties
            if (workItem.Type.Name == "Task")
            {
                if (workItem.Fields.Contains("Original Estimate"))
                {
                    item.OriginalEstimate = (double)(workItem.Fields["Original Estimate"].Value ?? 0d);
                }
                if (workItem.Fields.Contains("Completed Work"))
                {
                    item.CompletedWork = (double)(workItem.Fields["Completed Work"].Value ?? 0d);
                }
                if (workItem.Fields.Contains("Remaining Work"))
                {
                    item.RemainingWork = (double)(workItem.Fields["Remaining Work"].Value ?? 0d);
                }

                var parent = workItem.WorkItemLinks.Cast<WorkItemLink>().
                    FirstOrDefault(link=>link.LinkTypeEnd.Name.Equals("Parent"));

                if (parent!= null)
                {
                    item.Parent = parent.TargetId;
                }
            }

            if (workItem.Fields.Contains("Story Points"))
                item.OriginalEstimate = (double)(workItem.Fields["Story Points"].Value ?? 0d);

            return item;
        }

        public static TfsStory ToTfsStory(this WorkItem workItem)
        {
            var item = new TfsStory()
            {
                Id = workItem.Id,
                AssignedTo = (string)workItem.Fields[CoreField.AssignedTo].Value,
                Title = workItem.Title,
                CreatedDate = workItem.CreatedDate,
                IterationPath = workItem.IterationPath,
                State = workItem.State
            };
            // put user story specific properties
            if (workItem.Type.Name == "User Story")
            {
                if (workItem.Fields.Contains("Story Points"))
                    item.StoryPoints = (double)(workItem.Fields["Story Points"].Value ?? 0d);
                if (workItem.Fields.Contains("Start Date"))
                    item.CreatedDate = (DateTime)(workItem.Fields["Created Date"].Value ?? DateTime.MinValue);
                if (workItem.Fields.Contains("Closed Date"))
                    item.ClosedDate = (DateTime?)workItem.Fields["Closed Date"].Value;


                List<Revision> revisions = workItem.Revisions.ToList();

                Revision initialEstimationRevision = (from revision in revisions
                                                      where revision.Fields["Story Points"].Value != null
                                                      select revision).FirstOrDefault();

                var initialEstimateSP = initialEstimationRevision != null ? initialEstimationRevision.Fields["Story Points"].Value : 0d;

                item.FirstEstimationDate = initialEstimationRevision != null && initialEstimationRevision.Fields.Contains("State Change Date") ? (DateTime?)(initialEstimationRevision.Fields["State Change Date"].Value) : null;

            }
            return item;
        }

        public static List<Revision> ToList(this RevisionCollection revCollection)
        {
            List<Revision> revisions = new List<Revision>();

            foreach (Revision revision in revCollection)
            {
                revisions.Add(revision);
            }
            return revisions;
        }
    }
}
