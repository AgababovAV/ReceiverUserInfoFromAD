using System;
using System.Web;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;


namespace Receivers.UserParseEventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class UserParseEventReceiver : SPItemEventReceiver
    {
        /// <summary>
        /// An item was added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            SPWeb web = properties.Web;
            SPListItem item = properties.ListItem;
            item["textUsers"] = String.Empty;

            SPFieldUserValueCollection userValues = new SPFieldUserValueCollection(web, item["manyUsers"].ToString());

            using (SPSite site = properties.Site)
            {
                SPServiceContext serviceContext = SPServiceContext.GetContext(site);
                try
                {
                    UserProfileManager userProfileManager = new UserProfileManager(serviceContext);

                    foreach (SPFieldUserValue userValue in userValues)
                    {
                        if (userProfileManager.UserExists(userValue.User.LoginName))
                        {
                            var userProfile = userProfileManager.GetUserProfile(userValue.User.LoginName);
                            item["textUsers"] += userProfile.DisplayName.Trim().Substring(userProfile.DisplayName.IndexOf('(') + 1).TrimEnd(')') + " / ";
                            item["textUsers"] += userProfile["Title"].Value !=null ? userProfile["Title"] + " / " : "нет информации / ";
                            item["textUsers"] += userProfile["Department"].Value != null ? userProfile["Department"] + ";\n" : "нет информации;\n";   
                        }
                        else
                        {
                            item["textUsers"] += " нет информации; ";
                        }
                    }

                    item.Update();
                }
                catch
                {
                    //Empty
                }
            }
        }

        /// <summary>
        /// An item was updated.
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            SPWeb web = properties.Web;
            SPListItem item = properties.ListItem;
            item["textUsers"] = String.Empty;

            SPFieldUserValueCollection userValues = new SPFieldUserValueCollection(web, item["manyUsers"].ToString());

            using (SPSite site = properties.Site)
            {
                SPServiceContext serviceContext = SPServiceContext.GetContext(site);
                try
                {
                    UserProfileManager userProfileManager = new UserProfileManager(serviceContext);

                    foreach (SPFieldUserValue userValue in userValues)
                    {
                        if (userProfileManager.UserExists(userValue.User.LoginName))
                        {
                            var userProfile = userProfileManager.GetUserProfile(userValue.User.LoginName);
                            item["textUsers"] += userProfile.DisplayName.Trim().Substring(userProfile.DisplayName.IndexOf('(') + 1).TrimEnd(')') + " / ";
                            item["textUsers"] += userProfile["Title"].Value != null ? userProfile["Title"] + " / " :  "нет информации / ";
                            item["textUsers"] += userProfile["Department"].Value != null  ? userProfile["Department"] + "; " : "нет информации; ";
                        }
                        else {
                            item["textUsers"] += " нет информации; ";
                        }
                    }

                    item.Update();
                }
                catch
                {
                    //Empty
                }
            }
        }


    }
}