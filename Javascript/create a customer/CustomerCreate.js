
function API_Demo(){
    
    this.tokenObject={};
    this.endPoints =  getEndpoints();
 
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
        
        id = this.captureResourceId(httpResponse.getResponseHeader('Location'));
        console.log(id);
        
    }
    
}