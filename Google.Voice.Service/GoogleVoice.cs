using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Google.Voice.Entities;
using Google.Voice.Extensions;
using Google.Voice.Web;
using Google.Voice.Exceptions;

namespace Google.Voice
{
    public class GoogleVoice
    {

        /// <summary>
        /// WebClient object used when making requests to Google Voice
        /// </summary>
        public virtual WebClient WebClient { get; protected set; }

        /// <summary>
        /// GALX data provided by Google
        /// </summary>
        public string GALX { get; protected set; }

        /// <summary>
        /// Session ID data provided by Google
        /// </summary>
        public string Session { get; protected set; }

        public AccountData Account { get; protected set; }

        /// <summary>
        /// Login parameters used for login requests
        /// </summary>
        protected VariableCollection LoginParameters { get; set; }

        /// <summary>
        /// Creates a new instance of the GoogleVoice service 
        /// </summary>
        public GoogleVoice()
        {
            WebClient = new WebClient();
        }

        /// <summary>
        /// Generates a URL that corresponds to the correct web service endpoint
        /// on Google Voice; this method returns a URL based on the calling method or the supplied 
        /// parameter
        /// </summary>
        protected static string GenerateUrl(string method = null)
        {
            method = method ?? (new StackTrace(1).GetFrame(0).GetMethod().Name);

            switch (method.ToLower())
            {
                case "login":
                    return "https://accounts.google.com/ServiceLoginAuth";

                case "executelogin":
                    return "https://accounts.google.com/ServiceLogin?service=grandcentral&continue=https://www.google.com/voice";

                case "inbox":
                    return "https://www.google.com/voice/b/0/inbox/recent/";

                case "starred":
                    return "https://www.google.com/voice/b/0/inbox/recent/starred/";

                case "spam":
                    return "https://www.google.com/voice/b/0/inbox/recent/spam/";

                case "trash":
                    return "https://www.google.com/voice/b/0/inbox/recent/trash/";

                case "history":
                    return "https://www.google.com/voice/b/0/inbox/recent/all/";

                case "voicemail":
                    return "https://www.google.com/voice/b/0/inbox/recent/voicemail/";

                case "texts":
                    return "https://www.google.com/voice/b/0/inbox/recent/sms/";

                case "recorded":
                    return "https://www.google.com/voice/b/0/inbox/recent/recorded/";

                case "placed":
                    return "https://www.google.com/voice/b/0/inbox/recent/placed/";

                case "missed":
                    return "https://www.google.com/voice/b/0/inbox/recent/missed/";

                case "received":
                    return "https://www.google.com/voice/b/0/inbox/recent/received/";

                case "sms":
                    return "https://www.google.com/voice/b/0/sms/send/";

                case "call":
                    return "https://www.google.com/voice/b/0/call/connect/";

                case "search":
                    return "https://www.google.com/voice/b/0/inbox/search/";

                default: return "http://www.google.com/voice/b/0/";
            }
        }

        /// <summary>
        /// Checks the IGVRequestResult
        /// </summary>
        protected virtual IGVRequestResult CheckRelogin(IGVRequestResult rr, Result result)
        {
            rr.RequiresRelogin = result.Content.Contains("The username or password you entered is incorrect.");

            return rr;
        }

        public bool Logout()
        {
            Session = "";
            return true;
        }

        /// <summary>
        /// Performs a login to Google Voice
        /// </summary>
        public virtual LoginResult Login(string emailAddress, string password)
        {
            try
            {
                var result = WebClient.Get(GenerateUrl());
                GALX = result.GetInputValueByRegEx("GALX");

                VariableCollection parameters = new VariableCollection();

                parameters["continue"].Value = result.GetInputValueByRegEx("continue");
                parameters["followup"].Value = result.GetInputValueByRegEx("followup");
                parameters["dsh"].Value = result.GetInputValueByRegEx("dsh");
                parameters["ltmpl"].Value = result.GetInputValueByRegEx("ltmpl");
                parameters["timeStmp"].Value = result.GetInputValueByRegEx("timeStmp");
                parameters["secTok"].Value = result.GetInputValueByRegEx("secTok");
                parameters["rmShown"].Value = result.GetInputValueByRegEx("rmShown");
                parameters["signIn"].Value = result.GetInputValueByRegEx("signIn");
                parameters["asts"].Value = result.GetInputValueByRegEx("asts");

                parameters["PersistentCookie"].Value = "yes";
                parameters["GALX"].Value = GALX;
                parameters["pstMsg"].Value = 1;
                parameters["service"].Value = "grandcentral";

                return ExecuteLogin(emailAddress, password, parameters);
            }
            catch (Exception ex)
            {
                LoginResult failed = new LoginResult();
                failed.RequiresRelogin = true;
                return failed;
                ///throw new LoginException(ex);
            }
        }

        /// <summary>
        /// Executes the secondary process of POSTing login credentials 
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="password"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual LoginResult ExecuteLogin(string emailAddress, string password, VariableCollection parameters)
        {
            try
            {
                parameters["Email"].Value = emailAddress;
                parameters["Passwd"].Value = password;

                Result result = WebClient.Post(GenerateUrl(), parameters);
                Session = result.GetInputValueByRegExLiberal("_rnr_se");

                string genericInformationPart1 = result.GetBlock("_gcData =", "'contacts'");
                string genericInformationPart2 = "'rank':" + result.GetBlock("'rank':", "};") + "}";

                Account = (genericInformationPart1 + genericInformationPart2).ToObject<AccountData>();

                Account.contacts = new Dictionary<string, Contact>();
                var contactDetails = result.GetBlocks("{\"contactId\":", true, "{\"contactId\":", "'rank':");
                foreach (var contactDetail in contactDetails)
                {
                    string json = contactDetail.Substring(0, contactDetail.LastIndexOf('}') + 1);
                    Contact contact = json.ToObject<Contact>();
                    if (!Account.contacts.ContainsKey(contact.contactId) || Account.contacts[contact.contactId].phoneNumber == null)
                    {
                        Account.contacts[contact.contactId] = contact;
                    }
                }

                LoginResult returnValue = new LoginResult();
                CheckRelogin(returnValue, result);

                if (!returnValue.RequiresRelogin)
                {
                    LoginParameters = parameters;
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                throw new LoginException(ex);
            }
        }

        /// <summary>
        /// Re-logins to the Google Voice service
        /// </summary>
        /// <returns></returns>
        protected virtual LoginResult ExecuteRelogin()
        {
            try
            {
                string emailAddress = LoginParameters["Email"].ToString();
                string password = LoginParameters["Passwd"].ToString();

                return Login(emailAddress, password);
            }
            catch (Exception ex)
            {
                throw new LoginException(ex);
            }
        }

        /// <summary>
        /// Re-logins to the Google Voice service and executes a request
        /// </summary>
        /// <typeparam name="T">Type of object to return</typeparam>
        /// <param name="result">The object to return</param>
        /// <param name="method">Method to execute that returns the object</param>
        /// <param name="parameters">Parameters to provide to the object</param>
        protected virtual bool ReloginAnd<T>(out T result, MethodBase method = null, params object[] parameters)
        {
            method = method ?? new StackTrace(1).GetFrame(0).GetMethod();

            result = default(T);

            if (ExecuteRelogin().RequiresRelogin)
            {
                throw new LoginException();
            }
            else
            {
                result = (T)method.Invoke(this, parameters);
                return true;
            }
        }

        /// <summary>
        /// Re-logins to the Google Voice service and executes a request
        /// </summary>
        /// <typeparam name="T">Type of object to return</typeparam>
        /// <param name="result">The object to return</param>
        /// <param name="parameters">Parameters to provide to the object</param>
        protected virtual bool ReloginAnd<T>(out T result, params object[] parameters)
        {
            return ReloginAnd<T>(out result, new StackTrace(1).GetFrame(0).GetMethod(), parameters);
        }

        /// <summary>
        /// Generates generic parameters based on supplied parameter data
        /// </summary>
        /// <returns></returns>
        private static VariableCollection GenerateParameters(uint? pageNumber = null)
        {
            VariableCollection parameters = new VariableCollection();
            if (pageNumber.HasValue)
            {
                parameters["page"].Value = "p" + pageNumber;
            }
            return parameters;
        }

        /// <summary>
        /// Sends a SMS message to the destination phone number with the specified message
        /// </summary>
        public bool SMS(string destinationPhoneNumber, string message)
        {
            try
            {
                VariableCollection parameters = GenerateParameters();

                parameters["phoneNumber"].Value = destinationPhoneNumber;
                parameters["text"].Value = message;
                parameters["sendErrorSms"].Value = 0;
                parameters["_rnr_se"].Value = Session;

                var result = WebClient.Post(GenerateUrl(), parameters);

                GetFolderResult returnValue = new GetFolderResult();
                var dictionary = result.Content.ToObject<Dictionary<string, object>>();

                return Convert.ToBoolean(dictionary["ok"]);
            }
            catch (Exception ex)
            {
                throw new SmsException(ex);
            }
        }

        /// <summary>
        /// Initiates a call to the destination phone number from the local phone
        /// </summary>
        public bool Call(string localPhone, string destinationPhoneNumber)
        {
            try
            {
                VariableCollection parameters = GenerateParameters();

                parameters["outgoingNumber"].Value = destinationPhoneNumber;
                parameters["forwardingNumber"].Value = localPhone;
                parameters["subscriberNumber"].Value = "undefined";
                parameters["phoneType"].Value = 2;
                parameters["remember"].Value = 0;
                parameters["_rnr_se"].Value = Session;

                var result = WebClient.Post(GenerateUrl(), parameters);

                GetFolderResult returnValue = new GetFolderResult();
                var dictionary = result.Content.ToObject<Dictionary<string, object>>();

                return Convert.ToBoolean(dictionary["ok"]);
            }
            catch (Exception ex)
            {
                throw new CallException(ex);
            }
        }

        /// <summary>
        /// Retrieves a collection of contacts
        /// </summary>
        /// <param name="reload">Reload all contacts from the server</param>
        public Dictionary<string, Contact> GetContacts(bool reload = false)
        {
            if (reload)
            {
                if (ExecuteRelogin().RequiresRelogin)
                {
                    throw new LoginException();
                }
            }

            return Account.contacts;
        }

        /// <summary>
        /// Performs a search for the specified term
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve</param>
        public GetFolderResult Search(string term, uint? pageNumber = null)
        {
            try
            {
                VariableCollection parameters = GenerateParameters(pageNumber);

                parameters["q"].Value = term;

                var result = WebClient.Get(GenerateUrl(), parameters);

                GetFolderResult returnValue = new GetFolderResult();
                string json = result.GetJsonFromXml();

                returnValue = json.ToObject<GetFolderResult>();

                return returnValue;
            }
            catch (Exception ex)
            {
                throw new SearchException(ex);
            }
        }

        /// <summary>
        /// Retrieves the Inbox folder
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve</param>
        /// <param name="urlOverride">URL override, if retrieving from a different location</param>
        public GetFolderResult Inbox(uint? pageNumber = null, string urlOverride = null, bool unread = false)
        {
            string url = (urlOverride ?? GenerateUrl()) + (unread ? "unread/" : "");
            VariableCollection parameters = GenerateParameters(pageNumber);

            try
            {
                var result = WebClient.Get(url, parameters);

                GetFolderResult returnValue = new GetFolderResult();
                string json = result.GetJsonFromXml();

                try
                {
                    returnValue = json.ToObject<GetFolderResult>();
                }
                catch (Exception ex)
                {
                    object[] methodParameters = { pageNumber, urlOverride };

                    if (!ReloginAnd<GetFolderResult>(out returnValue, pageNumber, urlOverride))
                    {
                        throw new FolderException(url, ex);
                    }
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                throw new FolderException(url, ex);
            }
        }

        /// <summary>
        /// Retrieves the Starred folder
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve</param>
        public GetFolderResult Starred(uint? pageNumber = null, bool unread = false)
        {
            try
            {
                return Inbox(pageNumber, GenerateUrl(), unread);
            }
            catch (Exception ex)
            {
                throw new FolderException(MethodInfo.GetCurrentMethod().Name.ToLower(), ex);
            }
        }

        /// <summary>
        /// Retrieves the Starred folder
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve</param>
        public GetFolderResult Spam(uint? pageNumber = null, bool unread = false)
        {
            try
            {
                return Inbox(pageNumber, GenerateUrl(), unread);
            }
            catch (Exception ex)
            {
                throw new FolderException(MethodInfo.GetCurrentMethod().Name.ToLower(), ex);
            }
        }

        /// <summary>
        /// Retrieves the Trash folder
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve</param>
        public GetFolderResult Trash(uint? pageNumber = null, bool unread = false)
        {
            try
            {
                return Inbox(pageNumber, GenerateUrl(), unread);
            }
            catch (Exception ex)
            {
                throw new FolderException(MethodInfo.GetCurrentMethod().Name.ToLower(), ex);
            }
        }

        /// <summary>
        /// Retrieves the History folder
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve</param>
        public GetFolderResult History(uint? pageNumber = null, bool unread = false)
        {
            try
            {
                return Inbox(pageNumber, GenerateUrl(), unread);
            }
            catch (Exception ex)
            {
                throw new FolderException(MethodInfo.GetCurrentMethod().Name.ToLower(), ex);
            }
        }

        /// <summary>
        /// Retrieves the Voicemail folder
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve</param>
        public GetFolderResult Voicemail(uint? pageNumber = null, bool unread = false)
        {
            try
            {
                return Inbox(pageNumber, GenerateUrl(), unread);
            }
            catch (Exception ex)
            {
                throw new FolderException(MethodInfo.GetCurrentMethod().Name.ToLower(), ex);
            }
        }

        /// <summary>
        /// Retrieves the Texts/SMS folder
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve</param>
        public GetFolderResult Texts(uint? pageNumber = null, bool unread = false)
        {
            try
            {
                return Inbox(pageNumber, GenerateUrl(), unread);
            }
            catch (Exception ex)
            {
                throw new FolderException(MethodInfo.GetCurrentMethod().Name.ToLower(), ex);
            }
        }

        /// <summary>
        /// Retrieves the Texts/SMS folder
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve</param>
        public GetFolderResult SMS(uint? pageNumber = null, bool unread = false)
        {
            try
            {
                return Texts(pageNumber);
            }
            catch (Exception ex)
            {
                throw new FolderException(MethodInfo.GetCurrentMethod().Name.ToLower(), ex);
            }
        }

        /// <summary>
        /// Retrieves the Recorded folder
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve</param>
        public GetFolderResult Recorded(uint? pageNumber = null, bool unread = false)
        {
            try
            {
                return Inbox(pageNumber, GenerateUrl(), unread);
            }
            catch (Exception ex)
            {
                throw new FolderException(MethodInfo.GetCurrentMethod().Name.ToLower(), ex);
            }
        }

        /// <summary>
        /// Retrieves the Placed folder
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve</param>
        public GetFolderResult Placed(uint? pageNumber = null, bool unread = false)
        {
            try
            {
                return Inbox(pageNumber, GenerateUrl(), unread);
            }
            catch (Exception ex)
            {
                throw new FolderException(MethodInfo.GetCurrentMethod().Name.ToLower(), ex);
            }
        }

        /// <summary>
        /// Retrieves the Missed folder
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve</param>
        public GetFolderResult Missed(uint? pageNumber = null, bool unread = false)
        {
            try
            {
                return Inbox(pageNumber, GenerateUrl(), unread);
            }
            catch (Exception ex)
            {
                throw new FolderException(MethodInfo.GetCurrentMethod().Name.ToLower(), ex);
            }
        }

        /// <summary>
        /// Retrieves the Received folder
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve</param>
        public GetFolderResult Received(uint? pageNumber = null, bool unread = false)
        {
            try
            {
                return Inbox(pageNumber, GenerateUrl(), unread);
            }
            catch (Exception ex)
            {
                throw new FolderException(MethodInfo.GetCurrentMethod().Name.ToLower(), ex);
            }
        }
    }
}