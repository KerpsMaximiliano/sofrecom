var net = require('../config/net');
var http = require('http');
var querystring = require('querystring');

postToNet = function(data, fnOk, fnErr){ 

    // Build the post string from an object
    var post_data = querystring.stringify(data);

    // An object of options to indicate where to post to
    var post_options = {
        host: net.host,
        port: net.port,
        path: net.path,
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'Content-Length': Buffer.byteLength(post_data)
        }
    };

    // Set up the request
    var post_req = http.request(post_options, function(res) {
        res.setEncoding('utf8');
        /*res.on('data', function (chunk) {
            rpta.body = chunk;
        });*/
        res.on('data', fnOk);
    });

    /*post_req.on('error', (e) => {
        rpta.error = e.message;
    });*/

    post_req.on('error', fnErr);

    // post the data
    post_req.write(post_data);
    post_req.end();

};

module.exports = postToNet;