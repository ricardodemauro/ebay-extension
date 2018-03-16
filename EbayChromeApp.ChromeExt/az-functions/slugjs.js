const https = require("https");
const letters = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'w', 'x', 'y', 'z'];
const baseUri = 'https://autosug.ebay.com/autosug?kwd={0}&_jgr=1&sId=0&_ch=0&callback=nil';
const regex = 'kwd=[^&]+';
module.exports = function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');

    if (req.query.name) {

        let data = req.query.name;
        
        let responses = [];
        let completedRequests = 0;

        for(let i = 0; i < letters.length; i++) {
            const letter = letters[i];
            const query = data + ' ' + letter;
            let url = baseUri.replace('{0}', query);
            
            context.log('making request to ' + url);

            https.get(url, res => {
                res.setEncoding("utf8");
                let body = "";
                res.on("data", data => {
                    body += data;
                });
                res.on("end", () => {
                    const content = body.replace("/**/nil(","").replace("}})", "}}");
                    const json = JSON.parse(content);
                    const query = /kwd=([^&]+)/g.exec(url)[1]

                    completedRequests++;
                    responses.push({query: query, body: json.res.sug });

                    if(completedRequests == letters.length) {
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
            body: "Incorrect parameters format"
        };
        context.done();
    }
    
};