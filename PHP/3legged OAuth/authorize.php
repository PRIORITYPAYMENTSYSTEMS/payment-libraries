<?php

    require '3leg.php';
    
    parse_str(getRequestToken($consumerSecret,$links['request'][0],$links['request'][1],$params),$initCall);

    saveOAuthTokens($initCall['oauth_token'], $initCall['oauth_token_secret']);
    authorize( $initCall['oauth_token'],$links['authorize'][0]);
 
?>
