<?php

    require '3leg.php';

    $oauth_token = $_GET['oauth_token'];
    $oauth_verifier = $_GET['oauth_verifier'];

        $creds = getCreds();
        
    parse_str(getAccessToken($consumerSecret, $oauth_token, $creds['oauth_token_secret'], $links['access'][0], $links['access'][1], $params, $oauth_verifier), $data);
    echo "oauth_token: ".$data['oauth_token']."<br />";
    echo "oauth_token_secret: ".$data['oauth_token_secret']."<br />";

?>
