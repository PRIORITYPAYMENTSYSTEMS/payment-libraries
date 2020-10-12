<?php

$curl = curl_init();

curl_setopt_array($curl, array(
  CURLOPT_URL => "https://sandbox.api.mxmerchant.com/checkout/v3/payment/89089316?echo=true",
  CURLOPT_RETURNTRANSFER => true,
  CURLOPT_ENCODING => "",
  CURLOPT_MAXREDIRS => 10,
  CURLOPT_TIMEOUT => 0,
  CURLOPT_FOLLOWLOCATION => true,
  CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
  CURLOPT_CUSTOMREQUEST => "DELETE",
  CURLOPT_HTTPHEADER => array(
    "Authorization: Basic TE9iaDFWZTdGbkFMVcRTY3NIMG8xZ1JDOm1X7Ec0ZXpHalVjU2pTckJBd2N3ccNVSE53ST0=",
    "User-Agent: PostmanRuntime/7.22.0",
    "Accept: */*",
    "Cache-Control: no-cache",
    "Postman-Token: b05eaaba-28c9-4c26-8c8b-3fa856771374",
    "Host: sandbox.api.mxmerchant.com",
    "Accept-Encoding: gzip, deflate, br",
    "Content-Length: ",
    "Connection: keep-alive"
  ),
));

$response = curl_exec($curl);

curl_close($curl);
echo $response;
