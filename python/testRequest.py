import enterCreds
import OAuth

e=enterCreds;

createParams =  OAuth.ParamsAndHeaders(e.links['request'][1],e.links['request'][0],e.consumerKey,e.consumerSecret,"","")

headers= createParams.createHeader();

str='';
for tuple in headers[0]:
    for item in tuple:
        str+=item;
print str;
        