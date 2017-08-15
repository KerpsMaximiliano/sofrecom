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

exports.getCustomer = function(req, res){
    customerModel.findById(req.id , function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
            res.status(200).jsonp(result);
        }
    });
};

exports.addCustomer = function(req, res){
    var model = new customerModel();

    customerModel.findOne().sort('-id').exec(function(err, item) {
        var id = 1;
        if (item && item._doc && item._doc.id){
            id = item._doc.id + 1;
        }else{
            id = 1;
        }
        
        model.id = id;
        model.startDate = new Date();
        model.active = true;
        model.description = req.body.description;
        model.email = req.body.email;

        model.save(function(err, result){
            if(err){
                res.status(500).send(err.message);
            } else{
                res.status(200).jsonp(result);
            }
        });
    });

    

    //addToNet(model);

}


exports.activateCustomer = function(req, res){
    var customerId = req.params.id;

    customerModel.find({id: customerId}, function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
              result[0]._doc.active = true;
              result[0]._doc.startDate = new Date();
              result[0]._doc.endDate = null;

              customerModel.update({id: customerId}, result[0]._doc, function(err2, result2){
                    if(err){
                        res.status(500).send(err2.message);
                    } else{
                        res.status(200).jsonp(result2);
                    }
              });
        }
    });
}

exports.deActivateCustomer = function(req, res){
    var customerId = req.params.id;

    customerModel.find({id: customerId}, function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
              result[0]._doc.active = false;
              result[0]._doc.startDate = null;
              result[0]._doc.endDate = new Date();

              customerModel.update({id: customerId}, result[0]._doc, function(err2, result2){
                    if(err){
                        res.status(500).send(err2.message);
                    } else{
                        res.status(200).jsonp(result2);
                    }
              });
        }
    });
}


/*
addToNet = function(model){

    var rpta = {body: '', error: ''};

    postToNet({
        'description' : model.description
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

}*/

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