<?php

$curl = curl_init();

curl_setopt_array($curl, array(
  CURLOPT_URL => "https://sandbox.api.mxmerchant.com/checkout/v3/payment?echo=true",
  CURLOPT_RETURNTRANSFER => true,
  CURLOPT_ENCODING => "",
  CURLOPT_MAXREDIRS => 10,
  CURLOPT_TIMEOUT => 0,
  CURLOPT_FOLLOWLOCATION => true,
  CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
  CURLOPT_CUSTOMREQUEST => "POST",
  CURLOPT_POSTFIELDS =>"{\n    \"merchantId\": \"522158224\",\n    \"tenderType\": \"Card\",\n    \"amount\": \"9.00\",\n    \"paymentType\": \"sale\",\n    \"cardAccount\":{\"number\":\"4100000000000001\",\"expiryMonth\":\"04\",\"expiryYear\":\"2029\"},\n    \"authOnly\":false\n}\n",
  CURLOPT_HTTPHEADER => array(
    "Content-Type: application/json",
    "Authorization: Basic TE9iaDFWZTdGbkFMVDRTY3NIMGzzZ1JDOm1XZEc0ZXpHalVjU2pTzkJBd2N3QnNVSE53ST0="
  ),
));

$response = curl_exec($curl);

curl_close($curl);
echo $response;
