var mongoose = require('mongoose');
var productFamily1Model = mongoose.model('ProductFamily1');

exports.getProductFamilies1 = function(req, res){
    productFamily1Model.find(function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
            res.status(200).jsonp(result);
        }
    });
};

exports.addProductFamily1 = function(req, res){
    var item = new productFamily1Model();
    item.name = req.body.name;

    item.save(function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
            res.status(200).jsonp(result);
        }
    });
}

exports.editProductFamily1 = function(req, res){
    productFamily1Model.findById(req.body._id, function(err, result){
        if(err){
            res.status(200).jsonp("Product Family 1 id: " + req.body._id + " Not Found");
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

exports.deleteProductFamily1 = function(req, res){
    
    productFamily1Model.findById(req.body._id, function(err, result){
        if(err){
            res.status(200).jsonp("Product Family 1 id: " + req.body._id + " Not Found");
        } else{
            if(result){
                result.remove({"_id": req.body._id}, function(err2){
                    if(err2){
                        res.status(500).send(err.message);
                    } else {
                        res.status(200).jsonp("Product Family 1 id: " + req.body._id + " deleted successfully");
                    }
                })
            } else{
                res.status(200).jsonp("Product Family 1 id: " + req.body._id + " Not Found");
            }
            
        }
    })
}