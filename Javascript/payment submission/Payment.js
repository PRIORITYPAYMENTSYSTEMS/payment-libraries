function Payment(){

    var setTokenData = function(response){

        Config.tokens.oauth_token = response.oauth_token;
        Config.tokens.oauth_token_secret = response.oauth_token_secret;

    }

    var Tokens = {

        set: function(){

            var rt = new OAuthObject(Config.Endpoints.request);
                rt.run();
            setTokenData(rt.parsedResponse_Text());

            var at = new OAuthObject(Config.Endpoints.access);
                at.run();
            setTokenData(at.parsedResponse_Text());


        }
    }

    this.post = function(paymentData){

        Tokens.set();

        var postPayment = new OAuthObject(Config.Endpoints.payment);
            postPayment.run(JSON.stringify(paymentData));

    }

}

window.onload = function(){

    var payment = new Payment();
    payment.post({
        merchantId:  Config.merchantId,
        tenderType: "Card",
        cardAccount: {
            number: '4123412341234123',
            expiryMonth: '06',
            expiryYear: '2017',
            CVV: '123',
            avsZip: '12345'
        },
        amount: .01
    });

}