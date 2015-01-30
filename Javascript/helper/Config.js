
var Config = {
    credentials: {
        "consumerKey" : "",
        "consumerSecret" : ""
    },
    Endpoints: {
        request: {
             url: 'https://sandbox.api.prioritypaymentsystems.com/checkout/v1.1/' + 'oauth/1a/requesttoken'
            ,method: "POST"
        },
        access: {
             url: 'https://sandbox.api.prioritypaymentsystems.com/checkout/v1.1/' + 'oauth/1a/accesstoken'
            ,method: "POST"
        },
        payment:{
            'url': 'https://sandbox.api.prioritypaymentsystems.com/checkout/v1.1/' + 'payment'
            ,method: "POST"
            ,parameters: {
                echo: 'true'
            }
        }
    },
    merchantId: '',
    tokens: {
        oauth_token: '',
        oauth_token_secret: ''
    }
};