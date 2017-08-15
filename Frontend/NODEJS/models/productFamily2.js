var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var productFamily2Schema = new Schema({
    name: {type: String, required: true, unique:true}
});

module.exports = mongoose.model('ProductFamily2', productFamily2Schema);