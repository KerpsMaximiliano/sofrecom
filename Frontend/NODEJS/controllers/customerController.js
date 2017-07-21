var express = require('express');
var mongoose = require('mongoose');
var customerModel = mongoose.model('Customer');
var net = require('../config/net');
var http = require('http');
var querystring = require('querystring');
var netServices = require('../services/net');

//var url = net.customerUrl;


exports.getCustomers = function(req, res){
    customerModel.find(function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
            res.status(200).jsonp(result);
        }
    });
};

exports.addCustomer = function(req, res){
    var model = new customerModel();
    model.name = req.body.name;
    model.sql_id = req.body.sql_id;
    model.definite = req.body.definite;
    model.user = req.body.user;

    /*addToNet(model);

    model.save(function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
            res.status(200).jsonp(result);
        }
    });*/
}

addToNet = function(model){

    var rpta = {body: '', error: ''};

    postToNet({
        'name' : model.name
    }, 
    (data) => {
        rpta.body = data;
        console.log(rpta.body);
    }, 
    (err) => {
        rpta.error = err;
        console.log(rpta.error);
    }
    );

}

/*
addToNet = function(model){ 

    // Build the post string from an object
    var post_data = querystring.stringify({
        'name' : model.name
    });

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
        res.on('data', function (chunk) {
            console.log('Response: ' + chunk);
        });
    });

    // post the data
    post_req.write(post_data);
    post_req.end();

}*/