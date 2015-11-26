# ssqsignon-authproxy

[SSQ signon](https://ssqsignon.com) token endpoint proxy for 

- [Node.js](https://nodejs.org/)
- [ASP.Net Web API 2](http://www.asp.net/web-api)

This module lets you easily proxy HTTPS requests to the 
*SSQ signon online authorization server's token endpoint* through your web server.

You may use this proxy whenever the *client* registered with *SSQ signon* that represents your app has a *client secret* set.
The typical use case is that your app uses *Single Sign On* to log users in with another app. 

## Node.js

### Install

    $ npm install ssqsignon-authproxy

### Usage

#### Setting up the proxy

The *Node.js* version is implemented as a [Connect](http://www.senchalabs.org/connect/)-style middleware.
All you have to do is specify the URL address, and configure the proxy with your *SSQ signon module name*,
and the *client id* and *client secret* of the desired client in your *module*.

    app.use('/myauth', SsqSignonAuthProxy('my-module-name', '1234', 'my-client-secret'));
    
From now on all requests to `/myauth` will be forwarded to `https://ssqsignon.com/my-module-name/auth`, and their respective 
responses returned.

#### SsqSignonAuthProxy(moduleName, clientId, clientSecret, grantTypeDetection)

- *moduleName (string)*: The name of the module to proxy to.
- *clientId (string)*: The id of the client to proxy as. If a *client_id* is not present is the request it will be automatically appended.
- *clientSecret (string)*: The secret of the client to proxy as. If set the client id and secret will be appended as basic authentication to each request.
- *grantTypeDetection (bool)*: If set to a truthy value, the proxy will try to figure out the grant type if the *grant_type* field is empty, i.e.
    - when either *username* or *password* fields are set, it will set the grant type as *password*.
    - when the *code* field is set, it will set the grant type to *authorization_code*.
    - when the *refresh_token* field is set, it will set the grant type to *refresh_token*.

## ASP.Net Web API 2

### Install

    $ nuget install SSQSignon.AuthProxy

### Usage

#### Setting up the proxy

The *ASP.Net Web API 2* version is implemented as an abstract class `AuthProxyController` that inherits from `ApiController`.
All you have to is inherit your controller from `AuthProxyController` and specify your *SSQ signon module name*,
*client id* and *client secret* of the desired client (registered with your module) in the constructor.

    public class MyAuthController : AuthProxyController
    {
        public AuthController()
            :base("my-module-name", "1234", "my-client-secret")
        {
        }
    }
    
From now on all requests to `MyAuthController` will be forwarded to `https://ssqsignon.com/my-module-name/auth`, and their respective 
responses returned.

#### AuthProxyController(moduleName, clientId, clientSecret, grantTypeDetection)

- *moduleName (string)*: The name of the module to proxy to.
- *clientId (string)*: The id of the client to proxy as. If a *client_id* is not present is the request it will be automatically appended.
- *clientSecret (string)*: The secret of the client to proxy as. If set the client id and secret will be appended as basic authentication to each request.
- *grantTypeDetection (bool, defaults to false)*: If set to true, the proxy will try to figure out the grant type if the *grant_type* field is empty, i.e.
    - when either *username* or *password* fields are set, it will set the grant type as *password*.
    - when the *code* field is set, it will set the grant type to *authorization_code*.
    - when the *refresh_token* field is set, it will set the grant type to *refresh_token*.

## How it works

The proxy dispatches every received request (query string and headers included) over HTTPS to the *token endpoint* of your *SSQ signon* module.
A successful response from the *token endpoint* will then be forwarded back to the requester. If the *token endpoint* response contains an error code,
it's status and body will be wrapped into the body of a response with status 502 (bad gateway). If the request to *SSQ signon* fails, a response with 
code 500 (internal server error) will be returned.

## Examples

For a complete, working example, refer to one of the *SSQ slave apps* in the [SSQ signon examples](https://github.com/rivierasolutions/ssqsignon-examples) repository.

## Credits

  - [Riviera Solutions](https://github.com/rivierasolutions)

## License

[The MIT License](http://opensource.org/licenses/MIT)

Copyright (c) 2015 Riviera Solutions Piotr Wójcik <[http://rivierasoltions.pl](http://rivierasolutions.pl)>
