fetch("https://sandbox.api.mxmerchant.com/checkout/v3/customer?echo=true", {
    "method": "POST",
    "headers": {
        "Content-Type": "application/json",
        "Authorization": "Basic a2lyc3R5LnJ1c3NlbGw6TXZTbVhtUCExODc1"
    },
    "body": "{\"customerType\":\"Person\",\"isTaxExempt\":false,\"activeStatus\":true,\"spendProfileOverride\":false,\"merchantId\":0,\"name\":\"Customer Name\",\"firstName\":\"Customer\",\"lastName\":\"Name\",\"address1\":\"123 Street Address\",\"city\":\"City\",\"state\":\"State\",\"zip\":\"30004\"}"
})
    .then(response => {
        console.log(response);
    })
    .catch(err => {
        console.error(err);
    });