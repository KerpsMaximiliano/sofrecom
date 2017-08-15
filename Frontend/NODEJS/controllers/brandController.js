var mongoose = require('mongoose');
var model = mongoose.model('Brand');

exports.getById = function(req, res){
    model.findById(req.body._id, function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
            res.status(200).jsonp(result);
        }
    });
};

getByName = function(req, res){
    model.findOne({"name": req.params.name}, function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
            res.status(200).jsonp(result);
        }
    });
};

getAll = function(req, res){
    model.find(function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
            res.status(200).jsonp(result);
        }
    });
};

exports.get = function(req, res){
    if(req.params.name){
        if(isValidMongoId(req.params.name)){
            getById(req.params.name);
        } else {
            getByName(req, res);
        }
        
    } else{
        getAll(req, res);
    }
}

exports.add = function(req, res){
    var item = new model();
    item.name = req.body.name;

    item.save(function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
            res.status(200).jsonp(result);
        }
    });
}


exports.edit = function(req, res){
    model.findById(req.body._id, function(err, result){
        if(err){
            res.status(200).jsonp("Brand id: " + req.body._id + " Not Found");
        } else{
            result.name = req.body.name;
            result.save(function(err, persisted){
                if(err){
                    res.status(500).send(err.message);
                } else{
                    res.status(200).jsonp(persisted);
                }
            });
        }
        
    });
    
}

exports.delete = function(req, res){
    
    model.findById(req.body._id, function(err, result){
        if(err){
            res.status(200).jsonp("Brand id: " + req.body._id + " Not Found");
        } else{
            if(result){
                result.remove({"_id": req.body._id}, function(err2){
                    if(err2){
                        res.status(500).send(err.message);
                    } else {
                        res.status(200).jsonp("Brand id: " + req.body._id + " deleted successfully");
                    }
                })
            } else{
                res.status(200).jsonp("Brand id: " + req.body._id + " Not Found");
            }
            
        }
    })
}

