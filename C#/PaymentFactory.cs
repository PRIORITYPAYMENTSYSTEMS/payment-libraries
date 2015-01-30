using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Card_Account_Transactions
{
        [DataContract]
        public class PaymentFactory
        {
        [DataMember]
        public string merchantId;
 
        [DataMember]
        public string tenderType;
 
        [DataMember]
        public string amount;
 
        [DataMember]
        public cardAccount cardAccount;
 
        }
 
        [DataContract]
        public class cardAccount
        {
        [DataMember]
        public string number;
 
        [DataMember]
        public string expiryMonth;
 
        [DataMember]
        public string expiryYear;
 
        [DataMember]
        public string cvv;
 
        [DataMember]
        public string avsZip;
 
        [DataMember]
        public string avsStreet;
        }
}

