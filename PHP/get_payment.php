<?php

$curl = curl_init();

curl_setopt_array($curl, array(
  CURLOPT_URL => "https://sandbox.api.mxmerchant.com/checkout/v3/payment/83133741",
  CURLOPT_RETURNTRANSFER => true,
  CURLOPT_ENCODING => "",
  CURLOPT_MAXREDIRS => 10,
  CURLOPT_TIMEOUT => 0,
  CURLOPT_FOLLOWLOCATION => true,
  CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
  CURLOPT_CUSTOMREQUEST => "GET",
  CURLOPT_HTTPHEADER => array(
    "Authorization: Basic TE9iaDFWZTdGbkFMVDRTY3NIMG8xZ1JDOm1XZEc0ZXpHalVjU2pTckJBd2N3QnNVSE53ST0=",
    "User-Agent: PostmanRuntime/7.22.0",
    "Accept: */*",
    "Cache-Control: no-cache",
    "Postman-Token: 51c22522-adbc-43d1-bc83-16df44965eea",
    "Host: sandbox.api.mxmerchant.com",
    "Accept-Encoding: gzip, deflate, br",
    "Connection: keep-alive"
  ),
));

$response = curl_exec($curl);

curl_close($curl);
echo $response;
