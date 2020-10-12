<?php

$curl = curl_init();

curl_setopt_array($curl, array(
  CURLOPT_URL => "https://sandbox.api.mxmerchant.com/checkout/v3/customer?echo=true",
  CURLOPT_RETURNTRANSFER => true,
  CURLOPT_ENCODING => "",
  CURLOPT_MAXREDIRS => 10,
  CURLOPT_TIMEOUT => 0,
  CURLOPT_FOLLOWLOCATION => true,
  CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
  CURLOPT_CUSTOMREQUEST => "POST",
  CURLOPT_POSTFIELDS =>"{\r\n    \"merchantId\": 516151174,\r\n    \"name\": \"Johnson Nguyen\",\r\n    \"firstName\": \"Test\",\r\n    \"lastName\": \"Customer\",\r\n    \"address1\": \"221 E Baker St\",\r\n    \"address2\": \"Suite 2\",\r\n    \"city\": \"Dallas\",\r\n    \"state\": \"TX\",\r\n    \"zip\": \"75252\",\r\n    \"addressName\": \"Headquarters\",\r\n    \"customerType\": \"Person\",\r\n    \"spendProfileOverride\": true,\r\n    \"isTaxExempt\": false,\r\n    \"hasPayments\": false,\r\n    \"hasContracts\": false,\r\n    \"hasInvoices\": false,\r\n    \"displayColor\": \"A1BC3A\",\r\n }",
  CURLOPT_HTTPHEADER => array(
    "Content-Type: application/json",
    "Authorization: Basic dVNSMXY4YmVVRUdmZmp1UGhuZm1XQVRJOjBzzTVjcjVURlU5a2ppNS9US1cwWHJwZERXZz0="
  ),
));

$response = curl_exec($curl);

curl_close($curl);
echo $response;
