//using SpadStorePanel.Core.Models;
//using SmsIrRestful;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Text;
//using System.Web;

//namespace SpadStorePanel.Web.Providers
//{
//    public class SMSDotIrProvider:ISMSProvider
//    {
//        #region Panel Info

//        private string Username = "09124802433";
//        private string Password = "87d2a5";
//        private string userApiKey = ConfigurationManager.AppSettings.Get("ApiKey"); 
//        private string secretKey = ConfigurationManager.AppSettings.Get("SecurityCode");

//        #endregion

//        public void SendSMS(ref SMSLog smsLogs)
//        {
//            var token = GetToken();
//            try
//            {
//                var messageSendObject = new MessageSendObject()
//                {
//                    Messages = new List<string> { smsLogs.MessageBody }.ToArray(),
//                    MobileNumbers = new List<string> { smsLogs.ReceiverMobileNo }.ToArray(),
//                    LineNumber = smsLogs.LineNumber,
//                    SendDateTime = null,
//                    CanContinueInCaseOfError = true
//                };

//                MessageSendResponseObject messageSendResponseObject = new MessageSend().Send(token, messageSendObject);

//                if (messageSendResponseObject.IsSuccessful)
//                {
//                    smsLogs.ResponseMessage = messageSendResponseObject.Message;
//                }
//                else
//                {

//                }
//            }
//            catch (Exception ex)
//            {

//            }
//        }

//        public void SendGroupSms(List<SMSLog> smsLogs)
//        {
//            var token = GetToken();
//            try
//            {

//                List<string> numbers = new List<string>();
//                foreach (var log in smsLogs)
//                {
//                    numbers.Add(log.ReceiverMobileNo);
//                }

//                var messageSendObject = new MessageSendObject()
//                {
//                    Messages = new List<string> { smsLogs[0].MessageBody }.ToArray(),
//                    MobileNumbers = new List<string> { smsLogs[0].ReceiverMobileNo }.ToArray(),
//                    LineNumber = smsLogs[0].LineNumber,
//                    SendDateTime = null,
//                    CanContinueInCaseOfError = true
//                };

//                MessageSendResponseObject messageSendResponseObject = new MessageSend().Send(token, messageSendObject);

//                if (messageSendResponseObject.IsSuccessful)
//                {

//                }
//                else
//                {

//                }
//            }
//            catch
//            {

//            }
//        }

//        public void SendSMSByPattern(List<SMSLog> smsLogs, IDictionary<string, string> parameters)
//        {

//        }

//        private string GetToken()
//        {
//            SmsIrRestful.Token tk = new SmsIrRestful.Token();
//            string result = tk.GetToken(userApiKey, secretKey);

//            return result;
//        }
//    }
//}