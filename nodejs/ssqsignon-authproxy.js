module.exports = function ssqSignonAuthProxy(moduleName, clientId, clientSecret, grantTypeDetection) {

    var bodyParser = require('body-parser'),
        https = require('https');

    return [
        bodyParser.json(),
        bodyParser.urlencoded({ extended: true }),
        function(parentReq, res) {
            var body = parentReq.body;
            if (!body.client_id) {
                body.client_id = clientId;
            }
            if (!body.grant_type && grantTypeDetection) {
                body.grant_type = detectGrantType(body);
            }
            var bodyAsString = JSON.stringify(body);

            var req = https.request({
                method: 'POST',
                host: 'ssqsignon.com',
                path: [ '', moduleName, 'auth' ].join('/'),
                auth: clientSecret ? [ clientId, clientSecret ].join(':') : null,
                headers: { 'Content-Type': 'application/json; charset=utf-8', 'Content-Length': bodyAsString.length }
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
                        res.status(response.statusCode).send(parsed);
                    }
                });
            }).on('error', function(e) {
                res.status(500).send({ status: response.statusCode, reason: e });
            });
            req.write(bodyAsString);
            req.end();
        }
    ];

    function detectGrantType(body) {
        if (body.username || body.password) {
            return 'password';
        }
        if (body.code) {
            return 'authorization_code';
        }
        if (body.refresh_token) {
            return 'refresh_token';
        }
        return null;
    }

    function getBody(req, done) {
        done(null, JSON.stringify(req.body));
    }
};