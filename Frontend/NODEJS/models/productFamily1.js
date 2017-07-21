var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var productFamily1Schema = new Schema({
    name: {type: String, required: true, unique:true}
});

module.exports = mongoose.model('ProductFamily1', productFamily1Schema);