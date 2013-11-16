using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TfsApi.WorkItemTracking;
using Changeset = Microsoft.TeamFoundation.VersionControl.Client.Changeset;

namespace TFS.AutoAttachWorkItemIDs
{
    public class AutoAttachWorkItemIDsSubscriber : ISubscriber
    {
        public string Name
        {
            get { return "TFS.AutoAttachWorkItemIDs"; }
        }

        public SubscriberPriority Priority
        {
            get { return SubscriberPriority.Normal; }
        }

        public EventNotificationStatus ProcessEvent(TeamFoundationRequestContext requestContext, NotificationType notificationType, object notificationEventArgs, out int statusCode, out string statusMessage, out Microsoft.TeamFoundation.Common.ExceptionPropertyCollection properties)
        {
            statusCode = 0;
            properties = null;
            statusMessage = string.Empty;

            try
            {
                if (notificationType == NotificationType.Notification &&
                    notificationEventArgs is CheckinNotification)
                {
                    CheckinNotification args = notificationEventArgs as CheckinNotification;
                    NumberParser numberParser = new NumberParser();
                    WorkItemStore workItemStore = WorkItemStoreFactory.GetWorkItemStore(GetTFSUri(requestContext));
                    VersionControlServer service = workItemStore.TeamProjectCollection.GetService<VersionControlServer>();
                    Changeset changeset = service.GetChangeset(args.Changeset);
                    foreach (int workItemID in numberParser.Parse(args.Comment))
                    {
                        var workItem = workItemStore.GetWorkItem(workItemID);

                        if (workItem != null)
                        {
                            //now create the link
                            ExternalLink changesetLink = new ExternalLink(
                                workItemStore.RegisteredLinkTypes[ArtifactLinkIds.Changeset],
                                changeset.ArtifactUri.AbsoluteUri);
                            //you should verify if such a link already exists
                            if (!workItem.Links.OfType<ExternalLink>()
                                .Any(l => l.LinkedArtifactUri == changeset.ArtifactUri.AbsoluteUri))
                            {
                                workItem.Links.Add(changesetLink);
                                workItem.Save();
                            }
                        }
                    }
                }
                return EventNotificationStatus.ActionPermitted;
            }
            catch (Exception ex)
            {
                // log the error and fail the check in
                statusMessage = "Error in plugin '" + Name + "', error details: " + ex.ToString();
                EventLog.WriteEntry("TFS Service", statusMessage, EventLogEntryType.Error);
                return EventNotificationStatus.ActionDenied;
            }
        }

        public Type[] SubscribedTypes()
        {
            return new Type[]{ typeof(CheckinNotification) };
        }

        public static Uri GetTFSUri(TeamFoundationRequestContext requestContext)
        {
            return new Uri(requestContext.GetService<TeamFoundationLocationService>().GetServerAccessMapping(requestContext).AccessPoint.Replace("localhost", Environment.MachineName) + "/" + requestContext.ServiceHost.Name);
        }
    }
}
