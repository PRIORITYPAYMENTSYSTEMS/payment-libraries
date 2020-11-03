fetch("https://sandbox.api.mxmerchant.com/checkout/v3/payment/000000000?includeCustomer=true", {
  "method": "GET",
  "headers": {
    "Authorization": "Basic a2lyc3R5LnJ1c3NlbGw6TXZTbVhtUCExODc1"
  }
})
.then(response => {
  console.log(response);
})
.catch(err => {
  console.error(err);
});