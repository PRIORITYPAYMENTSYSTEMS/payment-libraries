<?php

$consumerKey = '';
$consumerSecret = '';

    $links = array(
        'request'=> array('https://api.prioritypaymentsystems.com/checkout/v1.1/oauth/1a/requesttoken',"POST"),
        'authorize'=> array('https://beta.mxmerchant.com/oauth/authorize',"GET"),
        'access'=> array('https://api.prioritypaymentsystems.com/checkout/v1.1/oauth/1a/accesstoken', "POST")
    );

    $params = array(
        'oauth_callback' => 'http://localhost:8888/Test/3leg/callback.php',
        'oauth_consumer_key' => $consumerKey,
        'oauth_nonce' => sha1(microtime()),
        'oauth_signature_method' => 'HMAC-SHA1',
        'oauth_timestamp' => time(),
        'oauth_version' => '1.0'
    );

    function urlencode_oauth($str) {
      return str_replace('+',' ',str_replace('%7E','~',rawurlencode($str)));
    }

    function getRequestToken($consumerSecret, $link, $method, $params){

        foreach ($params as $key=>$value) {
          $q[] = urlencode_oauth($key).'='.urlencode_oauth($value);
        }

        $q = implode('&',$q);

        $base_string = implode('&',array(
            $method,
            urlencode_oauth($link),
            urlencode_oauth($q)
            ));
        $key = urlencode_oauth($consumerSecret) . '&';

        $signature = base64_encode(hash_hmac('sha1',$base_string,$key,true));

        $params['oauth_signature'] = $signature;


        foreach ($params as $key=>$value) {
          $str[] = $key . '="'.urlencode_oauth($value).'"';
        }

        $str = implode(',',$str);

        $headers = array(
            'Authorization: OAuth '.$str,
            'Content-Length: 0',
        );

        $options = array(
            CURLOPT_HTTPHEADER => $headers, 
            CURLOPT_URL => $link, 
            CURLOPT_POST => true, 
            CURLOPT_RETURNTRANSFER => true, 
            CURLOPT_SSL_VERIFYPEER => false
        ); 

        $ch = curl_init(); 
        curl_setopt_array($ch, $options); 
        $response = curl_exec($ch); 
        curl_close($ch);

        return $response;

    }

    function authorize($token, $link){

        $loc = $link."?oauth_token=".$token;
        echo "<script LANGUAGE='javascript'>window.open('".$loc."','newwin','height=350,width=400,modal=yes,alwaysRaised=yes')</script>";

    }

    function getCreds(){

        $mySQLConnection = mysql_connect('', '', '') or die("Unable to connect to MySQL");
        mysql_select_db("",$mySQLConnection);
        $result = mysql_query("SELECT * FROM tokenStuff");
        $creds = mysql_fetch_array($result);

        return $creds;
    }

    function getAccessToken($consumerSecret, $token, $tokenSecret, $link, $method, $params, $verifier){

        $params['oauth_token'] = $token;
        $params['oauth_verifier'] = $verifier;
        ksort($params);

        foreach ($params as $key=>$value) {
          $q[] = urlencode_oauth($key).'='.urlencode_oauth($value);
        }

        $q = implode('&',$q);

        $parts = array(
            $method,
            urlencode_oauth($link),
            urlencode_oauth($q)
        );

        $base_string = implode('&',$parts);
        $key = urlencode_oauth($consumerSecret).'&'.urlencode_oauth($tokenSecret);
        $signature = base64_encode(hash_hmac('sha1',$base_string,$key,true));

        $params['oauth_signature'] = $signature;

        foreach ($params as $k=>$value) {
          $str[] = $k . '="'.urlencode_oauth($value).'"';
        }

        $str = implode(',',$str);
        $headers = array(
            'Authorization: OAuth '.$str,
            'Content-Type: application/json',
            'Content-Length: 0',
            'Connection: close'
        );

        $options = array(
            CURLOPT_HTTPHEADER => $headers, 
            CURLOPT_URL => $link, 
            CURLOPT_POST => true, 
            CURLOPT_RETURNTRANSFER => true, 
            CURLOPT_SSL_VERIFYPEER => false
        ); 

        $ch = curl_init(); 
        curl_setopt_array($ch, $options); 
        $response = curl_exec($ch); 
        curl_close($ch); 

        return $response;       
    }

    function AccessTokenExists(){

           $creds = getCreds();
            if ($creds['oauth_token'] == "" || $creds['oauth_token'] == null){
                return false;
            }else{
                return true;
            }

    }

    function saveOAuthTokens($token,$tokenSecret){

        $link = mysql_connect('localhost', 'root', 'root') or die("Unable to connect to MySQL");
        mysql_select_db("tempStore",$link);

        $tokeResponse =AccessTokenExists();

        if(!$tokeResponse){

            mysql_query("INSERT INTO tokenStuff(oauth_token,oauth_token_secret) VALUES ('".$token."','".$tokenSecret."')");
        }
        else{
            mysql_query("UPDATE tokenStuff SET  oauth_token='".$token."' , oauth_token_secret='".$tokenSecret."'");
        }
    }
 
?>
