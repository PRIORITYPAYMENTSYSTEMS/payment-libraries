using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PriorityPaymentTest
{
        [DataContract]
        public class Payment
        {
            [DataMember]
            public string merchantId;

            [DataMember]
            public string tenderType;

            [DataMember]
            public string amount;

            [DataMember]
            public CardAccount cardAccount;

        }

        [DataContract]
        public class CardAccount
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
