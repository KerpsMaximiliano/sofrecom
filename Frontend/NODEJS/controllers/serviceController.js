var express = require('express');
var mongoose = require('mongoose');
var serviceModel = mongoose.model('Service');
var customerModel = mongoose.model('Customer');
var net = require('../config/net');
var http = require('http');
var querystring = require('querystring');

//var url = net.customerUrl;


exports.getServices = function(req, res){
    var customerId = req.params.customerId;
    serviceModel.find({customerId: customerId}, function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
            res.status(200).jsonp(result);
        }
    });
};

exports.getService = function(req, res){
    serviceModel.findById(req.params.id , function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
            res.status(200).jsonp(result);
        }
    });
};

exports.addService = function(req, res){
    var model = new serviceModel();
    var customerId = req.params.customerId;

    serviceModel.findOne().sort('-id').exec(function(err, item) {
        var id = 1;
        if (item && item._doc && item._doc.id){
            id = item._doc.id + 1;
        }else{
            id = 1;
        }
        
        model.id = id;
        model.customerId = customerId;
        model.active = true;
        model.startDate = new Date();
        model.description = req.body.description;

        model.save(function(err, result){
            if(err){
                res.status(500).send(err.message);
            } else{
                res.status(200).jsonp(result);
                /*
                //grabo la relacion con customers

                //busco el customer
                customerModel.find({id: customerId}, function(custErr, custResult){
                    if(custErr){
                        res.status(500).send(custErr.message);
                    } else{
                        //le agrego el servicio
                        custResult[0]._doc.services.push(model);

                        //hago el update
                        customerModel.update({id: customerId}, custResult[0]._doc, function(err2, result2){
                                if(err2){
                                    res.status(500).send(err2.message);
                                } else{
                                    res.status(200).jsonp(result2);
                                }
                        });
                    }
                });
                //FIN grabo la relacion con customers
                */

            } // fin else

        }); //fin save
    });

    //addToNet(model);

}


exports.activateService = function(req, res){
    var serviceId = req.params.id;

    serviceModel.find({id: serviceId}, function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
              result[0]._doc.active = true;
              result[0]._doc.startDate = new Date();
              result[0]._doc.endDate = null;

              serviceModel.update({id: serviceId}, result[0]._doc, function(err2, result2){
                    if(err){
                        res.status(500).send(err2.message);
                    } else{
                        res.status(200).jsonp(result2);
                    }
              });
        }
    });
}

exports.deActivateService = function(req, res){
    var serviceId = req.params.id;

    serviceModel.find({id: serviceId}, function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
              result[0]._doc.active = false;
              result[0]._doc.startDate = null;
              result[0]._doc.endDate = new Date();

              serviceModel.update({id: serviceId}, result[0]._doc, function(err2, result2){
                    if(err){
                        res.status(500).send(err2.message);
                    } else{
                        res.status(200).jsonp(result2);
                    }
              });
        }
    });
}



