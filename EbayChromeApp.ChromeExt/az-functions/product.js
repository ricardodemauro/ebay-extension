const https = require("https");
const baseUri = 'https://svcs.ebay.com/services/search/FindingService/v1?SECURITY-APPNAME=RicardoM-sampleke-PRD-6f1a91299-0d0b7d55&OPERATION-NAME=findItemsByKeywords&SERVICE-VERSION=1.0.0&RESPONSE-DATA-FORMAT=JSON&REST-PAYLOAD&keywords={0}&paginationInput.entriesPerPage=6&GLOBAL-ID=EBAY-US&siteid=0';

module.exports = function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');

    if (req.body && req.body.producties) {
        let responses = [];
        let completedRequests = 0;

        const productCollection = req.body.producties;
        for(let i = 0; i < productCollection.length; i++){
            const product = productCollection[i];

            const url = baseUri.replace('{0}', product);

            https.get(url, res => {
                res.setEncoding("utf8");
                let body = "";
                res.on("data", data => {
                    body += data;
                });
                res.on("end", () => {
                    const json = JSON.parse(body);
                    const query = /keywords=([^&]+)/g.exec(url)[1]

                    completedRequests++;
                    responses.push({ 
                        totalEntries: json.findItemsByKeywordsResponse[0].paginationOutput[0].totalEntries[0], 
                        name: query 
                    });
                    
                    if(completedRequests == productCollection.length) {
                        context.res = {
                            body: responses,
                            status: 200,
                            headers: {
                                "Content-Type" : "application/json"
                            }
                        };
                        context.done();
                    }
                });
            });
        }
    }
    else {
        context.res = {
            status: 400,
            body: "Please pass a name on the query string or in the request body"
        };
        context.done();
    }
    
};