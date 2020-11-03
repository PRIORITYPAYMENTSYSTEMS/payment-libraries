fetch("https://sandbox.api.mxmerchant.com/checkout/v3/payment?echo=true&includeCustomerMatches=false", {
  "method": "POST",
  "headers": {
    "Content-Type": "application/json",
    "Authorization": "Basic dGVzdGFjY291bnQ6ZmFrZXBhc3N3b3Jk"
  },
  "body": "{\"paymentType\":\"Sale\",\"cardAccount\":{\"number\":\"4111111111111111\",\"expiryMonth\":\"06\",\"expiryYear\":\"23\",\"avsZip\":\"30004\",\"cvv\":\"321\"},\"entryClass\":\"WEB\",\"authOnly\":false,\"isAuth\":false,\"isSettleFunds\":true,\"source\":\"API\",\"taxExempt\":false,\"merchantId\":\"000000000\",\"amount\":0.01,\"tenderType\":\"card\",\"cardPresent\":true}"
})
.then(response => {
  console.log(response);
})
.catch(err => {
  console.error(err);
});
