using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace UI_Test_Tool
{
    public static class BrowserOperations
    {
        #region GooglePlay operations

        // General actions
        static string SaveButton = "document.evaluate('//*[text()=\"' + 'Save' + '\"]', document, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE).snapshotItem(0).click()";

        // Console homescreen
        static string AddNewApp = "document.evaluate('//*[text()=\"' + ' Add new Application ' + '\"]', document, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE).snapshotItem(0).click()";
        static string SelectDefaultLanguage = "document.getElementsByClassName(\"gwt-ListBox\")[0].value = \"nl-NL\"; var evt = document.createEvent(\"HTMLEvents\"); evt.initEvent(\"change\", false, true); document.getElementsByClassName(\"gwt-ListBox\")[0].dispatchEvent(evt);";
        static string EnterAppName = "document.getElementsByClassName(\"gwt-TextBox\")[0].value = \"TestApp\"; var evt = document.createEvent(\"HTMLEvents\"); evt.initEvent(\"change\", false, true); document.getElementsByClassName(\"gwt-ListBox\")[0].dispatchEvent(evt);";

        // Store listing
        static string EnterAppDescription = "";
        static string AddScreenshot = "";
        static string AddAppIcon = "";
        static string SelectApplicationType = "";
        static string SelectCategory = "";
        static string SelectContentRating = "";
        static string CheckPolicyCheckBox = "";

        // Upload APK
        static string NavigateToApk = "";
        static string UploadApk = "";
        static string BrowseFiles = "";

        // Pricing & Distribution
        static string NavigateToPricing = "";
        static string SelectAllCountries = "";
        static string CheckContentGuidelines = "";
        static string CheckUsExportLaws = "";

        // Publish app
        //TODO: ADD READY TO PUBLISH CLICKS (x2)

        #endregion
    }
}
