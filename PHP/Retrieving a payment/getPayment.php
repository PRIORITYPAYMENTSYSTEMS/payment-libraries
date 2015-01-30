<?php
 
require 'ConfigFactory.php';
require 'OAuthRequest.php';
 
    $c = new Creds();
    $creds = $c->creds();
    $links = $c->links();
    $accessTokenData = getAccessToken($links, $creds);    
    
    $paymentId = "";
    $payment = new OAuthRequest($endpoint['getPaymentById'], $accessTokenData, null, $paymentId);
    $payment->createHeader();
    echo $payment->sendRequest();
 
?>