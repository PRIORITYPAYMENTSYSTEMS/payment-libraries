

function OAuthObject(linkObject){

    this.linkObject = linkObject;
    this.credentialObject = Config.credentials;
    this.tokenObject = Config.tokens;

    this.httpResponse;
    this.parameterState = false;
    this.authorization_Headers = '';
    this.parameterAggregate = [];
    
    this.hasParameters = function(){

        if(this.qp_Override){
            this.parameters = qp_Override;
            return true;

        }else{

            if(this.linkObject.parameters){
                this.parameters = this.linkObject.parameters;
                return true;
            }else{
                return false;
            }

        }
    };

    this.addParameters = function(p){

        var QP = this.parameters;

        for(x in QP){
            p[x] = QP[x];
        }

        return p;
    }

    this.OAuthParams = function(){

        var params = {
            'oauth_consumer_key': this.credentialObject.consumerKey,
            'oauth_nonce':  generateNonce(),
            'oauth_signature_method':'HMAC-SHA1',
            'oauth_timestamp': generateTimestamp()
        };

        if(this.tokenObject.oauth_token != ''){
            params['oauth_token'] = this.tokenObject.oauth_token;
        }
        params['oauth_version'] = '1.0';

        if(this.hasParameters()){
            this.parameterState = true;
            params = this.addParameters(params);
        }

        var p = this.lexicograph(params);

        return p;

        function generateNonce(){

            return parseInt( new Date().getTime() / 1000);

        }

        function generateTimestamp(){

            return new Date().getTime();

        }


    }

    this.determineParameters = function(){

        var paramAggregate;
        paramAggregate = this.OAuthParams();

        this.parameterAggregate = paramAggregate;
        return paramAggregate;

    }

    this.getURL = function(){

        var tempURL = this.linkObject.url;
        if(this.id != '' && this.id != null){

            var t = this.linkObject.url;
            tempURL = t.replace("INITIAL_ID", this.id);

            if(this.subId != '' && this.subId != null){

                tempURL = tempURL.replace("SUB_ID", this.subId);
            }
        }

        this.linkObject.url = tempURL;

    };

    this.assembleBaseString = function(){

        var basestring = '';
            basestring += encodeURIComponent(this.linkObject.method) + "&";
            basestring += encodeURIComponent(this.linkObject.url) + "&";
            basestring += encodeURIComponent(normalizeString(this.parameterAggregate));
                
        return basestring;

        function normalizeString(normPieces){
            normString = '';
            count = 0;
            pieceCount = 0;
            for (x in normPieces){
                count++;
            }

            for (x in normPieces){

                if(pieceCount == count - 1){
                    normString += x + "=" + normPieces[x];
                }else{
                    normString += x + "=" + normPieces[x] + "&";
                }

                pieceCount++;

            }

            return normString;

        }
    };
    
    this.makeBaseString = function(){
        
         this.getURL();
        
        return this.assembleBaseString();
    }
    
    this.makeKey = function(){
        
        var keysig = encodeURIComponent(this.credentialObject.consumerSecret) + "&";
            if(this.tokenObject){
                
                keysig += encodeURIComponent(this.tokenObject.oauth_token_secret);
            }
            
        return keysig;
        
    };
    
    this.recaptureParameters = function(){
        
        var actualParameters = this.determineParameters();
        actualParameters['oauth_signature'] = generateHMAC_SHA1Hash(this.makeBaseString(), this.makeKey());
        
        return actualParameters;
        
        function generateHMAC_SHA1Hash(basestring,signatureKey){
            var s = CryptoJS.HmacSHA1( basestring, signatureKey);
            var sig = CryptoJS.enc.Base64.stringify(s);
            return sig;
        }
        
        
    };
    
    this.headerCreate = function(){
        
        return this.concatHeader(this.NVPConvert(this.lexicograph(this.recaptureParameters())));
        
    };
    
    this.NVPConvert = function(arrayPieces){
        
        chunks = [];
        for (key in arrayPieces){    
            chunks.push (key + "=" + arrayPieces[key]);
        }

        
        return chunks;
        
    };
    
    this.lexicograph = function(inputArr, sort_flags){
         
        var tmp_arr = {},
          keys = [],
          sorter, i, k, that = this,
          strictForIn = false,
          populateArr = {};

        switch (sort_flags) {
        case 'SORT_STRING':
          // compare items as strings
          sorter = function (a, b) {
            return that.strnatcmp(a, b);
          };
          break;
        case 'SORT_LOCALE_STRING':
          // compare items as strings, based on the current locale (set with  i18n_loc_set_default() as of PHP6)
          var loc = this.i18n_loc_get_default();
          sorter = this.php_js.i18nLocales[loc].sorting;
          break;
        case 'SORT_NUMERIC':
          // compare items numerically
          sorter = function (a, b) {
            return ((a + 0) - (b + 0));
          };
          break;
          // case 'SORT_REGULAR': // compare items normally (don't change types)
        default:
          sorter = function (a, b) {
            var aFloat = parseFloat(a),
              bFloat = parseFloat(b),
              aNumeric = aFloat + '' === a,
              bNumeric = bFloat + '' === b;
            if (aNumeric && bNumeric) {
              return aFloat > bFloat ? 1 : aFloat < bFloat ? -1 : 0;
            } else if (aNumeric && !bNumeric) {
              return 1;
            } else if (!aNumeric && bNumeric) {
              return -1;
            }
            return a > b ? 1 : a < b ? -1 : 0;
          };
          break;
        }

        // Make a list of key names
        for (k in inputArr) {
          if (inputArr.hasOwnProperty(k)) {
            keys.push(k);
          }
        }
        keys.sort(sorter);

        // BEGIN REDUNDANT
        this.php_js = this.php_js || {};
        this.php_js.ini = this.php_js.ini || {};
        // END REDUNDANT
        strictForIn = this.php_js.ini['phpjs.strictForIn'] && this.php_js.ini['phpjs.strictForIn'].local_value && this.php_js.ini['phpjs.strictForIn'].local_value !== 'off';
        populateArr = strictForIn ? inputArr : populateArr;

        // Rebuild array with sorted key names
        for (i = 0; i < keys.length; i++) {
          k = keys[i];
          tmp_arr[k] = inputArr[k];
          if (strictForIn) {
            delete inputArr[k];
          }
        }
        for (i in tmp_arr) {
          if (tmp_arr.hasOwnProperty(i)) {
            populateArr[i] = tmp_arr[i];
          }
        }

        return strictForIn || populateArr;
          
    };
    
    this.concatHeader = function(normPieces){
        
        var headerString = '';
        for(i = 0; i <= normPieces.length - 1; i++){

            if (i == normPieces.length - 1){
                
                headerString += normPieces[i];
                
            }
            else{
                
                headerString += normPieces[i] + ",";
                
            }
        }

        return headerString;
        
        
    };
    
    this.makeHTTPObject = function(){
        
        try {return new XMLHttpRequest();}
        catch (error) {}
        try {return new ActiveXObject("Msxml2.XMLHTTP");}
        catch (error) {}
        try {return new ActiveXObject("Microsoft.XMLHTTP");}
        catch (error) {}

        throw new Error("Could not create HTTP request object.");
        
    };

    this.getAPIResponse = function(){
        return this.httpResponse;
    };

    this.response_Text = function(){
        return this.httpResponse.responseText;
    }

    this.parsedResponse_Text = function(){
        return JSON.parse(this.response_Text());
    }

    this.please = function(normPieces){
        
        normString = '';
        count = 0;
        pieceCount = 0;
        for (x in normPieces){
            count++;
        }

        for (x in normPieces){
            if(pieceCount == count - 1){
                normString += normPieces[x];
            }else{
                normString +=  normPieces[x] + "&";
            }
            pieceCount++;
        }

        return normString;

    }

    this.setOAuthHeaders = function(){

        var headers = "OAuth " + this.headerCreate();
        this.authorization_Headers = headers;

    }

    this.sendRequest = function(json){
        
        var httpCall = this.makeHTTPObject();
        
        if(this.parameterState){
            this.linkObject.url = this.linkObject.url + "?" + this.please(this.NVPConvert(this.parameters));

        }

        httpCall.open(this.linkObject.method, this.linkObject.url, false);
        httpCall.setRequestHeader("Authorization", this.authorization_Headers);
        httpCall.setRequestHeader("Accept", "application/json");

        if(json != null){

            httpCall.setRequestHeader("Content-Type", "application/json");

        }

        httpCall.send(json);

        this.httpResponse = httpCall;

        return httpCall;

    };

    this.run = function(json){
        this.setOAuthHeaders();
        this.sendRequest(json);
    }
}