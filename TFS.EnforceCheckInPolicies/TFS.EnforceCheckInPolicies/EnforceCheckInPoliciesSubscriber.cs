namespace TFS.EnforceCheckInPolicies
{
    using System;
    using System.Diagnostics;

    using Microsoft.TeamFoundation.Common;
    using Microsoft.TeamFoundation.Framework.Server;
    using Microsoft.TeamFoundation.VersionControl.Server;

    public class EnforceCheckInPoliciesSubscriber : ISubscriber
    {
        public string Name
        {
            get
            {
                return "TFS.EnforceCheckInPolicies";
            }
        }

        public SubscriberPriority Priority
        {
            get
            {
                return SubscriberPriority.High;
            }
        }

        public EventNotificationStatus ProcessEvent(TeamFoundationRequestContext requestContext, NotificationType notificationType, object notificationEventArgs, out int statusCode, out string statusMessage, out ExceptionPropertyCollection properties)
        {
            statusCode = 0;
            properties = null;
            statusMessage = string.Empty;

            try
            {
                if (notificationType == NotificationType.DecisionPoint &&
                    notificationEventArgs is CheckinNotification)
                {
                    var args = notificationEventArgs as
                               CheckinNotification;
                    if (args.PolicyOverrideInfo.PolicyFailures.Length > 0)
                    {
                        statusMessage = "Policy Overriding is not allowed.";
                        return EventNotificationStatus.ActionDenied;
                    }
                }
                return EventNotificationStatus.ActionPermitted;
            }
            catch (Exception ex)
            {
                // log the error and fail the check in
                statusMessage = "Error in plugin '" + this.Name + "', error details: "
                                + ex;
                EventLog.WriteEntry("TFS Service", statusMessage,
                    EventLogEntryType.Error);
                return EventNotificationStatus.ActionDenied;
            }
        }

        public Type[] SubscribedTypes()
        {
            return new[] { typeof(CheckinNotification) };
        }
    }
}