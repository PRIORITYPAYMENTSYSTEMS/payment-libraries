<?php
    
    require 'Creds.php';
    require 'OAuth.php';

    $c = new Creds();
    $creds = $c->creds();
    $links = $c->links();
    $accessTokenData = getAccessToken($links, $creds);

    $orderInfo = array(
        "merchantId"=> $c->merchantId(),
        "totalAmount"=> "10.00"
    );

    $order = new OAuthRequest($links['postOrder'], $creds, $accessTokenData);
    $order->createHeader($orderInfo);
    print_r ($order->linkArray);
    print_r ($order->sendRequest($orderInfo));

?>