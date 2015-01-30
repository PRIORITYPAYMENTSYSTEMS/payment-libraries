<?php
 
require 'ConfigFactory.php';
require 'OAuthRequest.php';


  $c = new Creds();
  $creds = $c->creds();
  $links = $c->links();
  $accessTokenData = getAccessToken($links, $creds);

    $customerInfo = array(
        "name"=>""
    );

    $customer = new OAuthRequest($endpoint['postCustomer'], $accessTokenData);
    $customer->createHeader($customerInfo);
    echo sendRequest($customerInfo);
        
?>