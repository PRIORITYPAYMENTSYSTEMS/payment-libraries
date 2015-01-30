function API_Demo(){
    
    this.tokenObject={};
    this.idObject={};
    this.endPoints =  getEndpoints();
    
    this.setIdObject = function(id){

        this.idObject = {
            "id" : id
        };
        
    }

    this.captureResourceId = function (location){
        
        return location.substr(location.lastIndexOf('/') + 1, location.length);
        
    }

    this.setTokenData = function (response){
    
        this.tokenObject = {
          "oauth_token": response.oauth_token,
          "oauth_token_secret": response.oauth_token_secret
        };
    
    }
    
    this.getAPIResponse = function(object, json){
        
        object.setOAuthHeaders();
        return object.sendRequest(json);
        
    }
    
    this.setTokens = function() {
        
        rt = new OAuthObject(this.endPoints.requestToken);
        httpResponse = this.getAPIResponse(rt);
        this.setTokenData(JSON.parse(httpResponse.responseText));  

        at = new OAuthObject(this.endPoints.accessToken, this.tokenObject);
        httpResponse = this.getAPIResponse(at);
        this.setTokenData(JSON.parse(httpResponse.responseText));
        
    }
    
    this.postCustomer = function() {
    
        customer = new Object();
            customer.name = "john";

        postCustomer = new OAuthObject(this.endPoints.postCustomer, this.tokenObject);
        httpResponse = this.getAPIResponse(postCustomer, JSON.stringify(customer));
        id = this.captureResourceId(httpResponse.getResponseHeader('Location'))
        this.setIdObject(id);
        
        return id;

        
    }
    
    this.postCustomerCardAccount = function(){
        
         cardAccount = new Object();
            cardAccount.number = "4444555566667777";
            cardAccount.expiryMonth = "07";
            cardAccount.expiryYear = "2020";
            cardAccount.cvv = "180";
            cardAccount.avsZip = "12345";
    
        queryParameters = new Object();
        queryParameters.echo = 'true';
    
        postCustomerCardAccount = new OAuthObject(this.endPoints.postCustomerCardAccount, this.tokenObject, this.idObject, queryParameters);
        httpResponse = this.getAPIResponse(postCustomerCardAccount, JSON.stringify(cardAccount));
        return httpResponse.responseText;
        
    }
    
    this.postPayment = function (){
        
        Payment = new Object();
            Payment.merchantId = getMerchantId() ;
            Payment.tenderType = "Card";
            Payment.amount = ".01";
            Payment.cardAccount = new Object();
            Payment.cardAccount.number = "4444555566667777";
            Payment.cardAccount.expiryMonth = "07";
            Payment.cardAccount.expiryYear = "2020";
            Payment.cardAccount.cvv = "180";
            Payment.cardAccount.avsZip = "30303";

        echo = true;    
    
        postPayment = new OAuthObject(this.endPoints.postPayment, this.tokenObject, null, echo);
        httpResponse = this.getAPIResponse(postPayment, JSON.stringify(Payment));
        paymentId = this.captureResourceId(httpResponse.getResponseHeader('Location'));
        this.setIdObject(paymentId);
    }
    
    this.getPaymentById = function(){
        
        getPaymentById = new OAuthObject(this.endPoints.getPaymentById, this.tokenObject, this.idObject);
        httpResponse = this.getAPIResponse(getPaymentById);
        console.log(httpResponse);
        
    }
    
    
    this.storedCardPayment = function(){
        
        customerId = this.postCustomer();
        cardAccountData = JSON.parse(this.postCustomerCardAccount());
        CVV = '180';
        
        
        Payment = new Object();
            Payment.merchantId = getMerchantId() ;
            Payment.tenderType = "Card";
            Payment.amount = ".01";
            Payment.cardAccount = new Object();
            Payment.cardAccount.id = cardAccountData.id;
            Payment.cardAccount.created = cardAccountData.created;
            Payment.cardAccount.hash = cardAccountData.hash;
            Payment.cardAccount.cardType = cardAccountData.cardType;
            Payment.cardAccount.last4 = cardAccountData.last4;
            Payment.cardAccount.expiryMonth = cardAccountData.expiryMonth;
            Payment.cardAccount.expiryYear = cardAccountData.expiryYear;
            Payment.cardAccount.avsZip = cardAccountData.avsZip;
            Payment.cardAccount.cvv = CVV;

        queryParameters = new Object();
            queryParameters.echo = 'true';
            queryParameters.customerId = customerId;
            queryParameters.id = cardAccountData.id;
    
        postCardAccountToken = new OAuthObject(this.endPoints.postCardAccountToken, this.tokenObject, null, queryParameters);
        httpResponse = this.getAPIResponse(postCardAccountToken, JSON.stringify(Payment));
        paymentId = this.captureResourceId(httpResponse.getResponseHeader('Location'));
        console.log(paymentId);
        
    }
        
}