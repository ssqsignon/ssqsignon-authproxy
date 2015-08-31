module.exports = function ssqSignonAuthProxy(moduleName, clientId, clientSecret) {

    var bodyParser = require('body-parser'),
        https = require('https');

    return [
        bodyParser.json(),
        function(parentReq, res) {
            getBody(parentReq, function(err, body) {
                var req = https.request({
                    method: 'POST',
                    host: 'tinyusers.azurewebsites.net',
                    path: [ '', moduleName, 'auth' ].join('/'),
                    auth: [ clientId, clientSecret ].join(':'),
                    headers: { 'Content-Type': parentReq.get('Content-Type'), 'Content-Length': body.length }
                }, function(response) {
                    var data = '';
                    response.on('data', function(chunk) {
                        data += chunk;
                    });
                    response.on('end', function() {
                        var parsed = JSON.parse(data);
                        if (response.statusCode == 200) {
                            res.send(parsed);
                        } else {
                            res.status(502).send({ status: response.statusCode, reason: parsed });
                        }
                    });
                }).on('error', function(e) {
                    res.status(500).send({ status: response.statusCode, reason: e });
                });
                req.write(body);
                req.end();
            });
        }
    ];

    function getBody(req, done) {
        done(null, JSON.stringify(req.body));
    }
};