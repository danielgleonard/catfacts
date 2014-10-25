using System;

namespace Google.Voice.Exceptions
{
    public abstract class GoogleException : Exception
    {
        protected const string LOGIN_EXCEPTION = "API could not login";
        protected const string SEARCH_EXCEPTION = "API could not execute search";
        protected const string CALL_EXCEPTION = "API could not initiate call";
        protected const string SMS_EXCEPTION = "API could not send sms";
        protected const string FOLDER_EXCEPTION = "API could not retrieve folder %f";

        public GoogleException(string message)
            : base(message) { }

        public GoogleException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class LoginException : GoogleException
    {
        public LoginException() : base(LOGIN_EXCEPTION) { }
        public LoginException(Exception ex) : base(LOGIN_EXCEPTION, ex) { }
    }

    public class FolderException : GoogleException
    {
        public FolderException(string folder = "unknown") : base(FOLDER_EXCEPTION.Replace("%f", folder)) { }
        public FolderException(string folder, Exception ex) : base(FOLDER_EXCEPTION.Replace("%f", folder), ex) { }
        public FolderException(Exception ex) : base(FOLDER_EXCEPTION.Replace("%f", "unknown"), ex) { }
    }

    public class SearchException : GoogleException
    {
        public SearchException() : base(SEARCH_EXCEPTION) { }
        public SearchException(Exception ex) : base(SEARCH_EXCEPTION, ex) { }
    }

    public class SmsException : GoogleException
    {
        public SmsException() : base(SMS_EXCEPTION) { }
        public SmsException(Exception ex) : base(SMS_EXCEPTION, ex) { }
    }

    public class CallException : GoogleException
    {
        public CallException() : base(CALL_EXCEPTION) { }
        public CallException(Exception ex) : base(CALL_EXCEPTION, ex) { }
    }
}