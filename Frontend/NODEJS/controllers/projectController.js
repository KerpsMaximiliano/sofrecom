var express = require('express');
var mongoose = require('mongoose');
var projectModel = mongoose.model('Project');
var serviceModel = mongoose.model('Service');
var net = require('../config/net');
var http = require('http');
var querystring = require('querystring');

//var url = net.customerUrl;


exports.getProjects = function(req, res){
    var serviceId = req.params.serviceId;
    projectModel.find({serviceId: serviceId}, function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
            res.status(200).jsonp(result);
        }
    });
};

exports.getProject = function(req, res){
    projectModel.findById(req.params.id , function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
            res.status(200).jsonp(result);
        }
    });
};

exports.addProject = function(req, res){
    var model = new projectModel();
    var serviceId = req.params.serviceId;

    projectModel.findOne().sort('-id').exec(function(err, item) {
        var id = 1;
        if (item && item._doc && item._doc.id){
            id = item._doc.id + 1;
        }else{
            id = 1;
        }
        
        model.id = id;
        model.serviceId = serviceId;
        model.active = true;
        model.startDate = new Date();
        model.description = req.body.description;

        model.save(function(err, result){
            if(err){
                res.status(500).send(err.message);
            } else{
                res.status(200).jsonp(result);
            } // fin else

        }); //fin save
    });

}


exports.activateProject = function(req, res){
    var projectId = req.params.id;

    projectModel.find({id: projectId}, function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
              result[0]._doc.active = true;
              result[0]._doc.startDate = new Date();
              result[0]._doc.endDate = null;

              serviceModel.update({id: projectId}, result[0]._doc, function(err2, result2){
                    if(err){
                        res.status(500).send(err2.message);
                    } else{
                        res.status(200).jsonp(result2);
                    }
              });
        }
    });
}

exports.deActivateProject = function(req, res){
    var projectId = req.params.id;

    projectModel.find({id: projectId}, function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
              result[0]._doc.active = false;
              result[0]._doc.startDate = null;
              result[0]._doc.endDate = new Date();

              serviceModel.update({id: projectId}, result[0]._doc, function(err2, result2){
                    if(err){
                        res.status(500).send(err2.message);
                    } else{
                        res.status(200).jsonp(result2);
                    }
              });
        }
    });
}



