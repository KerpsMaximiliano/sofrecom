var mongoose = require('mongoose');
var Schema = mongoose.Schema;

productTypeSchema = new Schema({
    name: {type: String, required:true, unique:true},
    productFamily1: {type: String},
    productFamily2: {type: String}
});

module.exports = mongoose.model('ProductType', productTypeSchema);