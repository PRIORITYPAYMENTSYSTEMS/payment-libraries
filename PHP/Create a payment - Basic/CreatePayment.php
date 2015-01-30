<?php
 
require 'ConfigFactory.php';
require 'OAuthRequest.php';

    $c = new Creds();
    $creds = $c->creds();
    $links = $c->links();
        
    $accessTokenData = getAccessToken($links, $creds);    
    
    $info = array(
        
        "merchantId" => $c->merchantId(),
        "tenderType" => "Card",
        "amount" => '',
        "cardAccount" => array(
            "number" => '',
            "expiryMonth" => '',
            "expiryYear" => '',
            "CVV" => '',
            "avsZip" => ''
         )
        
    );
    
    /*
     * Without including the echo=true query parameter, you're returned the Id of the payment in the Location Header
     * Including it, it is returned with the response. Uncomment the below associative array to see affect on response.
     */
    
    
//    $queryParams=array(
//      "echo"=>"true"  
//    );
    
    $postPayment = new OAuthRequest($links['payment'],$creds, $accessTokenData, $queryParams);
    $postPayment->createHeader($info);
    echo $postPayment->sendRequest($info);
    
?>