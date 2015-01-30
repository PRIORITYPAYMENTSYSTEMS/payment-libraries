import base64
import datetime
import hmac
import hashlib
from hashlib import md5
import random
import re
import time
import urllib
import urllib2
import traceback
import xml.sax
import httplib
import sys
import cgi
import enterCreds



class ParamsAndHeaders:

    def __init__(self, method,link,consumerKey,consumerSecret,token,tokenSecret):
        self.method=method
        self.link=link
        self.consumerKey=enterCreds.consumerKey
        self.consumerSecret=enterCreds.consumerSecret
        self.token=token
        self.tokenSecret=tokenSecret
        
    def setupParams(self):
        self.params = {
          'oauth_consumer_key':self.consumerKey,
          'oauth_nonce':'1234',
          'oauth_signature_method':'HMAC-SHA1',
          'oauth_timestamp': "1360271497",
          'oauth_version':'1.0'
         }
        if self.token:
            self.params['oauth_token']=self.token
        return ksort(self.params)
            
            
        
    def makeQuery(self):
        query=concatSafeChar(self.setupParams())
        return query
        
    
    def makeBaseString(self):
        parts = [
                 self.method,
                 urlencode_oauth(self.link),
                 urlencode_oauth(self.makeQuery())
                 ];
    
        base_string='&'.join(parts);
        return base_string
    
    def makeSignature(self):
        keySig = urlencode_oauth(self.consumerSecret) + '&';
    
        hashed = hmac.new(keySig, self.makeBaseString(), hashlib.sha1) 
        signature = base64.b64encode(hashed.digest());
    
        self.params['oauth_signature']=signature;
        return self.params
    
    def createHeader(self):
        headerString=[]
        for (key, value) in self.makeSignature().items():
            headerString.append( key + '="' + urlencode_oauth(value) + '"');
        
        headerString = ','.join(headerString);
        
        headers = {
               'Authorization: ' : 'OAuth ' + headerString,
               #'Content-Type: ' : 'application/json',
               'Content-Length: ' : '0'
               }
        return ksort(headers)
    
    


def urlencode_oauth(str):
    str2=urllib.quote(str)
    str3=str2.replace('/', "%2F")
    return str3
 
def concatSafeChar(str) :
    #url encode per RFC 5489
    str=urllib.urlencode(str);
    str2 = str.replace('%7E','~');
    str3 = str2.replace('+',' ');
    return str3;
    
def ksort(d):
     return [(k,d[k]) for k in sorted(d.keys())]   
              