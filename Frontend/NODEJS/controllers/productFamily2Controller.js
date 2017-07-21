var mongoose = require('mongoose');
var productFamily2Model = mongoose.model('ProductFamily2');

exports.getProductFamilies2 = function(req, res){
    productFamily2Model.find(function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
            res.status(200).jsonp(result);
        }
    });
};

exports.addProductFamily2 = function(req, res){
    var item = new productFamily2Model();
    item.name = req.body.name;

    item.save(function(err, result){
        if(err){
            res.status(500).send(err.message);
        } else{
            res.status(200).jsonp(result);
        }
    });
}