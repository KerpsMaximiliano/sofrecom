var mongoose = require('mongoose');
var productTypeModel = mongoose.model('ProductType');

exports.getProductTypes = function(req, res){
    productTypeModel.find(function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
            res.status(200).jsonp(result);
        }
    });
};

exports.addProductType = function(req, res){
    var item = new productTypeModel();
    item.name = req.body.name;
    item.productFamily1 = req.body.productFamily1;
    item.productFamily2 = req.body.productFamily2;

    item.save(function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else {
            res.status(200).jsonp(result);
        }
    });
};