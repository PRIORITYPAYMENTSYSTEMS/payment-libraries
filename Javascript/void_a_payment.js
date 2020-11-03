fetch("https://sandbox.api.mxmerchant.com/checkout/v3/payment/{{paymentId}}", {
    "method": "DELETE",
    "headers": {
        "Authorization": "Basic dGVzdGFjY291bnQ6ZmFrZXBhc3N3b3Jk"
    }
})
    .then(response => {
        console.log(response);
    })
    .catch(err => {
        console.error(err);
    });
