
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
    
}